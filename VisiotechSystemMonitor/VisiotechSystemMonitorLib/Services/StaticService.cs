using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using VisiotechSystemMonitorLib.Interfaces;
using VisiotechSystemMonitorLib.Models;

namespace VisiotechSystemMonitorLib.Services
{
    public class StaticService : IStaticService
    {
        private readonly string _connectionString;
        public StaticService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Save(ObservableCollection<SampleModel> data)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            foreach (var item in data)
            {
                var cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO Samples (TimeStamp, ProcessorID, MotherBoardID, GpuID, CpuUse, RamUse) VALUES (@TimeStamp, @ProcessorID, @MotherBoardID, @GpuID, @CpuUse, @RamUse)";

                cmd.Parameters.AddWithValue("@TimeStamp", item.TimeStamp);
                cmd.Parameters.AddWithValue("@ProcessorID", item.ProcessorID);
                cmd.Parameters.AddWithValue("@MotherBoardID", item.MotherBoardID);
                cmd.Parameters.AddWithValue("@GpuID", item.GpuID);
                cmd.Parameters.AddWithValue("@CpuUse", item.CpuUse);
                cmd.Parameters.AddWithValue("@RamUse", item.RamUse);

                cmd.ExecuteNonQuery();
            }
        }

        public ObservableCollection<SampleModel> Load()
        {
            var result = new ObservableCollection<SampleModel>();

            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT TimeStamp, ProcessorID, MotherBoardID, GpuID, CpuUse, RamUse FROM Samples WHERE id = (SELECT MAX(id) FROM Samples);";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new SampleModel
                {
                    TimeStamp = reader.GetDateTime(0),
                    ProcessorID = reader.GetString(1),
                    MotherBoardID = reader.GetString(2),
                    GpuID = reader.GetString(3),
                    CpuUse = (float)reader.GetDouble(4),
                    RamUse = (float)reader.GetDouble(5),
                });
            }

            return result;
        }
    }
}
