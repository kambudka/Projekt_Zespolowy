using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlCompany
    {
        // dodanie/modyfikacja/skasowanie godzin otwarcia
        // skasowanie - godzina otwarcia = godzina zamknięcia
        // jeśli jakiegoś dnia nie ma, to znaczy, że korty są nieczynne
        public static bool AddModifyOpeningHours(OpeningHours openingHours)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.AddOpeningHours", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@DayOfWeek", openingHours.DayOfWeek);
                    command.Parameters.AddWithValue("@TimeFrom", openingHours.TimeFrom);
                    command.Parameters.AddWithValue("@TimeTo", openingHours.TimeTo);

                    command.CommandTimeout = SqlDatabase.Timeout;

                    try
                    {
                        command.ExecuteNonQuery();
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

        // zwraca listę godzin otwarcia
        public static List<OpeningHours> GetOpeningHours()
        {
            var list = new List<OpeningHours>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("select * from dbo.VOpeningHours", connection);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        OpeningHours oH = new OpeningHours();
                        oH.DayOfWeek = (int)reader["DayOfWeek"];
                        oH.DayName = "";
                        oH.TimeFrom = (TimeSpan)reader["TimeFrom"];
                        oH.TimeTo = (TimeSpan)reader["TimeTo"];
                        switch (oH.DayOfWeek)
                        {
                            case 1: // dla sqla pierwszy dzien tygodnia to niedziela
                                oH.DayName = Resources.Texts.DayWeek7;
                                break;
                            case 2:
                                oH.DayName = Resources.Texts.DayWeek1;
                                break;
                            case 3:
                                oH.DayName = Resources.Texts.DayWeek2;
                                break;
                            case 4:
                                oH.DayName = Resources.Texts.DayWeek3;
                                break;
                            case 5:
                                oH.DayName = Resources.Texts.DayWeek4;
                                break;
                            case 6:
                                oH.DayName = Resources.Texts.DayWeek5;
                                break;
                            case 7:
                                oH.DayName = Resources.Texts.DayWeek6;
                                break;

                        }
                        list.Add(oH);

                        //list.Add(new OpeningHours()
                        //{
                        //    DayOfWeek = (int)reader["DayOfWeek"],
                        //    DayName = "",
                        //    TimeFrom = (TimeSpan)reader["TimeFrom"],
                        //    TimeTo = (TimeSpan)reader["TimeTo"],
                        //});

                        
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return list;
        }

        // zwraca godziny otwarcia daneogo dnia
        public static OpeningHours GetOpeningHour(int dayOfWeek)
        {
            OpeningHours openingHours = null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("select * from dbo.VOpeningHours where DayOfWeek = @dayOfWeek", connection);
                    command.Parameters.AddWithValue("@dayOfWeek", dayOfWeek);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        openingHours = new OpeningHours()
                        {
                            DayOfWeek = (int)reader["DayOfWeek"],
                            DayName = "",
                            TimeFrom = (TimeSpan)reader["TimeFrom"],
                            TimeTo = (TimeSpan)reader["TimeTo"],
                        };                   
                    }
                    if(openingHours!=null)
                    switch (openingHours.DayOfWeek)
                    {
                        case 1: // dla sqla pierwszy dzien tygodnia to niedziela
                            openingHours.DayName = Resources.Texts.DayWeek7;
                            break;
                        case 2:
                            openingHours.DayName = Resources.Texts.DayWeek1;
                            break;
                        case 3:
                            openingHours.DayName = Resources.Texts.DayWeek2;
                            break;
                        case 4:
                            openingHours.DayName = Resources.Texts.DayWeek3;
                            break;
                        case 5:
                            openingHours.DayName = Resources.Texts.DayWeek4;
                            break;
                        case 6:
                            openingHours.DayName = Resources.Texts.DayWeek5;
                            break;
                        case 7:
                            openingHours.DayName = Resources.Texts.DayWeek6;
                            break;
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return openingHours;
        }

        // zwraca kompleks -> dane kompleksu, w tabeli kompleks zawsze jest jeden rekord
        public static ComplexCourt GetComplex()
        {
            ComplexCourt complexCourt = null;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("SELECT * FROM ComplexCourt", connection);

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        complexCourt = new ComplexCourt()
                        {
                            ComplexName = (string)reader["ComplexName"],
                            City = (string)reader["City"],
                            Street = (string)reader["Street"],
                            ZipCode = (string)reader["ZipCode"],
                            FirstWinterSeasonMonth = (int)reader["FirstWinterSeasonMonth"],
                            LastWinterSeasonMonth = (int)reader["LastWinterSeasonMonth"],
                        };
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }
            return complexCourt;
        }
    }
}