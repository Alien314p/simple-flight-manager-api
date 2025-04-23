using flight.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Generic;
using StackExchange.Redis;
using System.Text.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Windows.Input;
using System.Data.Common;
using static Azure.Core.HttpHeader;
using System.Security.Cryptography;
using Dapper;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using Serilog.Core;
using Microsoft.EntityFrameworkCore;




namespace flight.Services
{
    public class flightService
    {


        public async Task<bool> AddFlight(Flight obj)
        {
            string connectionString = "server=(localdb)\\Local; Initial Catalog=flightmanagerdb ;Integrated Security=True;";
            string insertQuery = "INSERT INTO flights (Destination, Origin, Capacity, FlightDate, FlightId, Company, FlightTime) VALUES (@Destination, @Origin, @Capacity, @FlightDate, @FlightId, @Company, @FlightTime)";

            

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    int rowsAffected = await connection.ExecuteAsync(insertQuery, new
                    {
                        Destination = obj.Destination,
                        Origin = obj.Origin,
                        Capacity = obj.Capacity,
                        FlightDate = obj.FlightDate,
                        FlightId = obj.FlightId,
                        Company = obj.Company,
                        FlightTime = obj.FlightTime
                    });

                    // Log the result
               

                    return rowsAffected > 0;
                }
                catch (SqlException ex)
                {
                    // Log the error
                    //ogger.Error($"An error occurred: {ex.Message}");
                    Console.WriteLine("error");
                    return false;
                }
            }
        }

        

        // not with dabber yet
        //adding tickets - storing them in db if its valid << considering the capacity>>.
        public async Task<bool> AddTicket(Ticket obj)
        {

            string connectionString = "server=(localdb)\\Local; Initial Catalog=flightmanagerdb ;Integrated Security=True;";
            string insertQuery = "INSERT INTO tickets (TicketId, Type , Price, FlightId, Status ) VALUES (@Value1, @Value2, @Value3, @Value4, @Value5)";

            string selectQuery = "SELECT * FROM flights WHERE FlightId= @id";

            string updateQuery = "UPDATE flights SET Capacity = @newcap WHERE FlightId = @id";



            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlConnection connection2 = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                        selectCommand.Parameters.AddWithValue("@id", obj.FlightId);


                        SqlDataReader reader = selectCommand.ExecuteReader();

                        SqlCommand Command = new SqlCommand(updateQuery, connection2);
                        //Command.CommandText = updateQuery;

                        while (reader.Read())
                        {
                            int capacity = reader.GetInt32(reader.GetOrdinal("Capacity"));
                            Console.WriteLine(capacity);


                            capacity--;

                            Console.WriteLine(capacity);
                            if (capacity >= 0)
                            {
                                connection2.Close();
                                Command.Parameters.Clear();
                                Command.Parameters.AddWithValue("@newcap", capacity);
                                Command.Parameters.AddWithValue("@id", obj.FlightId);


                                connection2.Open();
                                Console.WriteLine(capacity);
                                int rowsAffected = Command.ExecuteNonQuery();


                            }
                            else
                            {
                                //connection.Close();
                                Console.WriteLine("FULL CAPACITY");
                                return false;
                            }

                        }
                        //connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                        connection.Close();
                        return false;
                    }
                }

            }



            /// changed the connection name


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                int rowsAffected = connection.Execute(insertQuery, new
                {
                    Value1 = obj.TicketId,
                    Value2 = obj.Type,
                    Value3 = obj.Price,
                    Value4 = obj.FlightId,
                    Value5 = obj.Status
                });

                Console.WriteLine("Rows affected: " + rowsAffected);
                return true;
            }
        }


            //adding Passengers and buying ticket  - storing them in db if its valid.
            public async Task<bool> AddPassBuyTicket(Passenger obj)
        {

            string connectionString = "server=(localdb)\\Local; Initial Catalog=flightmanagerdb ;Integrated Security=True;";
            string insertQuery = "INSERT INTO passengers (Fullname, Age , Gender , TicketId, PassengerId, FlightId ) VALUES (@Fullname, @Age , @Gender , @TicketId, @PassengerId, @FlightId )";


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    await connection.OpenAsync();

                    int rowsAffected = await connection.ExecuteAsync(insertQuery, new
                    {
                        Fullname = obj.Fullname,
                        Age = obj.Age,
                        Gender = obj.Gender,
                        TicketId = obj.TicketId,
                        PassengerId = obj.personId,
                        FlightId = obj.FlightId,
                        
                    });

                    // Log the result

                    if (UpdateTicketStatus(obj))
                    {
                        Console.WriteLine("was true");
                        return true;

                    }
                    else
                    {
                        Console.WriteLine("was false");
                        return false;
                    }


                    
                }
                catch (SqlException ex)
                {
                    // Log the error
                    //ogger.Error($"An error occurred: {ex.Message}");
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

        }



        public bool UpdateTicketStatus(Passenger obj)
        {
            string connectionString = "server=(localdb)\\Local; Initial Catalog=flightmanagerdb ;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var ticket = connection.QuerySingleOrDefault<Ticket>("SELECT Status, TicketId FROM tickets WHERE TicketId = @TicketId", new { TicketId = obj.TicketId });

                if (ticket != null && !ticket.Status)
                {
                    int rowsAffected = connection.Execute("UPDATE tickets SET Status = @Status WHERE TicketId = @TicketId", new { Status = true, TicketId = obj.TicketId });

                    Console.WriteLine($"Rows affected by update: {rowsAffected}");

                    return true;
                }

                return false;
            }
        }


        // SHOW PASSENGER LIST FOR A FLIGHT
        public List<Dictionary<string, string>> GetPassList(string Fid)
        {
            string connectionString = "server=(localdb)\\Local; Initial Catalog=flightmanagerdb ;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var passengers = connection.Query("SELECT Fullname, Age, Gender FROM passengers WHERE FlightId = @fid", new { fid = Fid });

                var passList = passengers.Select(p => new Dictionary<string, string>
        {
                    { "name", p.Fullname },
                    { "age", p.Age.ToString() },
                    { "gender", p.Gender }}).ToList();

                return passList;
            }
        }




        //show the capacity left
        public List<Dictionary<string, string>> CapLeft()
        {
            string connectionString = "server=(localdb)\\Local; Initial Catalog=flightmanagerdb ;Integrated Security=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var flights = connection.Query("SELECT Capacity, Destination, Origin, FlightDate, FlightTime FROM flights WHERE Capacity > 0");

                var capLeftList = flights.Select(f => new Dictionary<string, string>
        {
            { "Destination", f.Destination },
            { "Origin", f.Origin },
            { "Date", f.FlightDate },
            { "Time", f.FlightTime },
            { "Tickets left", f.Capacity.ToString()}}).ToList();

                return capLeftList;
            }
        }


    }
}

