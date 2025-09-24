using renkimsumatin.Models;
using renkimsumatin.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace renkimsumatin.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly DataService _dataService;
        private Module _activeModule = Module.Scores;

        public MainViewModel()
        {
            _dataService = new DataService();
            StudentsViewModel = new StudentsViewModel(_dataService);
            SubjectsViewModel = new SubjectsViewModel(_dataService);
            AssessmentsViewModel = new AssessmentsViewModel(_dataService);
            ScoresViewModel = new ScoresViewModel(_dataService);
            ReportsViewModel = new ReportsViewModel(_dataService);
        }

        public Module ActiveModule
        {
            get => _activeModule;
            set
            {
                _activeModule = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ModuleTitle));
            }
        }

        public string ModuleTitle => $"{ActiveModule} Management";

        public StudentsViewModel StudentsViewModel { get; }
        public SubjectsViewModel SubjectsViewModel { get; }
        public AssessmentsViewModel AssessmentsViewModel { get; }
        public ScoresViewModel ScoresViewModel { get; }
        public ReportsViewModel ReportsViewModel { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}