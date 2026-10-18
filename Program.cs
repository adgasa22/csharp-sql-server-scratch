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

                using (SqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(
                            "SlotId {0} SlotNumber {1} SlotType {2} Status {3} Rate {4}",
                            reader.GetInt32(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetDecimal(4)
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }
        }
    }
}