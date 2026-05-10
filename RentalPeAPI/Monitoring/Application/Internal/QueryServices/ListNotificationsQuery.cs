// Monitoring/Application/Internal/QueryServices/ListNotificationsQuery.cs
using MediatR;
using System.Collections.Generic;
using RentalPeAPI.Monitoring.Domain.Entities;

namespace RentalPeAPI.Monitoring.Application.Internal.QueryServices;

public record ListNotificationsQuery(int ProjectId) 
    : IRequest<IEnumerable<Notification>>;