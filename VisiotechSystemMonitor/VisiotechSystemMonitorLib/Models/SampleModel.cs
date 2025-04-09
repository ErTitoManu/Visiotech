namespace VisiotechSystemMonitorLib.Models
{
    public class SampleModel
    {
        public DateTime TimeStamp { get; set; }
        public string ProcessorID { get; set; } = string.Empty;
        public string MotherBoardID { get; set; } = string.Empty;
        public string GpuID { get; set; } = string.Empty;
        public float CpuUse { get; set; }
        public float RamUse { get; set; }
    }
}
