using System.Threading.Tasks;

using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace TestAppWpf
{
    public interface ILoadViewModel
    {
        bool IsLoading { get; set; }

        DelegateCommand LoadCommand { get; set; }
        Task OnLoad();
    }

    public class LoadViewModel : BindableBase, ILoadViewModel
    {
        private bool _isLoading;

        public LoadViewModel()
        {
            LoadCommand = DelegateCommand.FromAsyncHandler(OnLoad);
        }

        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                if(_isLoading == value)
                    return;
                _isLoading = value;
                OnPropertyChanged(() => IsLoading);
            }
        }

        public DelegateCommand LoadCommand { get; set; }

        [MetricInterception(1)]
        public virtual async Task OnLoad()
        {
            IsLoading = true;

            await Task.Delay(3000);

            IsLoading = false;
        }
    }
}