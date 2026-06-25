using FlightReservations.Models;

namespace FlightReservations.Data
{
    public static class DbInitializer
    {
        public static void SeedFlights(ApplicationDbContext context)
        {
            if (context.Flights.Any()) return;

            var flights = new List<Flight>
            {
                new Flight
                {
                    Origin = City.Belgrade,
                    Destination = City.Nis,
                    DepartureDate = DateTime.Now.AddDays(10),
                    NumberOfStops = 0,
                    TotalSeats = 50,
                    Status = FlightStatus.Active
            },

                new Flight
                {
                    Origin = City.Belgrade,
                    Destination = City.Kraljevo,
                    DepartureDate = DateTime.Now.AddDays(7),
                    NumberOfStops = 1,
                    TotalSeats = 4,
                    Status = FlightStatus.Active
                },

                new Flight
                {
                    Origin = City.Nis,
                    Destination = City.Belgrade,
                    DepartureDate = DateTime.Now.AddDays(2),
                    NumberOfStops = 0,
                    TotalSeats = 30,
                    Status = FlightStatus.Active
                },

                new Flight
                {
                    Origin = City.Kraljevo,
                    Destination = City.Nis,
                    DepartureDate = DateTime.Now.AddDays(15),
                    NumberOfStops = 0,
                    TotalSeats = 20,
                    Status = FlightStatus.Active
                },
            };

            

            context.Flights.AddRange(flights);
            context.SaveChanges();
        }
    }
}
