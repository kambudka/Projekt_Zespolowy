﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using SystemRezerwacjiKortow.Models;

namespace SystemRezerwacjiKortow.Database
{
    public static class SqlGear
    {
        // dodanie sprzętu lub jego modyfikacja
        // GearID = 0 to dodawanie nowego sprzętu
        // GearID > 0 modyfikowanie sprzętu o tym ID
        public static bool AddModifyGear(Gear gear)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.AddGear", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PriceH", gear.PriceH);
                    command.Parameters.AddWithValue("@Name", gear.Name);
                    command.Parameters.AddWithValue("@Amount", gear.Amount);
                    command.Parameters.AddWithValue("@GearID", gear.GearID);
                    command.Parameters["@GearID"].Direction = ParameterDirection.Output;  // żeby móc wyciągać dane

                    command.CommandTimeout = SqlDatabase.Timeout;

                    try
                    {
                        command.ExecuteNonQuery();
                        gear.GearID = int.Parse(command.Parameters["@GearID"].Value.ToString());
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

        // usuwanie sprzętu
        // sprawdzenie czy nie ma powiązań w innych tabelach jest po stronie bazy
        // przy wywołaniu dać w if'ie, żeby sprawdzić czy na pewno udało się usunąć
        public static bool DeleteGear(Court court)
        {
            bool result = false;
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("dbo.DeleteGear", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@GearID", court.CourtID);

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

        // zwraca listę sprzętu
        public static List<Gear> GetGears()
        {
            var list = new List<Gear>();
            using (SqlConnection connection = SqlDatabase.NewConnection())
            {
                if (SqlDatabase.OpenConnection(connection))
                {
                    var command = new SqlCommand("select * from dbo.VGear", connection);
                    command.CommandTimeout = SqlDatabase.Timeout;

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        list.Add(new Gear()
                        {
                            GearID = (int)reader["GearID"],
                            PriceH = (decimal)reader["PriceH"],
                            Name = (string)reader["Name"],
                            Amount = (int)reader["Amount"]
                        });
                    }
                    SqlDatabase.CloseConnection(connection);
                }
            }    
            return list;
        }
    }
}