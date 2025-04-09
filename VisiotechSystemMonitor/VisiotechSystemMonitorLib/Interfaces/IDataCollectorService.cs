using VisiotechSystemMonitorLib.Models;

namespace VisiotechSystemMonitorLib.Interfaces
{
    public interface IDataCollectorService
    {
        SampleModel Get();
    }
}