using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlUser
    {
        // dodanie użytkownika albo aktualizacja jego imienia, nazwiska, daty urodzenia, jezli ten uzytkownik juz istnieje
        // przy wywolywaniu funckji w rejestracji 
        // pod user.RoleID trzeba podstawić SqlDatabase.UserRoleID
        // pod user.CustomerID trzeba podstawić SqlDatabase.CustomerAtr
        public static void InsertUser(User user)
        {
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    if (user.UserID == 0)
                    {
                        var command = new SqlCommand
                        ("INSERT INTO [User] (FirstName, Surname, Email, DateOfBirth, Password, IsEmailVeryfied, RoleID, CustomerID, ActivationCode) " +
                        "VALUES (@name, @surname, @email, @birth, @password, @veryfied, @role, @customer, @code) " +
                        "SELECT @@IDENTITY as UserID"
                        , connection);
                        command.Parameters.AddWithValue("@name", user.FirstName);
                        command.Parameters.AddWithValue("@surname", user.Surname);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@birth", user.DateOfBirth);
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@veryfied", user.IsEmailVeryfied);
                        command.Parameters.AddWithValue("@role", user.RoleID);
                        command.Parameters.AddWithValue("@customer", user.CustomerID);
                        command.Parameters.AddWithValue("@code", user.ActivationCode);
                        command.CommandTimeout = SqlDatabase.Timeout;
                        var result = command.ExecuteReader();
                        if (result.Read())
                        {
                            var tmp = result["UserID"];
                            user.UserID = int.Parse(tmp.ToString());
                            //var tmp = reader.GetValue(0);
                            // position = int.Parse(tmp.ToString()) + 1;
                        }
                    }
                    else
                    {
                        var command = new SqlCommand
                        ("UPDATE [User] SET FirstName = @name, Surname = @surname, DateOfBirth = @birth " +
                        "WHERE UserID = @UserID"
                        , connection);
                        command.Parameters.AddWithValue("@name", user.FirstName);
                        command.Parameters.AddWithValue("@surname", user.Surname);
                        command.Parameters.AddWithValue("@birth", user.DateOfBirth);
                        command.Parameters.AddWithValue("@UserID", user.UserID);
                        command.CommandTimeout = SqlDatabase.Timeout;
                        var result = command.ExecuteReader();
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
        }

        // sprawdzenie czy użytkownik o danym adresie email już istnieje
        public static bool CheckUserExists(string email)
        {
            bool exists = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT COUNT(*) FROM [VUser] WHERE Email = @email", connection);
                    command.Parameters.AddWithValue("@email", email);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    var result = command.ExecuteReader();
                    while (result.Read())
                    {
                        if (result.GetInt32(0) > 0) exists = true;
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }  
            return exists;
        }

        // sprawdzanie czy kod aktywacyjny istnieje - do weryfikacji użytkownika
        public static bool FindActivationCode(string code)
        {
            bool exists = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT COUNT(*) FROM [VUser] WHERE ActivationCode = @code", connection);
                    command.Parameters.AddWithValue("@code", code);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    var result = command.ExecuteReader();
                    while (result.Read())
                    {
                        if (result.GetInt32(0) > 0) exists = true;
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            } 
            return exists;
        }

        
        // sprawdzenie czy użytkownik potwierdził swój adres email (wszedł w link aktywacyjny przesłany mailem)
        // do logowania użytkownika
        public static bool CheckEmailVeryfied(UserLogin login)
        {
            bool exists = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT * FROM [VUser] WHERE Email = @email and IsEmailVeryfied = 1", connection);
                    command.Parameters.AddWithValue("@email", login.Email);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    var result = command.ExecuteReader();
                    while (result.Read())
                    {
                        if (result.GetInt32(0) > 0) exists = true;
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return exists;
        }


        // email użytkownika został zweryfikowany - uzytkownik nacisnął link w mailu
        public static void UpdateUserEmailVeryfied(string code)
        {
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("UPDATE [VUser] SET IsEmailVeryfied = 1  WHERE ActivationCode = @code", connection);
                    command.Parameters.AddWithValue("@code", code);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    command.ExecuteNonQuery();
                    SqlDatabase.CloseConnection(connection);
                }
            }        
        }


        // zwraca hasło użytkownika o podanym adresie email
        public static string GetUserPassword(UserLogin login)
        {
            string password = "";
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT Password FROM [VUser] WHERE Email = @email", connection);
                    command.Parameters.AddWithValue("@email", login.Email);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    var result = command.ExecuteReader();
                    while (result.Read())
                    {
                        password = result.GetString(0);
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }     
            return password;
        }

        // dodanie danych użytkownika lub ich modyfikacja
        // CustomerID = SqlDatabase.CustomerAtr dodanie nowych danych
        // CustomerID > 0 modyfikacja istniejących
        public static bool AddModyfyAddress(Customer customer, String email)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.AddAddress", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@CompanyName", customer.CompanyName);
                    command.Parameters.AddWithValue("@City", customer.City);
                    command.Parameters.AddWithValue("@Street", customer.Street);
                    command.Parameters.AddWithValue("@ZipCode", customer.ZipCode);
                    command.Parameters.AddWithValue("@DiscountValue", customer.DiscountValue);
                    command.Parameters.AddWithValue("@UserID", GetUser(email).UserID);
                    command.Parameters.AddWithValue("@CustomerID", customer.CustomerID);
                    command.Parameters.AddWithValue("@CanReserve", customer.CanReserve);
                    command.Parameters["@CustomerID"].Direction = ParameterDirection.Output;

                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
                        customer.CustomerID = int.Parse(command.Parameters["@CustomerID"].Value.ToString());
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


        // usuwanie użytkownika
        // możliwe tylko jeśli nie ma powiązań w innych tabelach
        // przy wywołaniu dać w if'ie, żeby sprawdzić czy na pewno udało się usunąć
        // jeżli użytkownik nie miał rezerwacji itp, ale miał adres, to adres jest usuwany razem z użytkownikiem
        public static bool DeleteUser(User user)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.DeleteUser", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@UserID", user.UserID);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    try
                    {
                        command.ExecuteNonQuery();
                        user.UserID = 0;
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


        // zwraca listę użytkowników, którzy nie są skasowani
        public static List<User> GetUsers()
        {
            var list = new List<User>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {             
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("select * from dbo.VUser", connection);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new User()
                        {
                            UserID = (int)reader["UserID"],
                            FirstName = (string)reader["FirstName"],
                            Surname = (string)reader["Surname"],
                            Email = (string)reader["Email"],
                            DateOfBirth = (DateTime)reader["DateOfBirth"],
                            IsEmailVeryfied = (bool)reader["IsEmailVeryfied"],
                            RoleID = (int)reader["RoleID"],
                            CustomerID = (int)reader["CustomerID"],
                            RoleName = (string)reader["RoleName"]
                        });
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }           
            return list;
        }

        public static string GetUserRole(string email)
        {
            string RoleName = "";
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT RoleName FROM VUser WHERE Email = @email", connection);
                    command.Parameters.AddWithValue("@email", email);

                    var result = command.ExecuteReader();
                    while (result.Read())
                    {
                        RoleName = result.GetString(0);
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return RoleName;
        }

        public static User GetUser(string email)
        {
            User user=null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT * FROM VUser WHERE Email = @email", connection);
                    command.Parameters.AddWithValue("@email", email);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        user = new User() {
                            UserID = (int)reader["UserID"],
                            FirstName = (string)reader["FirstName"],
                            Surname = (string)reader["Surname"],
                            Email = (string)reader["Email"],
                            DateOfBirth = (DateTime)reader["DateOfBirth"],
                            IsEmailVeryfied = (bool)reader["IsEmailVeryfied"],
                            RoleID = (int)reader["RoleID"],
                            CustomerID = (int)reader["CustomerID"],
                            RoleName = (string)reader["RoleName"]
                        };
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return user;
        }

        public static Customer GetCustomer(User user)
        {
            Customer customer=null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("select * from Customer WHERE CustomerID = @customerID", connection);
                    command.Parameters.AddWithValue("@customerID", GetUser(user.Email).CustomerID);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        customer=new Customer()
                        {
                            CustomerID = (int)reader["CustomerID"],
                            CompanyName = (string)reader["CompanyName"],
                            City = (string)reader["City"],
                            Street = (string)reader["Street"],
                            ZipCode = (string)reader["ZipCode"],
                            DiscountValue = (decimal)reader["DiscountValue"],
                            CanReserve = (bool)reader["CanReserve"],
                        };
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return customer;
        }

        // zmiana hasła użytkownika, użytkownik jest zalogowany, klika w guzik 'zmień hasło'
        // OldPasswordHash -> wprowadzone przez użytkownika stare hasło
        // NewPasswordHash -> wprowadzone przez użytkownika nowe hasło
        // email -> email zalogowanego użytkownika
        public static bool ChangePassword(string OldPasswordHash, string NewPasswordHash, string email)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.ChangePassword", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@OldPasswordHash", OldPasswordHash);
                    command.Parameters.AddWithValue("@NewPasswordHash", NewPasswordHash);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
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

        // przypomnienie hasła użytkownika, użytkownik przy logowaniu klika w 'przypomnij hasło', na podany przez niego email zostaje wysłana wiadomość z kodem aktywacyjnym
        // NewPasswordHash -> wprowadzone przez użytkownika nowe hasło
        // ActivationCode -> wygenerowany do linka kod aktywacyjny (jak przy rejestracji)
        public static bool RemindPassword(string NewPasswordHash, string ActivationCode)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.ChangePassword", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ActivationCode", ActivationCode);
                    command.Parameters.AddWithValue("@NewPasswordHash", NewPasswordHash);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
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

        public static string GetUserPassword(User login)
        {
            string password = "";
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT Password FROM [VUser] WHERE Email = @email", connection);
                    command.Parameters.AddWithValue("@email", login.Email);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    var result = command.ExecuteReader();
                    while (result.Read())
                    {
                        password = result.GetString(0);
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return password;
        }

        // zapis nowego kodu aktywacyjnego
        // email -> wprowadzony przez użytkownika email
        // ActivationCode -> kod aktywacyjny wycięty z linka (guid ma stałą liczbę znaków)
        public static bool SaveUserActivationCode(string email, string ActivationCode)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.SaveUserActivationCode", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@ActivationCode", ActivationCode);
                    command.Parameters.AddWithValue("@email", email);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    // użyć jeżeli chcemy wykorzystać wartość return z procedury
                    //command.Parameters.Add("@ReturnValue", SqlDbType.Int, 4).Direction = ParameterDirection.ReturnValue;

                    try
                    {
                        command.ExecuteNonQuery();
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

    }
}