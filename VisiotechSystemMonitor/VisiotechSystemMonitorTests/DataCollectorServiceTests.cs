using Xunit;
using VisiotechSystemMonitorLib.Services;
using VisiotechSystemMonitorLib.Models;
using System;
using System.Diagnostics;
using System.Threading;
using Moq;

namespace VisiotechSystemMonitor.Tests.Services
{
    public class DataCollectorServiceTests
    {
        [Fact]
        public void Get_ReturnsSampleModelWithCurrentData()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.NotNull(sample);
            Assert.NotEqual(default(DateTime), sample.TimeStamp);
            Assert.NotNull(sample.ProcessorID);
            Assert.NotNull(sample.MotherBoardID);
            Assert.NotNull(sample.GpuID);
            Assert.True(sample.CpuUse >= 0 && sample.CpuUse <= 100);
            Assert.True(sample.RamUse >= 0 && sample.RamUse <= 100);
        }

        [Fact]
        public void Get_CpuUsageIsWithinValidRange()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.True(sample.CpuUse >= 0 && sample.CpuUse <= 100);
        }

        [Fact]
        public void Get_RamUsageIsWithinValidRangeOrNegativeOneOnError()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.True(sample.RamUse >= 0 && sample.RamUse <= 100 || sample.RamUse == -1);
        }

        [Fact]
        public void Get_ReturnsNonNullSampleModel()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.NotNull(sample);
        }

        [Fact]
        public void Get_TimeStampIsCurrent()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();
            var startTime = DateTime.Now;

            // Act
            var sample = dataCollectorService.Get();
            var endTime = DateTime.Now;

            // Assert
            Assert.True(sample.TimeStamp >= startTime && sample.TimeStamp <= endTime.AddMilliseconds(100)); // Allow for slight time difference
        }

        [Fact]
        public void Get_ProcessorIDIsNotNullOrEmpty()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.False(string.IsNullOrEmpty(sample.ProcessorID));
        }

        [Fact]
        public void Get_MotherBoardIDIsNotNullOrEmpty()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.False(string.IsNullOrEmpty(sample.MotherBoardID));
        }

        [Fact]
        public void Get_GpuIDIsNotNullOrEmpty()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.False(string.IsNullOrEmpty(sample.GpuID));
        }

        [Fact]
        public void Get_CpuUseIsNonNegative()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.True(sample.CpuUse >= 0);
        }

        [Fact]
        public void Get_RamUseIsNonNegativeOrNegativeOne()
        {
            // Arrange
            var dataCollectorService = new DataCollectorService();

            // Act
            var sample = dataCollectorService.Get();

            // Assert
            Assert.True(sample.RamUse >= 0 || sample.RamUse == -1);
        }
    }
}