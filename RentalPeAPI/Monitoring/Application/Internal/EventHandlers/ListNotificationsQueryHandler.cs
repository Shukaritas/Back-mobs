// Monitoring/Application/Internal/QueryServices/ListNotificationsQueryHandler.cs
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RentalPeAPI.Monitoring.Domain.Entities;
using RentalPeAPI.Monitoring.Domain.Repositories;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

public class ListNotificationsQueryHandler 
    : IRequestHandler<ListNotificationsQuery, IEnumerable<Notification>>
{
    private readonly INotificationRepository _notificationRepository;

    public ListNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<IEnumerable<Notification>> Handle(
        ListNotificationsQuery query, 
        CancellationToken cancellationToken)
    {
        // Lista las notificaciones por proyecto
        return await _notificationRepository.ListByProjectIdAsync(query.ProjectId);
    }
}