using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System.ComponentModel;
using System.Threading.Tasks;

namespace TestAppWpf
{
    public interface ILoadViewModel
    {
        Task OnActivated();
        bool IsLoading { get; set; }
    }

    public class LoadViewModel : BindableBase, ILoadViewModel
    {
        private bool _isLoading;

        public LoadViewModel()
        {
            
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading == value) return;
                _isLoading = value;
                OnPropertyChanged(() => IsLoading);
            }
        }

        public DelegateCommand LoadCommand
        {
            get; set;
        }

        [MetricInterception(1)]
        public virtual async Task OnActivated()
        {
            IsLoading = true;

            await Task.Delay(3000);

            IsLoading = false;
        }
    }

    
}
