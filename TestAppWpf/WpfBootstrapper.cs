using System;
using System.ComponentModel;
using System.Windows;

using Prism.SimpleInjector;

namespace TestAppWpf
{
    public class WpfBootstrapper : SimpleInjectorBootstrapper
    {
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
        }

        protected override DependencyObject CreateShell()
        {
            return Container.GetInstance<MainWindow>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
            var viewModel = Container.GetInstance<ILoadViewModel>();
            Application.Current.MainWindow.DataContext = viewModel;
            ((INotifyPropertyChanged)viewModel).PropertyChanged += Bootstrapper_PropertyChanged;
        }

        private void Bootstrapper_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine("Property change HIT!");
        }
    }
}