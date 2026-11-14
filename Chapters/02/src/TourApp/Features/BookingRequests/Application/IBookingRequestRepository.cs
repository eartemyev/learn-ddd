using TourApp.Features.BookingRequests.Domain;

namespace TourApp.Tests.Features.BookingRequests.Application
{
    public interface IBookingRequestRepository
    {
        Task<BookingRequest?> GetAsync(
            Guid id,
            CancellationToken cancellationToken);

        Task SaveAsync(
            BookingRequest booking,
            CancellationToken cancellationToken);
    }
}
