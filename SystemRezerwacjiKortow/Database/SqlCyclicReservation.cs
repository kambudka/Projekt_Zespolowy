using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlCyclicReservation
    {
        // składanie rezerwacji cyklicznej na kort
        // zwraca true, jeśli rezerwacja się powiodła lub false w przypadku niepowodzenia
        // courtID - id kortu, ktory ma byc zarezerwowany
        // descrpiton - opis rezerwacji, max 50znaków
        // dateTimeFrom - data i godzina pierwszej rezerwacji
        // dateTimeTo - data i godzina ostatniej rezerwacji
        // userID - id uzytkownika skladajacego rezerwacje
        // hoursAmount - na ile godzin ma byc rezerwacja
        // type - 0 -> odnawiana w dany dzien tygodnia
        //        1 -> odnawiana w dany dzien miesiaca
        //        2 -> odnawiana co interval dni, wtedy interval musi byc wiekszy od 0
        // interval - co ile dni rezerwacja ma byc odnowiona, tylko dla typu 2
        // rezerwacja jest mozliwa tylko, gdy w tym czasie nie ma innych rezerwacji (sprawdzane na poziomie bazy)
        //    oraz gdy rozpoczyna sie i konczy w czasie godzin otwarcia kompleksu
        public static bool SetReservationCourtCyclic(int courtID, string description, DateTime dateTimeFrom, DateTime dateTimeTo, int userID,
            int hoursAmount, int type, int interval)
        {
            bool result = false;
            int cyclicReservationID = 0;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    if (description.Length > 50) description = description.Substring(0, 50);
                    var command = new SqlCommand("dbo.SetReservationCourtCyclic", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CourtID", courtID);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@DateStart", dateTimeFrom);
                    command.Parameters.AddWithValue("@DateStop", dateTimeTo);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@HoursAmount", hoursAmount);
                    command.Parameters.AddWithValue("@Type", type);
                    command.Parameters.AddWithValue("@Interval", interval);
                    command.Parameters.AddWithValue("@CyclicReservationID", cyclicReservationID);
                    command.Parameters["@CyclicReservationID"].Direction = ParameterDirection.Output;  // żeby móc wyciągać dane
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
                        cyclicReservationID = int.Parse(command.Parameters["@CyclicReservationID"].Value.ToString());
                        result = cyclicReservationID > 0;
                    }
                    catch (Exception ex)
                    {
                    }

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //result = int.Parse(command.Parameters["@ReturnValue"].Value.ToString());
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return result;
        }

        // zwraca listę wszystkich rezerwacji cyklicznych
        // userID -> id użytkownika, który wyświetla listę
        // dla zwykłego usera wynik to lista jego rezerwacji
        // dla admina lista wszystkich rezerwacji
        public static List<CyclicReservation> GetReservationsCyclic(int userID)
        {
            var list = new List<CyclicReservation>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.GetReservationsCyclic", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = SqlDatabase.Timeout;
                    command.Parameters.AddWithValue("@UserID", userID);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new CyclicReservation()
                        {
                            CyclicReservationID = (int)reader["CyclicReservationID"],
                            Description = (string)reader["Description"],
                            DayOfWeek = (int)reader["DayOfWeek"],
                            DayOfMonth = (int)reader["DayOfMonth"],
                            DayInterval = (int)reader["DayInterval"],
                            Time = (TimeSpan)reader["Time"],
                            DateStart = (DateTime)reader["DateStart"],
                            DateStop = (DateTime)reader["DateStop"],
                            UserID = (int)reader["UserID"],
                            GearID = (int)reader["GearID"],
                            CourtID = (int)reader["CourtID"],
                            DateCancel = reader.IsDBNull(reader.GetOrdinal("DateCancel")) ? (DateTime?)null : (DateTime?)reader["DateCancel"],
                            Amount = (int)reader["Amount"],
                            UserName = (string)reader["UserName"],
                            ReservationName = (string)reader["ReservationName"]
                        });
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return list;
        }

        // anulowanie rezerwacji cyklicznej
        // zwraca true, jeśli anulowanie się powiodło lub false w przypadku niepowodzenia
        // cyclicReservationID - rezerwacja, ktora ma byc anulowana
        // userID - id uzytkownika, ktory anuluje rezeracje
        //          zwykly user moze anulowac tylko swoje, administrator wszystkie
        // anulowanie rezerwacji cyklicznej anuluje wszystkie należące do niej rezerwacje
        // nie mozna przywrocic anulowanej rezerwacji
        public static bool CancelReservationCyclic(int cyclicReservationID, int userID)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.CancelReservationCyclic", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CyclicReservationID", cyclicReservationID);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
                        // użyć jeżeli chcemy wykorzystać wartość return z procedury
                        result = int.Parse(command.Parameters["@ReturnValue"].Value.ToString()) == 1;
                    }
                    catch (Exception ex)
                    {
                    }

                    SqlDatabase.CloseConnection(connection);
                }
            }
            return result;
        }


        // zwraca konkretną rezerwację cykliczną
        // id - id szukanej rezerwacji
        public static CyclicReservation GetReservationCyclic(int id)
        {
            CyclicReservation cyclicReservation = null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT * FROM VReservationCyclic WHERE CyclicReservationID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cyclicReservation = new CyclicReservation()
                        {
                            CyclicReservationID = (int)reader["CyclicReservationID"],
                            Description = (string)reader["Description"],
                            DayOfWeek = (int)reader["DayOfWeek"],
                            DayOfMonth = (int)reader["DayOfMonth"],
                            DayInterval = (int)reader["DayInterval"],
                            Time = (TimeSpan)reader["Time"],
                            DateStart = (DateTime)reader["DateStart"],
                            DateStop = (DateTime)reader["DateStop"],
                            UserID = (int)reader["UserID"],
                            GearID = (int)reader["GearID"],
                            CourtID = (int)reader["CourtID"],
                            DateCancel = reader.IsDBNull(reader.GetOrdinal("DateCancel")) ? (DateTime?)null : (DateTime?)reader["DateCancel"],
                            Amount = (int)reader["Amount"],
                            UserName = (string)reader["UserName"],
                            ReservationName = (string)reader["ReservationName"]
                        };
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return cyclicReservation;
        }
    }
}