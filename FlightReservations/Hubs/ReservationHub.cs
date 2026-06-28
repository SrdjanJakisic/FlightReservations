using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace FlightReservations.Hubs
{
    [Authorize]
    public class ReservationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User?.IsInRole("Agent") == true)
                await Groups.AddToGroupAsync(Context.ConnectionId, "Agents");

            await base.OnConnectedAsync();
        }
    }
}
