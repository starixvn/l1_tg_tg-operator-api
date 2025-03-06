using MediatR;
using TgOperator.Features.Players.Commands;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using TGOperatorApi.Features.Players.Models.Enums;

namespace TgOperator.Features.Players.Handlers
{
    public class MaintenanceCommandHandler : IRequestHandler<MaintenanceCommand, MaintenanceResponse>
    {
        private readonly IHub _sentryHub;

        public MaintenanceCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _sentryHub = sentryHub;
        }

        public async Task<MaintenanceResponse> Handle(MaintenanceCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var now = DateTime.UtcNow;
            try
            {
                //check manternance

                //TODO  return maintenance
                return new MaintenanceResponse
                {
                    TimeStamp = now,
                    Result = new MaintenanceResult
                    {
                        Seq = request.Seq,
                        IsMaintenance = false,
                        Message = "No maintenance scheduled",
                        StartTime = now,
                        EndTime = now,
                        ErrorCode = 0
                    }
                };
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("maintenanceRequest", request);
                });

                return new MaintenanceResponse
                {
                    TimeStamp = now,
                    Result = new MaintenanceResult
                    {
                        Seq = request.Seq,
                        ErrorCode = ErrorCode.Fail,
                        ErrorDescription = "Unspecified Error."
                    }
                };
            }
        }
    }
} 