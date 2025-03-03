using System.Text.Json;
using MediatR;
using MediatR.Pipeline;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;

namespace TgOperator.Features.Players.PostProcessors
{
    public class OperatorApiLogPostProcessor<TRequest, TResponse> : IRequestPostProcessor<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private const string CommandSuffix = "Command";
        private const string RequestPropertyName = "Request";
        
        private readonly IMongoCollection<OperatorApiLog> _operatorApiLogs;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OperatorApiLogPostProcessor(
            IMongoDbFactory mongoDbFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _operatorApiLogs = mongoDbFactory.GetCollection<OperatorApiLog>();
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
            return commandName.Replace(CommandSuffix, "");
        }

        private object GetCallData(TRequest request)
        {
            var requestProperty = request.GetType().GetProperty(RequestPropertyName);
            return requestProperty?.GetValue(request) ?? request;
        }
    }
} 