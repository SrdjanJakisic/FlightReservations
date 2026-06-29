using FlightReservations.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlightReservations.Tests
{
    public class ReservationLogicTests
    {
        private static Flight ActiveFlight(int totalSeats, int daysFromNow)
            => new Flight
            {
                Origin = City.Belgrade,
                Destination = City.Nis,
                DepartureDate = DateTime.Now.AddDays(daysFromNow),
                NumberOfStops = 0,
                TotalSeats = totalSeats,
                Status = FlightStatus.Active
            };

        [Fact]
        public async Task CreateReservation_FlightLessThan3DaysAway_Fails()
        {
            using var testContext = new TestContext();
            var flight = ActiveFlight(totalSeats: 50, daysFromNow: 1);
            testContext.Db.Flights.Add(flight);
            testContext.Db.SaveChanges();

            var (success, error) = await testContext.ReservationService
                .CreateReservationAsync(flight.Id, testContext.visitorId, 1);

            Assert.False(success);
            Assert.Contains("3 days", error);
        }
        [Fact]
        public async Task GetAvailableSeats_SubtractsPendingAndApproved_IgnoresCancelled()
        {
            using var testContext = new TestContext();
            var flight = ActiveFlight(totalSeats: 10, daysFromNow: 10);
            testContext.Db.Flights.Add(flight);
            testContext.Db.SaveChanges();

            testContext.Db.Reservations.Add(new Reservation
            {
                FlightId = flight.Id,
                VisitorId = testContext.visitorId,
                NumberOfSeats = 3,
                Status = ReservationStatus.Pending
            });

            testContext.Db.Reservations.Add(new Reservation
            {
                FlightId = flight.Id,
                VisitorId = testContext.visitorId,
                NumberOfSeats = 2,
                Status = ReservationStatus.Approved
            });

            testContext.Db.Reservations.Add(new Reservation
            {
                FlightId = flight.Id,
                VisitorId = testContext.visitorId,
                NumberOfSeats = 5,
                Status = ReservationStatus.Cancelled
            });

            testContext.Db.SaveChanges();

            var available = await testContext.FlightService.GetAvailableSeatsAsync(flight.Id);

            Assert.Equal(5, available);
        }
        [Fact]
        public async Task CreateReservation_MoreSeatsThanAvailable_Fails()
        {
            using var testContext = new TestContext();
            var flight = ActiveFlight(totalSeats: 3, daysFromNow: 10);
            testContext.Db.Flights.Add(flight);
            testContext.Db.SaveChanges();

            var (success, error) = await testContext.ReservationService
                .CreateReservationAsync(flight.Id, testContext.visitorId, 5);

            Assert.False(success);
            Assert.Contains("available", error);
        }
        [Fact]
        public async Task CreateReservation_Valid_CreatesPendingReservation()
        {
            using var testContext = new TestContext();
            var flight = ActiveFlight(totalSeats: 50, daysFromNow: 10);
            testContext.Db.Flights.Add(flight);
            testContext.Db.SaveChanges();

            var (success, error) = await testContext.ReservationService
                .CreateReservationAsync(flight.Id, testContext.visitorId, 2);

            Assert.True(success, error);
            Assert.Null(error);

            var reservation = testContext.Db.Reservations.Single();
            Assert.Equal(ReservationStatus.Pending, reservation.Status);
            Assert.Equal(2, reservation.NumberOfSeats);
        }
    }
}
