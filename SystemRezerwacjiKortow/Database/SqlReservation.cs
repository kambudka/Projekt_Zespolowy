using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlReservation
    {
        // sprawdzanie zajętości kortu w danym dniu lub godzinie
        // courtID - id kortu, którego zajętość ma być sprawdzona
        // testDate - data, której zajętość ma być sprawdzona
        // testHour - godzina, której zajętość ma być sprawdzona
        // jeśli testHour zosatnie podane 0, to będzie sprawdzana zajętość całego dnia, a nie konkretnej godziny
        // zwraca: 0 - całkowicie wolny
        //         1 - całkowicie zajęty
        //         2 - częściowo zajęty
        //         3 - turniej
        //         4 - nieczynne
        public static int GetReservationStateCourt(int courtID, DateTime testDate, int testHour)
        {
            int result = -1;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.GetStateReservationCourt", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CourtID", courtID);
                    command.Parameters.AddWithValue("@Date", testDate);
                    command.Parameters.AddWithValue("@Hour", testHour);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    result = int.Parse(command.Parameters["@ReturnValue"].Value.ToString());

                    SqlDatabase.CloseConnection(connection);
                }
            }
            return result;
        }

        // składanie rezerwacji (zwykłej) na kort
        // zwraca true, jeśli rezerwacja się powiodła lub false w przypadku niepowodzenia
        // courtID - id kortu, ktory ma byc zarezerwowany
        // dateTimeFrom - data i godzina rozpoczecia rezerwacji
        // dateTimeTo - data i godzina zakonczenia rezerwacji
        // userID - id uzytkownika skladajacego rezerwacje
        // rezerwacja jest mozliwa tylko, gdy w tym czasie nie ma innych rezerwacji (sprawdzane na poziomie bazy)
        //    oraz gdy rozpoczyna sie i konczy w czasie godzin otwarcia kompleksu
        public static bool SetReservationCourt(int courtID, DateTime dateTimeFrom, DateTime dateTimeTo, int userID)
        {
            bool result = false;
            int reservationID = 0;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.SetReservationCourt", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CourtID", courtID);
                    command.Parameters.AddWithValue("@DateFrom", dateTimeFrom);
                    command.Parameters.AddWithValue("@DateTo", dateTimeTo);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@ReservationID", reservationID);
                    command.Parameters["@ReservationID"].Direction = ParameterDirection.Output;  // żeby móc wyciągać dane
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
                        reservationID = int.Parse(command.Parameters["@ReservationID"].Value.ToString());
                        result = reservationID > 0;
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

        // zwraca listę wszystkich rezerwacji
        // userID -> id użytkownika, który wyświetla listę
        // dla zwykłego usera wynik to lista jego rezerwacji
        // dla admina lista wszystkich rezerwacji
        public static List<Reservation> GetReservations(int userID)
        {
            var list = new List<Reservation>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.GetReservations", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = SqlDatabase.Timeout;
                    command.Parameters.AddWithValue("@UserID", userID);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Reservation()
                        {
                            ReservationID = (int)reader["ReservationID"],
                            UserID = (int)reader["UserID"],                           
                            DateFrom = (DateTime)reader["DateFrom"],
                            DateTo = (DateTime)reader["DateTo"],
                            Payment = (decimal)reader["Payment"],
                            GearID = (int)reader["GearID"],                            
                            CourtID = (int)reader["CourtID"],
                            DateOfInsert = (DateTime)reader["DateOfInsert"],
                            //DateOfCancel = (DateTime?)reader["DateCancel"],
                            DateOfCancel = reader.IsDBNull(reader.GetOrdinal("DateCancel")) ? (DateTime?)null : (DateTime?)reader["DateCancel"],
                            IsExecuted = (bool)reader["IsExecuted"],
                            CyclicReservationID = (int)reader["CyclicReservationID"],
                            ContestID = (int)reader["ContestID"],
                            ReservationName = (string)reader["ReservationName"],
                            UserName = (string)reader["UserName"],
                            Amount = (int)reader["Amount"],
                            ContestName = (string)reader["ContestName"],
                            IsAccepted = (bool)reader["IsAccepted"]
                        });
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return list;
        }

        // akceptacja/anulowanie akceptacji rezerwacji
        // zwraca true, jeśli akceptacja/anulowanie się powiodło lub false w przypadku niepowodzenia
        // reservationID - id rezerwacji, ktorej status ma byc zmieniony
        // Accept - true -> dana rezerwacja ma byc zaakceptowana
        //          false -> dana rezerwacja ma by niezaakceptowana
        // akceptowania/anulowanie akceptacji jest mozliwe tylko gdy rezerwacja nie jest wykonana (IsExecuted jest false) -> sprawdzane na poziomie bazy
        // UWAGA! 
        // akceptacja/anulowanie akceptacji rezerwacji bedacej czescia rezerwacji cyklicznej lub turniejowej
        // powoduje akceptacje/anulowanie akceptacji dla wszystkich rezerwacji nalezacych do danej rezerwacji cyklicznej/turniejowej
        // przyklad: 
        // rezerwacja cykliczna od 11.03.2019 do 24.03.2019 co 2 dni, akceptujemy jedną z rezerwacji zwykłych - 13.03.2019
        // pozostałe rezerwacje zwykłe w podanym przedziałe również zostaną zaakceptowane
        public static bool AcceptReservation(int reservationID, bool Accept)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.AcceptReservation", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReservationID", reservationID);
                    if(Accept) command.Parameters.AddWithValue("@Accept", 1);
                    else command.Parameters.AddWithValue("@Accept", 0);
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

        // anulowanie rezerwacji
        // zwraca true, jeśli anulowanie się powiodło lub false w przypadku niepowodzenia
        // reservationID - rezerwacja, ktora ma byc anulowana
        // userID - id uzytkownika, ktory anuluje rezeracje
        //          zwykly user moze anulowac tylko swoje, administrator wszystkie
        // rezerwacje mozna anulowac tylko w momencie gdy nie jest wykonana (IsExecuted jest false) - sprawdzane na poziomie bazy
        // nie mozna przywrocic anulowanej rezerwacji
        public static bool CancelReservation(int reservationID, int userID)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.CancelReservation", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ReservationID", reservationID);
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

        // zwraca konkretną rezerwacje
        // id - id szukanej rezerwacji
        public static Reservation GetReservation(int id)
        {
            Reservation reservation = null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT * FROM VReservation WHERE ReservationID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        reservation = new Reservation()
                        {
                            ReservationID = (int)reader["ReservationID"],
                            UserID = (int)reader["UserID"],
                            DateFrom = (DateTime)reader["DateFrom"],
                            DateTo = (DateTime)reader["DateTo"],
                            Payment = (decimal)reader["Payment"],
                            GearID = (int)reader["GearID"],
                            CourtID = (int)reader["CourtID"],
                            DateOfInsert = (DateTime)reader["DateOfInsert"],
                            //DateOfCancel = (DateTime?)reader["DateCancel"],
                            DateOfCancel = reader.IsDBNull(reader.GetOrdinal("DateCancel")) ? (DateTime?)null : (DateTime?)reader["DateCancel"],
                            IsExecuted = (bool)reader["IsExecuted"],
                            CyclicReservationID = (int)reader["CyclicReservationID"],
                            ContestID = (int)reader["ContestID"],
                            ReservationName = (string)reader["ReservationName"],
                            UserName = (string)reader["UserName"],
                            Amount = (int)reader["Amount"],
                            ContestName = (string)reader["ContestName"],
                            IsAccepted = (bool)reader["IsAccepted"]
                        };
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return reservation;
        }

        // zwraca id rezerwacji > 0, jesli rezerwacja istnieje, w innym przypadku zwraca 0 lub -1 jesli blad wykonania procedury
        // courtID - identyfikator kortu
        // testDate - data i godzina sprawdzanej rezerwacji
        public static int GetReservationIDCourt(int courtID, DateTime testDate)
        {
            int result = -1;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.GetReservationIDCourt", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CourtID", courtID);
                    command.Parameters.AddWithValue("@Date", testDate);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    result = int.Parse(command.Parameters["@ReturnValue"].Value.ToString());

                    SqlDatabase.CloseConnection(connection);
                }
            }
            return result;
        }
    }
}