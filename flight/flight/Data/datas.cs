using flight.Models;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;


namespace RedisPOC.Data
{
    public class DbContextClass : DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<Passenger> passengers { get; set; }
        public DbSet<Ticket> tickets { get; set; }


    }




}