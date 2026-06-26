using FlightReservations.Data;
using FlightReservations.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using FlightReservations.Models;
using FlightReservations.Repositories;

namespace FlightReservations.Tests
{
    public class TestContext : IDisposable
    {
        private readonly SqliteConnection _connection;
        public ApplicationDbContext Db { get; }
        public FlightService FlightService { get; }
        public ReservationService ReservationService { get; }
        public string visitorId { get; } = "visitor-1";

        public TestContext()
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_connection).Options;

            Db = new ApplicationDbContext(options);
            Db.Database.EnsureCreated();

            Db.Users.Add(new ApplicationUser { Id = visitorId, UserName = "visitor" });
            Db.SaveChanges();

            var flightRepo = new FlightRepository(Db);
            var reservationRepo = new ReservationRepository(Db);
            var unitOfWork = new UnitOfWork(Db, flightRepo, reservationRepo);

            FlightService = new FlightService(unitOfWork);
            ReservationService = new ReservationService(unitOfWork, FlightService);
        }

        public void Dispose()
        {
            Db.Dispose();
            _connection.Dispose();
        }


    }
}
