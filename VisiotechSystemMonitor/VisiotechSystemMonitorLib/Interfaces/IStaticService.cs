using System.Collections.ObjectModel;
using VisiotechSystemMonitorLib.Models;

namespace VisiotechSystemMonitorLib.Interfaces
{
    public interface IStaticService
    {
        void Save(ObservableCollection<SampleModel> data);
        ObservableCollection<SampleModel> Load();
    }
}