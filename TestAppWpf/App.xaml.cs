using System.Windows;

namespace TestAppWpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //var bootstrapper = new Bootstrapper();
            var bootstrapper = new WpfBootstrapper();
            bootstrapper.Run();
        }
    }
}
