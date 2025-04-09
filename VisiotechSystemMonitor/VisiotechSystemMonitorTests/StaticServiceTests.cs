using Xunit;
using Moq;
using VisiotechSystemMonitorLib.Services;
using VisiotechSystemMonitorLib.Models;
using Microsoft.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace VisiotechSystemMonitor.Tests.Services
{
    public class StaticServiceTests
    {

        // Declaración de la variable _connectionString a nivel de clase
        private readonly string _connectionString = "Server=localhost;Database=VisiotechMonitor_Test;Trusted_Connection=True;TrustServerCertificate=True;";

        // Helper para crear una conexión y comando simulados (útil para pruebas que no acceden a la DB real)
        private (Mock<SqlConnection>, Mock<SqlCommand>) CreateMockConnectionAndCommand()
        {
            var mockConnection = new Mock<SqlConnection>();
            var mockCommand = new Mock<SqlCommand>();

            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
            return (mockConnection, mockCommand);
        }

        [Fact]
        public void Save_InsertsDataIntoSamplesTable()
        {
            // Arrange
            var samplesToSave = new ObservableCollection<SampleModel>
            {
                new SampleModel { TimeStamp = DateTime.Now, ProcessorID = "CPU1", MotherBoardID = "MB1", GpuID = "GPU1", CpuUse = 30.5f, RamUse = 45.2f },
                new SampleModel { TimeStamp = DateTime.Now.AddSeconds(1), ProcessorID = "CPU2", MotherBoardID = "MB2", GpuID = "GPU2", CpuUse = 60.1f, RamUse = 70.8f }
            };

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Obtener el número inicial de registros
            int initialCount = 0;
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Samples", connection))
            {
                initialCount = (int)cmd.ExecuteScalar();
            }

            var staticService = new StaticService(_connectionString); // Pass connection string

            // Act
            staticService.Save(samplesToSave);

            // Assert
            int finalCount = 0;
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Samples", connection))
            {
                finalCount = (int)cmd.ExecuteScalar();
            }

            Assert.Equal(initialCount + samplesToSave.Count, finalCount);

            // Cleanup (optional, depending on your testing strategy)
            foreach (var sample in samplesToSave)
            {
                using (var cmd = new SqlCommand("DELETE FROM Samples WHERE TimeStamp = @TimeStamp", connection))
                {
                    cmd.Parameters.AddWithValue("@TimeStamp", sample.TimeStamp);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [Fact]
        public void Load_RetrievesLatestSampleFromSamplesTable()
        {
            // Arrange
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Insert some test data, ensuring the last one has a distinct TimeStamp
            var now = DateTime.Now;
            var samplesToInsert = new ObservableCollection<SampleModel>
            {
                new SampleModel { TimeStamp = now.AddSeconds(-2), ProcessorID = "OldCPU1", MotherBoardID = "OldMB1", GpuID = "OldGPU1", CpuUse = 10.5f, RamUse = 20.2f },
                new SampleModel { TimeStamp = now.AddSeconds(-1), ProcessorID = "OldCPU2", MotherBoardID = "OldMB2", GpuID = "OldGPU2", CpuUse = 30.1f, RamUse = 40.8f },
                new SampleModel { TimeStamp = now, ProcessorID = "NewCPU", MotherBoardID = "NewMB", GpuID = "NewGPU", CpuUse = 50.7f, RamUse = 60.3f }
            };

            foreach (var sample in samplesToInsert)
            {
                using (var cmd = new SqlCommand("INSERT INTO Samples (TimeStamp, ProcessorID, MotherBoardID, GpuID, CpuUse, RamUse) VALUES (@TimeStamp, @ProcessorID, @MotherBoardID, @GpuID, @CpuUse, @RamUse)", connection))
                {
                    cmd.Parameters.AddWithValue("@TimeStamp", sample.TimeStamp);
                    cmd.Parameters.AddWithValue("@ProcessorID", sample.ProcessorID);
                    cmd.Parameters.AddWithValue("@MotherBoardID", sample.MotherBoardID);
                    cmd.Parameters.AddWithValue("@GpuID", sample.GpuID);
                    cmd.Parameters.AddWithValue("@CpuUse", sample.CpuUse);
                    cmd.Parameters.AddWithValue("@RamUse", sample.RamUse);
                    cmd.ExecuteNonQuery();
                }
            }

            var staticService = new StaticService(_connectionString);

            // Act
            var latestSample = staticService.Load().FirstOrDefault();

            // Assert
            Assert.NotNull(latestSample);
            Assert.Equal(now, latestSample.TimeStamp);
            Assert.Equal("NewCPU", latestSample.ProcessorID);
            Assert.Equal(50.7f, latestSample.CpuUse);
            Assert.Equal(60.3f, latestSample.RamUse);

            // Cleanup
            foreach (var sample in samplesToInsert)
            {
                using (var cmd = new SqlCommand("DELETE FROM Samples WHERE TimeStamp = @TimeStamp", connection))
                {
                    cmd.Parameters.AddWithValue("@TimeStamp", sample.TimeStamp);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        [Fact]
        public void Load_ReturnsEmptyCollectionWhenNoSamplesExist()
        {
            // Arrange
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            // Ensure the table is empty for this test
            using (var cmd = new SqlCommand("DELETE FROM Samples", connection))
            {
                cmd.ExecuteNonQuery();
            }

            var staticService = new StaticService(_connectionString);

            // Act
            var loadedSamples = staticService.Load();

            // Assert
            Assert.Empty(loadedSamples);
        }
    }
}