using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Sentry;

namespace TgOperator.Features.Players.Handlers
{
    public class MaintenanceCommandHandler : IRequestHandler<MaintenanceCommand, MaintenanceResult>
    {
        private readonly IMongoCollection<MaintenanceSchedule> _maintenanceSchedules;
        private readonly IHub _sentryHub;

        public MaintenanceCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _maintenanceSchedules = mongoDbFactory.GetCollection<MaintenanceSchedule>(
                mongoDbSettings.Value.MaintenanceSchedulesCollectionName);
            _sentryHub = sentryHub;
        }

        public async Task<MaintenanceResult> Handle(MaintenanceCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var request = command.MaintenanceRequest;
                
                var maintenance = await _maintenanceSchedules
                    .Find(x => 
                        x.OperatorId == request.OperatorId &&
                        x.SiteId == request.SiteId &&
                        (x.GameTypeId == request.GameTypeId || string.IsNullOrEmpty(x.GameTypeId)) &&
                        x.StartTime <= DateTime.UtcNow &&
                        x.EndTime >= DateTime.UtcNow &&
                        x.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                return new MaintenanceResult
                {
                    IsMaintenance = maintenance != null,
                    Message = maintenance?.Message ?? "No maintenance scheduled",
                    StartTime = maintenance?.StartTime,
                    EndTime = maintenance?.EndTime
                };
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex);
                throw;
            }
        }
    }
} 