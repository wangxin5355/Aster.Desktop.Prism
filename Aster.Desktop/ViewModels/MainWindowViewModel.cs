using Prism.Logging;
using Prism.Mvvm;
using Prism.Ioc;
using Unity;
using CommonServiceLocator;
using System.Windows;
using Prism;

namespace Aster.Desktop.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {
            //var Container = ServiceLocator.Current.GetInstance<IUnityContainer>();
            var container = (Application.Current as PrismApplicationBase).Container;
            container.Resolve<ILoggerFacade>().Log("MainViewModel created", Category.Info, Priority.None); 

        }
    }
}
