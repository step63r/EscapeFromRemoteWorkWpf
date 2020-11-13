using EscapeFromRemoteWorkWpf.Models;
using Prism.Mvvm;

namespace EscapeFromRemoteWorkWpf.ViewModels
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
            var executor = new MouseExecutor();
            _ = executor.Execute();
        }
    }
}
