using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlContest
    {
        // składanie rezerwacji turniejowej
        // rezerwacja wszystkich kortow w zadanym okresie(cale dnie)
        // zwraca true, jeśli rezerwacja się powiodła lub false w przypadku niepowodzenia
        // name - nazwa turnieju, max 50znakow
        // organizer - organizator turnieju, max 200znakow
        // description - opis turnieju, max 200znaków
        // dateStart - poczatek turnieju
        // dateStop - koniec turnieju (wlacznie)
        // userID - id uzytkownika skladajacego rezerwacje
        // rezerwacja jest mozliwa tylko, gdy w tym czasie nie ma innych rezerwacji na wszystkich kortach (sprawdzane na poziomie bazy)
        public static bool SetReservationContest(string name, string organizer, string description, DateTime dateStart, DateTime dateStop, int userID)
        {
            bool result = false;
            int contestID = 0;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    if (description.Length > 200) description = description.Substring(0, 200);
                    if (organizer.Length > 200) organizer = organizer.Substring(0, 200);
                    if (name.Length > 200) name = name.Substring(0, 200);
                    var command = new SqlCommand("dbo.SetReservationContest", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Name", name);
                    command.Parameters.AddWithValue("@Organizer", organizer);
                    command.Parameters.AddWithValue("@Description", description);
                    command.Parameters.AddWithValue("@DateStart", dateStart);
                    command.Parameters.AddWithValue("@DateStop", dateStop);
                    command.Parameters.AddWithValue("@UserID", userID);
                    command.Parameters.AddWithValue("@ContestID", contestID);
                    command.Parameters["@ContestID"].Direction = ParameterDirection.InputOutput;  // żeby móc wyciągać dane
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
                        contestID = int.Parse(command.Parameters["@ContestID"].Value.ToString());
                        result = contestID > 0;
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

        // zwraca listę wszystkich turniejow
        // userID -> id użytkownika, który wyświetla listę
        // dla zwykłego usera wynik to lista jego turniejow
        // dla admina lista wszystkich turniejow
        public static List<Contest> GetContests(int userID)
        {
            var list = new List<Contest>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.GetReservationsContest", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = SqlDatabase.Timeout;
                    command.Parameters.AddWithValue("@UserID", userID);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Contest()
                        {
                            ContestID = (int)reader["ContestID"],
                            Name = (string)reader["Name"],
                            DateFrom = (DateTime)reader["DateFrom"],
                            DateTo = (DateTime)reader["DateTo"],
                            Description = (string)reader["Description"],
                            Organizer = (string)reader["Organizer"],
                            UserID = (int)reader["UserID"],
                            DateCancel = reader.IsDBNull(reader.GetOrdinal("DateCancel")) ? (DateTime?)null : (DateTime?)reader["DateCancel"],
                            PaymentToPay = (decimal)reader["PaymentToPay"],
                            UserName = (string)reader["UserName"]                     
                        });
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return list;
        }
        // anulowanie turnieju
        // zwraca true, jeśli anulowanie się powiodło lub false w przypadku niepowodzenia
        // contestID - turniej, ktora ma byc anulowany
        // userID - id uzytkownika, ktory anuluje turniej
        //          zwykly user moze anulowac tylko swoje, administrator wszystkie
        // anulowanie turnieju anuluje wszystkie należące do niego rezerwacje
        // nie mozna przywrocic anulowanego turnieju
        public static bool CancelContest(int contestID, int userID)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.CancelReservationContest", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ContestID", contestID);
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

        // zwraca konkretny turniej
        // id - id szukanego turnieju
        public static Contest GetContest(int id)
        {
            Contest contest = null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT * FROM VContest WHERE ContestID = @id", connection);
                    command.Parameters.AddWithValue("@id", id);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        contest = new Contest()
                        {
                            ContestID = (int)reader["ContestID"],
                            Name = (string)reader["Name"],
                            DateFrom = (DateTime)reader["DateFrom"],
                            DateTo = (DateTime)reader["DateTo"],
                            Description = (string)reader["Description"],
                            Organizer = (string)reader["Organizer"],
                            UserID = (int)reader["UserID"],
                            DateCancel = reader.IsDBNull(reader.GetOrdinal("DateCancel")) ? (DateTime?)null : (DateTime?)reader["DateCancel"],
                            PaymentToPay = (decimal)reader["PaymentToPay"],
                            UserName = (string)reader["UserName"]
                        };
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return contest;
        }

        // zaplata za caly turniej - wykonanie wszystkich rezerwacji nalezacych do turnieju (do archiwum)
        // zwraca true, jeśli zaplata się powiodła lub false w przypadku niepowodzenia
        // contestID - turniej, ktory ma byc zaplacony/wykonany
        // rezerwacje turnieju mozna zaplacic tylko w momencie gdy nie jest wykonana (IsExecuted jest false) i nie jest anulowana - sprawdzane na poziomie bazy
        // nie mozna anulowac wykonania/zaplacenia rezerwacji
        public static bool MakePaymentContest(int contestID)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.MakePaymentContest", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ContestID", contestID);
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

    }
}