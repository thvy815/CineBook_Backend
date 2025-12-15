using Microsoft.AspNetCore.SignalR;
using ShowtimeService.API.Hubs;
using ShowtimeService.Application.Interfaces;

namespace ShowtimeService.API.HubNotifiers
{
    public class SeatHubNotifier : ISeatHubNotifier
    {
        private readonly IHubContext<SeatHub> _hub;

        public SeatHubNotifier(IHubContext<SeatHub> hub)
        {
            _hub = hub;
        }

        public async Task NotifySeatUpdated(Guid showtimeId, string seatNumber, bool locked)
        {
            await _hub.Clients.Group(showtimeId.ToString()).SendAsync("SeatUpdated", new
            {
                showtimeId,
                seat = seatNumber,
                locked
            });
        }
    }
}
