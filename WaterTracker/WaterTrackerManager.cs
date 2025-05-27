using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;
using static WaterTracker.Utility;

namespace WaterTracker
{
    public class WaterTrackerManager
    {
        private readonly string connectionString = @"Data Source=WaterTracker.db";

        public void InitializeDataBase()
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();



                tableCmd.CommandText =
                 @"CREATE TABLE IF NOT EXISTS drinking_water(
                     Id INTEGER PRIMARY KEY  AUTOINCREMENT,
                     Date TEXT,
                     Quantity INTEGER
                     )";
                tableCmd.ExecuteNonQuery();
                connection.Close();
            }
        }
        public void GetUserInput()
        {
            List<string> options = new List<string> { "Close the Application", "View All Records", "Insert Record", "Delete Record", "Update Record" };
            Console.Clear();
            bool closeApp = false;
            while (closeApp == false)
            {
                Print("====== Main Menu ======");
                Print("\nWhat would you like to do?\n");

                int index = 0;
                foreach (string option in options)
                {
                    Print($"({index}) {option}");
                    index++;
                }
                Print("\n=====================\n");

                string commandInput = Console.ReadLine();

                switch (ConvertToInteger(commandInput))
                {
                    case 0:
                        Print("Goodbye!!!");
                        closeApp = true;
                        Environment.Exit(2000);
                        break;
                    case 1:
                        GetAllRecords();
                        break;
                    case 2:
                        Insert();
                        break;
                    case 3:
                        Delete();
                        break;
                    case 4:
                        Update();
                        break;
                    default:
                        Print("Invalid Command. Please type a number 0 - 4");
                        break;
                }
            }
        }

        private void GetAllRecords()
        {
            Console.Clear();

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"SELECT * FROM drinking_water";

                List<DrinkingWater> tableData = new List<DrinkingWater>();

                SqliteDataReader reader = tableCmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        tableData.Add(
                            new DrinkingWater
                            {
                                Id = reader.GetInt32(0),
                                Date = DateTime.ParseExact(reader.GetString(1), "dd-MM-yy", new CultureInfo("en-US")),
                                Quantity = reader.GetInt32(2)
                            });
                    }
                }
                else
                {
                    Print("No rows found");
                }

                connection.Close();

                Print("-------------------\n");

                foreach (var row in tableData)
                {
                    Print($"{row.Id} - {row.Date.ToString("dd-MMM-yyyy")} - Quantity: {row.Quantity}");
                }

                Print("-------------------\n");

            }
        }
        private void Insert()
        {
            string date = GetDateInput();
            Print("\n\nPlease insert the number of glasses of water your drank\n");
            int quantity = ConvertToInteger(Console.ReadLine());

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"INSERT INTO drinking_water(date, quantity) VALUES('{date}',  {quantity})";
                tableCmd.ExecuteNonQuery();

                connection.Close();
            }

            GetUserInput();

        }
        private void Update()
        {
            Console.Clear();
            GetAllRecords();

            Print("\nPlease type Id of the record you would like to update ");
            int recordId = ConvertToInteger(Console.ReadLine());

            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                var checkCmd = connection.CreateCommand();
                checkCmd.CommandText = $"SELECT EXISTS(SELECT 1 FROM drinking_water WHERE Id = {recordId})";
                int checkQuery = ConvertToInteger(Convert.ToString(checkCmd.ExecuteScalar()));

                if (checkQuery == 0)
                {
                    Print($"\nRecord with {recordId} doesn't exists.\n");
                    connection.Close();
                    Update();
                }


                string date = GetDateInput();

                Print("\nPlease insert the number of glasses of water you Actually drank that day");
                int quantity = ConvertToInteger(Console.ReadLine());

                var tableCmd = connection.CreateCommand();
                tableCmd.CommandText = $"UPDATE drinking_water SET date = '{date}', quantity = '{quantity}' WHERE Id = {recordId}";

                tableCmd.ExecuteNonQuery();
                connection.Close();
            }

            GetUserInput();

        }
        private string GetDateInput()
        {
            Print("\n\nPlease insert the date: (Format: dd-mm-yy). Type 0 to return to the main menu.");

            string dateInput = Console.ReadLine();

            if (dateInput == "0") GetUserInput();

            while (!DateTime.TryParseExact(dateInput, "dd-MM-yy", new CultureInfo("en-US"), DateTimeStyles.None, out _))
            {
                Print("\nInvalid date. (Format: dd-mm-yy). Type 0 to return to main menu");
                dateInput = Console.ReadLine();
            }

            return dateInput;
        }
        private void Delete()
        {
            Console.Clear();
            GetAllRecords();

            Print("Please type the Id of the record you want to delete");
            int recordId = ConvertToInteger(Console.ReadLine());


            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                var tableCmd = connection.CreateCommand();

                tableCmd.CommandText = $"DELETE from drinking_water WHERE Id= '{recordId}'";
                int rowCount = tableCmd.ExecuteNonQuery();

                if (rowCount == 0)
                {
                    Console.WriteLine($"\nRecord with Id {recordId} doesn't exist.\n");
                    Delete();
                }

                connection.Close();
                Print($"\nRecord with Id {recordId} was deleted\n");
                GetUserInput();
            }

        }
    }
}