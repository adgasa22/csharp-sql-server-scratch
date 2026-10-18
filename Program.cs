using System;
using Dapper;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main()
    {
        string connectionString =
            "Server=localhost,1433;Database=ParkingInfoSystem;User Id=sa;" +
            "Password=YourPassword;TrustServerCertificate=True;";


        try
        {
            using (Db db = new Db(connectionString))
            {

                string sql = @"
                    SELECT SlotId, SlotNumber, SlotType, Status, HourlyRate
                    FROM dbo.ParkingSlots;";

                SqlCommand query = new SqlCommand(sql, db.Connection);

                var parkings = db.Connection.Query<Parking>(
                @"SELECT SlotId, SlotNumber, SlotType, Status, HourlyRate
                  FROM dbo.ParkingSlots;"
                ).ToList();

                foreach (var p in parkings)
                {
                    Console.WriteLine(
                        "SlotId {0} SlotNumber {1} SlotType {2} Status {3} Rate {4}",
                        p.SlotId, p.SlotNumber, p.SlotType, p.Status, p.HourlyRate
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