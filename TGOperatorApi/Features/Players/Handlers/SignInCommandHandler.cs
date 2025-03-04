using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Newtonsoft.Json;
using VendorCore;
using TgOperator.Features.Players.PostProcessors;
using TGOperator.Core.Model;
using static System.Net.WebRequestMethods;
using TgOperator.Features.Vendors.Services;
using System.Net.Http.Json;
using System.Text;
using System.Security.Cryptography;

namespace TgOperator.Features.Players.Handlers
{
    public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResult>
    {
        private readonly IMongoCollection<Player> _players;
        private readonly IMongoCollection<OperatorApiLog> _operatorApiLogs;
        private readonly IHub _sentryHub;

        public SignInCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IHub sentryHub)
        {
            _players = mongoDbFactory.GetCollection<Player>(
                mongoDbSettings.Value.PlayersCollectionName);
            _sentryHub = sentryHub;
        }

        public async Task<SignInResult> Handle(SignInCommand command, CancellationToken cancellationToken)
        {
            var request = command.SignInRequest;
            var session = await _players.Database.Client.StartSessionAsync();

            try
            {
                session.StartTransaction();
                var config = new VGConfig();
                var ApiUrl = "https://l2-api.lumigame.com/SignIn/Index/";
                var operatorId = "SabaPlay";
                var operatorSecret = "SabaPlay";
                var operatorToken = "TGVGSabaPlay";
                var siteId = "4100300";
                var clientId = "ONEworks1";
                var clientSecret = "123licensee1%^&sit";

                //TODO check player
                //var player = await _players.Find(session, x =>
                //  x.PlayerId == request.PlayerId.ToString() &&
                //  x.SiteId == siteId)
                //  .FirstOrDefaultAsync(cancellationToken);

                //if (player == null)
                //{
                //    return new SignInResult
                //    {
                //        Success = false,
                //        ErrorCode = 4012,
                //        Description = $"Player not found: {request.PlayerId}"
                //    };
                //}
               

                var vGSignInRequest = new VGSignInRequest
                {
                    Auth = new Auth
                    {
                        OperatorId = operatorId,
                        OperatorSecret = operatorSecret,
                    },
                    SiteId = siteId,
                    PlayerId = request.PlayerId,
                    PlayerName = "TG",
                    Currency = request.CurrencyId,
                    IP = request.Inputs.GetValueOrDefault("IP", ""),
                    BrowserUserAgent = request.Inputs.GetValueOrDefault("BrowserUserAgent", ""),
                    SiteName = "site_name",
                    IsCash = false,
                    GameId = request.Inputs.GetValueOrDefault("GameId", ""),
                    ReturnUrl = request.Inputs.GetValueOrDefault("ReturnUrl", ""),
                    Domain = request.Inputs.GetValueOrDefault("Domain", ""),
                    Location = request.Inputs.GetValueOrDefault("Location", ""),
                    GameTypeId = int.Parse(request.Inputs.GetValueOrDefault("GameTypeId", "")),
                    AppType = 1,
                    OperatorToken = operatorToken,
                    Ref = "m_direct",
                };

                var stringData = JsonConvert.SerializeObject(vGSignInRequest);
                var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(operatorSecret));
                var hashResult = hmac.ComputeHash(Encoding.ASCII.GetBytes(stringData));
                var accessKey = Convert.ToBase64String(hashResult);
                var postdata = JsonConvert.SerializeObject(vGSignInRequest, Formatting.None);
                var header = new Dictionary<string, string>()
                {
                    { "Access-Key", accessKey },
                };

                //todo call VG
                var result = ThirdPartyHelperBase.HttpPost(ApiUrl, postdata, "application/json", header);

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