using System.Windows;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Prism.Unity;
using System.ComponentModel;
using System;

namespace TestAppWpf
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
                    
            Container.AddNewExtension<Interception>();

            Container.RegisterType<ILoadViewModel, LoadViewModel>(
                new Interceptor<VirtualMethodInterceptor>(),
                new InterceptionBehavior<PolicyInjectionBehavior>()
                );
            
            //container.RegisterType<ILoadViewModel, LoadViewModel>(new Interceptor<VirtualMethodInterceptor>(),
            //    new InterceptionBehavior<MetricInterceptionBehavior>());
        }
        
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
            var viewModel = Container.Resolve<ILoadViewModel>();
            Application.Current.MainWindow.DataContext = viewModel;
            ((INotifyPropertyChanged)viewModel).PropertyChanged += Bootstrapper_PropertyChanged;
            viewModel.OnActivated();
        }

        private void Bootstrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("Property change HIT!");
        }
    }    
}
