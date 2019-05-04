using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlCourt
    {
        // dodanie kortu lub jego modyfikacja
        // CourtID = 0 to dodawanie nowego kortu
        // CourtID > 0 modyfikowanie kortu o tym ID
        public static bool AddModifyCourt(Court court)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.AddCourt", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CourtNumber", court.CourtNumber);
                    command.Parameters.AddWithValue("@SurfaceType", court.SurfaceType);
                    command.Parameters.AddWithValue("@IsForDoubles", court.IsForDoubles);
                    command.Parameters.AddWithValue("@IsCovered", court.IsCovered);
                    command.Parameters.AddWithValue("@PriceH", court.PriceH);
                    command.Parameters.AddWithValue("@Name", court.Name);
                    command.Parameters.AddWithValue("@PriceWinterRatio", court.PriceWinterRatio);
                    command.Parameters.AddWithValue("@PriceWeekendRatio", court.PriceWeekendRatio);
                    command.Parameters.AddWithValue("@CourtID", court.CourtID);
                    command.Parameters["@CourtID"].Direction = ParameterDirection.InputOutput;  // żeby móc wyciągać dane           

                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;
                    try
                    {
                        command.ExecuteNonQuery();
                        court.CourtID = int.Parse(command.Parameters["@CourtID"].Value.ToString());
                        result = true;
                    }
                    catch (Exception ex)
                    {
                    }

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //customer.CustomerID = int.Parse(command.Parameters["@ReturnValue"].Value.ToString());

                    SqlDatabase.CloseConnection(connection);
                }
            }        
            return result;
        }

        // usuwanie kortu
        // sprawdzenie czy nie ma powiązań w innych tabelach jest po stronie bazy
        // przy wywołaniu dać w if'ie, żeby sprawdzić czy na pewno udało się usunąć
        public static bool DeleteCourt(Court court)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.DeleteCourt", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CourtID", court.CourtID);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    try
                    {
                        command.ExecuteNonQuery();
                        court.CourtID = 0;
                        result = true;
                    }
                    catch (Exception ex)
                    {
                    }

                    SqlDatabase.CloseConnection(connection);
                }
            }                
            return result;
        }

        // zwraca listę kortów
        public static List<Court> GetCourts()
        {
            var list = new List<Court>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("select * from dbo.VCourt", connection);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Court()
                        {
                            CourtID = (int)reader["CourtID"],
                            CourtNumber = (int)reader["CourtNumber"],
                            SurfaceType = (string)reader["SurfaceType"],
                            IsForDoubles = (bool)reader["IsForDoubles"],
                            IsCovered = (bool)reader["IsCovered"],
                            PriceH = (decimal)reader["PriceH"],
                            Name = (string)reader["Name"],
                            PriceWinterRatio = (decimal)reader["PriceWinterRatio"],
                            PriceWeekendRatio = (decimal)reader["PriceWeekendRatio"],
                            PriceWinter = (decimal)reader["PriceWinter"],
                            PriceWinterWeekend = (decimal)reader["PriceWinterWeekend"],
                            PriceSummerWeekend = (decimal)reader["PriceSummerWeekend"],
                            PriceSummer = (decimal)reader["PriceSummer"],
                        });
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }       
            return list;
        }

        // zwraca konkretny kort
        public static Court GetCourt(int id)
        {
            Court court = null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT * FROM VCourt WHERE CourtID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        court = new Court()
                        {
                            CourtID = (int)reader["CourtID"],
                            CourtNumber = (int)reader["CourtNumber"],
                            SurfaceType = (string)reader["SurfaceType"],
                            IsForDoubles = (bool)reader["IsForDoubles"],
                            IsCovered = (bool)reader["IsCovered"],
                            PriceH = (decimal)reader["PriceH"],
                            Name = (string)reader["Name"],
                            PriceWinterRatio = (decimal)reader["PriceWinterRatio"],
                            PriceWeekendRatio = (decimal)reader["PriceWeekendRatio"],
                            PriceWinter = (decimal)reader["PriceWinter"],
                            PriceWinterWeekend = (decimal)reader["PriceWinterWeekend"],
                            PriceSummerWeekend = (decimal)reader["PriceSummerWeekend"],
                            PriceSummer = (decimal)reader["PriceSummer"],
                        };
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return court;
        }

        // zwraca cenę konkretnego kortu uwzględniając zniżkę klienta
        // DateStart - data rozpoczęcia wypożyczenia/rezerwacji
        // DateEnd - data zakończenia wypożyczenia/rezerwacji
        // CourtID - id kortu, którego cenę chcemy policzyć (otrzymana cena uwzględnia godziny otwarcia kortów
        // cena w niedzielę dla kortu o id 2 przy cenniku zimowym jest 300, godziny otwarcia w niedzielę np. 12-15, rezerwacja 10-12 da cenę 0, 12-13 da 300
        // UserID - id użytkownika, który wypozycza (pozwala to na uwzględnienie zniżki dla danego klienta)
        // 0 - oznacza ogólną cenę kortu w danym czasie, bez uwzględnienia klienta
        public static decimal GetCourtPrice(DateTime DateStart, DateTime DateEnd, int CourtID, int UserID)
        {
            decimal price = 0;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("select [dbo].[GetPriceRentalCourt] (@DateStart, @DateEnd, @CourtID, @UserID) as Price", connection);
                    command.Parameters.AddWithValue("@DateStart", DateStart);
                    command.Parameters.AddWithValue("@DateEnd", DateEnd);
                    command.Parameters.AddWithValue("@CourtID", CourtID);
                    command.Parameters.AddWithValue("@UserID", UserID);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        price = (decimal)reader["Price"];
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return price;
        }


        // zwraca listę dostepnych kortów
        // courtID > 0 - lista dla wybranego kortu, 0 - wszystkie korty
        // dateFrom - od tej daty szukane terminy
        // dateTo - do tej daty szukane terminy
        public static List<Court> GetAvailableCourts(int courtID, DateTime dateFrom, DateTime dateTo)
        {
            var list = new List<Court>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.GetAvailableCourt", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CourtID", courtID);
                    command.Parameters.AddWithValue("@DateFrom", dateFrom);
                    command.Parameters.AddWithValue("@DateTo", dateTo);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Court()
                        {
                            CourtID = (int)reader["CourtID"],
                            CourtNumber = (int)reader["CourtNumber"],
                            SurfaceType = (string)reader["SurfaceType"],
                            IsForDoubles = (bool)reader["IsForDoubles"],
                            IsCovered = (bool)reader["IsCovered"],
                            PriceH = (decimal)reader["PriceH"],
                            Name = (string)reader["Name"],
                            PriceWinterRatio = (decimal)reader["PriceWinterRatio"],
                            PriceWeekendRatio = (decimal)reader["PriceWeekendRatio"],
                            PriceWinter = (decimal)reader["PriceWinter"],
                            PriceWinterWeekend = (decimal)reader["PriceWinterWeekend"],
                            PriceSummerWeekend = (decimal)reader["PriceSummerWeekend"],
                            PriceSummer = (decimal)reader["PriceSummer"],
                            DateFrom = (DateTime)reader["DateFrom"],
                            DateTo = (DateTime)reader["DateTo"]
                        });
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return list;
        }
    }
}