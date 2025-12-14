using Microsoft.AspNetCore.SignalR;
using ShowtimeService.Application.Services;

namespace ShowtimeService.API.Hubs
{
    public class SeatHub : Hub
    {
        private readonly ShowtimeSeatService _lockService;

        public SeatHub(ShowtimeSeatService lockService)
        {
            _lockService = lockService;
        }

        // Client invokes: connection.invoke("LockSeat", showtimeId, seatNumber, userId)
        public async Task LockSeat(Guid showtimeId, Guid seatId, string userId)
        {
            var locked = await _lockService.TryLockSeatDb(showtimeId, seatId);

            // Broadcast only to group (showtime). Use Groups if you join groups.
            await Clients.Group(showtimeId.ToString()).SendAsync("SeatUpdated", new
            {
                showtimeId,
                seat = seatId,
                locked,
                //user = locked ? userId : (await _lockService.GetLockOwner(showtimeId, seatId))
            });
        }

        public async Task ReleaseSeat(Guid showtimeId, Guid seatId, string userId)
        {
            var released = await _lockService.ReleaseSeatDb(showtimeId, seatId);

            if (released)
            {
                await Clients.Group(showtimeId.ToString()).SendAsync("SeatUpdated", new
                {
                    showtimeId,
                    seat = seatId,
                    locked = false,
                    user = ""
                });
            }
        }

        public override async Task OnConnectedAsync()
        {
            // Optionally read showtimeId query param and join group
            var http = Context.GetHttpContext();
            var showtimeId = http.Request.Query["showtimeId"].ToString();
            if (!string.IsNullOrEmpty(showtimeId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, showtimeId);
            }
            await base.OnConnectedAsync();
        }
    }
}
