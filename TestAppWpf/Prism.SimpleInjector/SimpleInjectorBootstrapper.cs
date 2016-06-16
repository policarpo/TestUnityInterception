using System;
using System.Globalization;

using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.ServiceLocation;

using Prism.Events;
using Prism.Logging;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Regions;

using SimpleInjector;

namespace Prism.SimpleInjector
{
    public abstract class SimpleInjectorBootstrapper : Prism.Bootstrapper
    {
        private bool _useDefaultConfiguration = true;

        public Container Container { get; protected set; }
        private readonly string LoggerCreatedSuccessfully = "Logger created";
        private readonly string CreatingModuleCatalog = "Creating module catalog";
        private readonly string CreatingSimpleInjectorContainer = "Creating simple injector container";
        private readonly string ConfiguringSimpleInjector;
        private readonly string ConfiguringServiceLocatorSingleton = "Configuring ServiceLocator singleton.";
        private readonly string ConfiguringViewModelLocator = ">Configuring the ViewModelLocator to use SimpleInjector.";
        private readonly string CreatingShell = "Creating the shell";
        private readonly string ConfiguringRegionAdapters = "Configuring region adapters.";
        private readonly string ConfiguringDefaultRegionBehaviors = "Configuring the default region behaviors.";
        private readonly string RegisteringFrameworkExceptionTypes = "Registering Framework Exception Types.";
        private readonly string SettingTheRegionManager = "Setting the RegionManager.";
        private readonly string UpdatingRegions = "Updating regions";
        private readonly string InitializingShell = "Initializing shell";
        private readonly string InitializingModules = "Initializing modules";
        private readonly string BootstrapperSequenceCompleted = "The bootstrap sequence is complete.";
        private readonly string NullModuleCatalogException = "The IModuleCatalog is required and cannot be null in order to initialize the modules";
        private readonly string NullStructureMapContainerException = "The Container is required and cannot be null";
        private readonly string NullLoggerFacadeException = "The ILoggerFacade is required and cannot be null.";
        private readonly string TypeMappingAlreadyRegistered = "Type '{0}' was already registered by the application. Skipping...";

        public override void Run(bool runWithDefaultConfiguration)
        {
            _useDefaultConfiguration = runWithDefaultConfiguration;
            Logger = CreateLogger();
            if (Logger == null)
            {
                throw new InvalidOperationException(NullLoggerFacadeException);
            }

            Logger.Log(LoggerCreatedSuccessfully, Category.Debug, Priority.Low);
            Logger.Log(CreatingModuleCatalog, Category.Debug, Priority.Low);

            ModuleCatalog = CreateModuleCatalog();
            if (ModuleCatalog == null)
            {
                throw new InvalidOperationException(NullModuleCatalogException);
            }

            ConfigureModuleCatalog();

            Logger.Log(CreatingSimpleInjectorContainer, Category.Debug, Priority.Low);
            Container = CreateContainer();
            if(Container == null)
            {
                throw new InvalidOperationException(NullStructureMapContainerException);
            }

            Logger.Log(ConfiguringSimpleInjector, Category.Debug, Priority.Low);
            ConfigureContainer();

            Logger.Log(ConfiguringServiceLocatorSingleton, Category.Debug, Priority.Low);
            ConfigureServiceLocator();

            Logger.Log(ConfiguringViewModelLocator, Category.Debug, Priority.Low);
            ConfigureViewModelLocator();

            Logger.Log(ConfiguringRegionAdapters, Category.Debug, Priority.Low);
            ConfigureRegionAdapterMappings();

            Logger.Log(ConfiguringDefaultRegionBehaviors, Category.Debug, Priority.Low);
            ConfigureDefaultRegionBehaviors();

            Logger.Log(RegisteringFrameworkExceptionTypes, Category.Debug, Priority.Low);
            RegisterFrameworkExceptionTypes();

            Logger.Log(CreatingShell, Category.Debug, Priority.Low);
            Shell = CreateShell();
            if (Shell != null)
            {
                Logger.Log(SettingTheRegionManager, Category.Debug, Priority.Low);
                RegionManager.SetRegionManager(Shell, Container.GetInstance<IRegionManager>());

                Logger.Log(UpdatingRegions, Category.Debug, Priority.Low);
                RegionManager.UpdateRegions();

                Logger.Log(InitializingShell, Category.Debug, Priority.Low);
                InitializeShell();
            }

            if (Container.GetRegistration(typeof(IModuleManager)) != null)
            {
                Logger.Log(InitializingModules, Category.Debug, Priority.Low);
                InitializeModules();
            }

            Logger.Log(BootstrapperSequenceCompleted, Category.Debug, Priority.Low);
        }

