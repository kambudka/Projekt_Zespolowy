using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlHire
    {
        // zwraca listę wszystkich rezerwacji wykonanych/zaplaconych
        // courtID -> id kortu, którego rezerwacje chcemy
        // courtID = 0 -> wszystkie rezerwacje
        // courtID > 0 -> rezerwacje danego kortu
        // courtID domyslnie jest 0
        // dateFrom -> poczatek przedzialu, z ktorego rezerwacje maja wyswietlone
        //      domyslnie jest null, jak nie poda sie tej daty, to wszystkie od poczatku wezmie
        // dateTo -> koniec przedzialu, z ktorego rezerwacje maja byc wyswietlone
        //      domyslnie jest null, jak nie poda sie tej daty, to wszystkie do aktualnej daty wezmie
        public static List<Hire> GetHires(int courtID = 0, DateTime? dateFrom = null, DateTime? dateTo = null)
        {
            var list = new List<Hire>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    string whereSql = "";
                    if (courtID != 0) whereSql = "CourtID = @CourtID";
                    if(dateFrom !=null)
                    {
                        if (whereSql != "") whereSql += " and ";
                        whereSql += "DateFrom >= @DateFrom";
                    }
                    if (dateTo != null)
                    {
                        if (whereSql != "") whereSql += " and ";
                        whereSql += "DateTo <= @DateTo";
                    }
                    if (whereSql != "") whereSql = " where " + whereSql;
                    
                    var command = new SqlCommand("SELECT * FROM VHire" + whereSql, connection);
                    if(courtID != 0) command.Parameters.AddWithValue("@CourtID", courtID);
                    if (dateFrom != null) command.Parameters.AddWithValue("@DateFrom", dateFrom);
                    if (dateTo != null) command.Parameters.AddWithValue("@DateTo", dateTo);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Hire()
                        {
                            HireID = (int)reader["HireID"],
                            DateFrom = (DateTime)reader["DateFrom"],
                            DateTo = (DateTime)reader["DateTo"],
                            GearID = reader.IsDBNull(reader.GetOrdinal("GearID")) ? (int?)null : (int?)reader["GearID"],
                            GearAmount = reader.IsDBNull(reader.GetOrdinal("GearAmount")) ? (int?)null : (int?)reader["GearAmount"],
                            CourtID = (int)reader["CourtID"],
                            Payment = (decimal)reader["Payment"],
                            UserID = (int)reader["UserID"],
                            DatePayment = (DateTime)reader["DatePayment"],
                            ReservationID = (int)reader["ReservationID"],
                            DateFromAsDate = (DateTime)reader["DateFromAsDate"],
                            DateFromAsTime = (TimeSpan)reader["DateFromAsTime"],
                            DateFromAsMonth = (int)reader["DateFromAsMonth"],
                            DateFromAsDayOfMonth = (int)reader["DateFromAsDayOfMonth"],
                            DateFromAsDayOfWeek = (int)reader["DateFromAsDayOfWeek"],
                            Type = (int)reader["Type"]
                        });
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return list;
        }

        // zwraca konkretną rezerwacje wykonaną/zapłaconą
        // id - id szukanej rezerwacji
        public static Hire GetHire(int id)
        {
            Hire hire = null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT * FROM VHire WHERE HireID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        hire = new Hire()
                        {
                            HireID = (int)reader["HireID"],
                            DateFrom = (DateTime)reader["DateFrom"],
                            DateTo = (DateTime)reader["DateTo"],
                            GearID = reader.IsDBNull(reader.GetOrdinal("GearID")) ? (int?)null : (int?)reader["GearID"],
                            GearAmount = reader.IsDBNull(reader.GetOrdinal("GearAmount")) ? (int?)null : (int?)reader["GearAmount"],
                            CourtID = (int)reader["CourtID"],
                            Payment = (decimal)reader["Payment"],
                            UserID = (int)reader["UserID"],
                            DatePayment = (DateTime)reader["DatePayment"],
                            ReservationID = (int)reader["ReservationID"],
                            DateFromAsDate = (DateTime)reader["DateFromAsDate"],
                            DateFromAsTime = (TimeSpan)reader["DateFromAsTime"],
                            DateFromAsMonth = (int)reader["DateFromAsMonth"],
                            DateFromAsDayOfMonth = (int)reader["DateFromAsDayOfMonth"],
                            DateFromAsDayOfWeek = (int)reader["DateFromAsDayOfWeek"],
                            Type = (int)reader["Type"]
                        };
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return hire;
        }
    }
}