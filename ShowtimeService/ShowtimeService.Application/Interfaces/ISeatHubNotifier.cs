namespace ShowtimeService.Application.Interfaces
{
    public interface ISeatHubNotifier
    {
        Task NotifySeatUpdated(Guid showtimeId, string seatNumber, bool locked, string userId);
    }
}
