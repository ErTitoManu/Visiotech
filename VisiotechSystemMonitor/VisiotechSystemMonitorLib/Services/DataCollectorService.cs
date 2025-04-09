using System.Diagnostics;
using VisiotechSystemMonitorLib.Models;
using Visiotech.HardwareInfo;
using VisiotechSystemMonitorLib.Interfaces;
using System.Runtime.InteropServices;


namespace VisiotechSystemMonitorLib.Services
{
    public class DataCollectorService : IDataCollectorService
    {
        public SampleModel Get()
        {
            return new SampleModel
            {
                TimeStamp = DateTime.Now,
                ProcessorID = HardwareInfo.GetProcessorID(),
                MotherBoardID = HardwareInfo.GetMotherboardID(),
                GpuID = HardwareInfo.GetGpuID(),
                CpuUse = GetCpuUsage(),
                RamUse = GetRamUsage()
            };
        }

        private float GetCpuUsage()
        {
            DateTime startTime = DateTime.UtcNow;
            TimeSpan startCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            Thread.Sleep(500);

            DateTime endTime = DateTime.UtcNow;
            TimeSpan endCpuUsage = Process.GetCurrentProcess().TotalProcessorTime;

            double cpuUsedInMs = (endCpuUsage - startCpuUsage).TotalMilliseconds;
            double totalMs = (endTime - startTime).TotalMilliseconds;
            int cpuCores = Environment.ProcessorCount;

            return (float)((cpuUsedInMs / (totalMs * cpuCores)) * 100);
        }

        private float GetRamUsage()
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                return memStatus.dwMemoryLoad;
            }
            return -1; // o lanza una excepción si prefieres
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    }
}

