using System.Text.Json;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.Configuration;
using TgOperator.Shared.MongoDB;

namespace TgOperator.Features.Players.PostProcessors
{
    public class OperatorApiLogPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMongoCollection<OperatorApiLog> _operatorApiLogs;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OperatorApiLogPostProcessor(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _operatorApiLogs = mongoDbFactory.GetCollection<OperatorApiLog>(
                mongoDbSettings.Value.OperatorApiLogsCollectionName);
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
        {
            if (request is not IPlayerCommand playerCommand)
            {
                return;
            }

            var log = new OperatorApiLog
            {
                Type = GetOperationType(request),
                Call = GetCallData(request),
                Response = response,
                Timestamp = DateTime.UtcNow
            };

            await _operatorApiLogs.InsertOneAsync(log, cancellationToken);
        }

        private string GetOperationType(TRequest request)
        {
            var commandName = request.GetType().Name;
            return commandName.EndsWith("Command") 
                ? commandName[..^7] // Remove "Command" suffix
                : commandName;
        }

        private object GetCallData(TRequest request)
        {
            var requestProperty = request.GetType().GetProperty("Request");
            return requestProperty?.GetValue(request) ?? request;
        }
    }
} 