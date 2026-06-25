using FlightReservations.Models;
using Microsoft.AspNetCore.Identity;

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
        public static async Task SeedUsersAndRolesAsync(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Agent", "Visitor" };
            foreach(var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role)) 
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            await CreateUserAsync(userManager, "admin", "admin123", "Admin");
            await CreateUserAsync(userManager, "agent", "agent123", "Agent");
            await CreateUserAsync(userManager, "visitor", "visitor123", "Visitor");
            
        }

        private static async Task CreateUserAsync(UserManager<ApplicationUser> userManager,
            string userName, string password, string role)
        {
            if (await userManager.FindByNameAsync(userName) != null) return;

            var user = new ApplicationUser
            {
                UserName = userName,
                Email = $"{userName}@demo.local",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded) await userManager.AddToRoleAsync(user, role);
        }
    }
}
