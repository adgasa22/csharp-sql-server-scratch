using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString =
            "Server=localhost,1433;Database=ParkingInfoSystem;User Id=sa;" +
            "Password=YourPassword;TrustServerCertificate=True;";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                string sql = @"
                    SELECT SlotId, SlotNumber, SlotType, Status, HourlyRate
                    FROM dbo.ParkingSlots;";

                SqlCommand query = new SqlCommand(sql, connection);

                List<Parking> parkings = new();

                using (SqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        parkings.Add(new Parking
                        {
                            SlotId = reader.GetInt32(0),
                            SlotNumber = reader.GetString(1),
                            SlotType = reader.GetString(2),
                            Status = reader.GetString(3),
                            HourlyRate = reader.GetDecimal(4)
                        });
                    }
                }

                foreach (var p in parkings)
                {
                    Console.WriteLine(
                        "SlotId {0} SlotNumber {1} SlotType {2} Status {3} Rate {4}",
                        p.SlotId, p.SlotNumber, p.SlotType, p.Status, p.HourlyRate
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }
    }
}