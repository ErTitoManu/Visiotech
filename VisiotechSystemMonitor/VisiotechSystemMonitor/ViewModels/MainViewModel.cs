using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using VisiotechSystemMonitorLib.Models;
using VisiotechSystemMonitorLib.Services;
using VisiotechSystemMonitorLib.Interfaces;
using VisiotechSystemMonitor.Helper;

namespace VisiotechSystemMonitor.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly System.Timers.Timer _timer;
        private readonly IDataCollectorService _dataCollector;
        private readonly IStaticService _static;
        private int _intervalMilliseconds = 500;
        public bool _isRunning = false;
        public ObservableCollection<SampleModel> Samples { get; set; }
        public RelayCommand<object?> StartStopCommand { get; }

        public int IntervalMilliseconds
        {
            get => _intervalMilliseconds;
            set
            {
                _intervalMilliseconds = value;
                OnPropertyChanged();
                IsIntervalValid(_intervalMilliseconds);
                UpdateTimer();
            }
        }

        public static bool IsIntervalValid(int intervalMilliseconds)
        {
            return intervalMilliseconds >= 500 && intervalMilliseconds <= 10000;
        }

        public MainViewModel(IDataCollectorService dataCollector, IStaticService staticService)
        {
            _dataCollector = dataCollector;
            _static = staticService;
            Samples = new ObservableCollection<SampleModel>(_static.Load());

            _timer = new System.Timers.Timer(_intervalMilliseconds);
            _timer.Elapsed += OnTimerElapsed;

            StartStopCommand = new RelayCommand<object?>(__ => Toggle());
        }

        private void Toggle()
        {
            _isRunning = !_isRunning;
            _timer.Enabled = _isRunning;
        }

        private void UpdateTimer()
        {
            _timer.Interval = Math.Clamp(_intervalMilliseconds, 500, 10000);
        }

        public void OnTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            SampleModel data = _dataCollector.Get();
            System.Windows.Application.Current.Dispatcher.Invoke(() => Samples.Add(data));
            _static.Save(Samples);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}