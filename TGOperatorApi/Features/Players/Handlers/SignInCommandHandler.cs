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
    public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResult>
    {
        private readonly IMongoCollection<Player> _players;
        private readonly IHub _sentryHub;

        public SignInCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _players = mongoDbFactory.GetCollection<Player>();
            _sentryHub = sentryHub;
        }

        public async Task<SignInResult> Handle(SignInCommand command, CancellationToken cancellationToken)
        {
            var request = command.SignInRequest;
            var session = await _players.Database.Client.StartSessionAsync();

            try
            {
                session.StartTransaction();
                var vendorId = "0";
                var siteId = "0";
                var browserUserAgent = request.Inputs.GetValueOrDefault("browserUserAgent", "");
                var gameId = request.Inputs.GetValueOrDefault("gameId", "");
                var domain = request.Inputs.GetValueOrDefault("domain", "");
                var location = request.Inputs.GetValueOrDefault("location", "");
                var gameTypeId = request.Inputs.GetValueOrDefault("gameTypeId", "");
                var ip = request.Inputs.GetValueOrDefault("ip", "");
                var returnUrl = request.Inputs.GetValueOrDefault("returnUrl", "");

                var player = await _players.Find(session, x =>
                  x.OperatorId == vendorId &&
                  x.PlayerId == request.PlayerId.ToString() &&
                  x.SiteId == siteId)
                  .FirstOrDefaultAsync(cancellationToken);

                if (player == null)
                {
                    return new SignInResult
                    {
                        Success = false,
                        ErrorCode = 4012,
                        Description = $"Player not found: {request.PlayerId}"
                    };
                }

                var vGSignInRequest = new VGSignInRequest
                {
                    Auth = new Auth
                    {
                        OperatorId = vendorId
                    },
                    BrowserUserAgent = browserUserAgent,
                    PlayerId = request.PlayerId,
                    PlayerName = player.CharacterName,
                    GameId = gameId,
                    Currency = request.CurrencyId,
                    IP = ip,
                    ReturnUrl = returnUrl,
                    Domain = domain,
                    Location = location,
                    GameTypeId = int.Parse(gameTypeId),
                    IsCash = false,
                    DisplayCurrency = "IDR",
                    AppType = 1,
                    DisableJackpotGames = false,
                    DisableAward = false,
                    OperatorToken = "5d7aa6fb-e90c-482b-bc2e-12b03faf8ee4",
                    Ref = "m_direct",
                    SiteName = "site_name"
                };

                //Todo call VG

                await session.CommitTransactionAsync(cancellationToken);

                return new SignInResult
                {
                    ErrorCode = 0,
                    Message = "",
                    RedirectUrl = "https://www.google.com/",
                    Token = ""
                };
            }
            catch (Exception ex)
            {
                await session.AbortTransactionAsync(cancellationToken);
                _sentryHub.CaptureException(ex);
                throw;
            }
            finally
            {
                session?.Dispose();
            }
        }
    }
} 