        protected override void ConfigureServiceLocator()
        {
            ServiceLocator.SetLocatorProvider(() => Container.GetInstance<IServiceLocator>());
        }

        protected override void ConfigureViewModelLocator()
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory((type) => Container.GetInstance(type));
        }

        protected virtual void ConfigureContainer()
        {
            Container.RegisterSingleton(Logger);
            Container.RegisterSingleton(ModuleCatalog);
            
            if (_useDefaultConfiguration)
            {
                //Container.ResolveUnregisteredType += Container_ResolveUnregisteredType;

                RegisterTypeIfMissing<IServiceLocator, SimpleInjectorServiceLocatorAdapter>(true);
                RegisterTypeIfMissing<IModuleInitializer, ModuleInitializer>(true);
                RegisterTypeIfMissing<IModuleManager, ModuleManager>(true);
                RegisterTypeIfMissing<RegionAdapterMappings, RegionAdapterMappings>(true);
                RegisterTypeIfMissing<IRegionManager, RegionManager>(true);
                RegisterTypeIfMissing<IEventAggregator, EventAggregator>(true);
                RegisterTypeIfMissing<IRegionViewRegistry, RegionViewRegistry>(true);
                RegisterTypeIfMissing<IRegionBehaviorFactory, RegionBehaviorFactory>(true);
                RegisterTypeIfMissing<IRegionNavigationJournalEntry, RegionNavigationJournalEntry>(false);
                RegisterTypeIfMissing<IRegionNavigationJournal, RegionNavigationJournal>(false);
                RegisterTypeIfMissing<IRegionNavigationService, RegionNavigationService>(false);
                RegisterTypeIfMissing<IRegionNavigationContentLoader, RegionNavigationContentLoader>(true);

                //Container.ResolveUnregisteredType -= Container_ResolveUnregisteredType;
            }
        }

        protected virtual Container CreateContainer()
        {
            return new Container();
        }

        protected void RegisterTypeIfMissing<TFrom, TTo>(bool registerAsSingleton)
        {
            RegisterTypeIfMissing(typeof(TFrom), typeof(TTo), registerAsSingleton);
        }

        protected void RegisterTypeIfMissing(Type fromType, Type toType, bool registerAsSingleton)
        {
            
            if (fromType == null)
            {
                throw new ArgumentNullException(nameof(fromType));
            }
            if (toType == null)
            {
                throw new ArgumentNullException(nameof(toType));
            }

            if (Container.GetRegistration(fromType) != null)
            {
                Logger.Log(
                    String.Format(CultureInfo.CurrentCulture, TypeMappingAlreadyRegistered, fromType.Name),
                    Category.Debug, Priority.Low);
            }
            else
            {
                if (registerAsSingleton)
                {
                    Container.RegisterSingleton( () =>fromType, toType);
                }
                else
                {
                    Container.Register(fromType, toType);
                }
            }
        }

        private void Container_ResolveUnregisteredType(object sender, UnregisteredTypeEventArgs e)
        {
            var emptyValidator = Container.GetInstance(e.UnregisteredServiceType);

            // Register the instance as singleton.
            e.Register(() => emptyValidator);
        }
    }
}