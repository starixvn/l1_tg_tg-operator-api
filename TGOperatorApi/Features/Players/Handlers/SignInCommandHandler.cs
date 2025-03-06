using MediatR;
using MongoDB.Driver;
using TgOperator.Features.Players.Commands;
using TgOperator.Features.Players.Models;
using TgOperator.Shared.MongoDB;
using Microsoft.Extensions.Options;
using TgOperator.Shared.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using TgOperator.Core.Features.Player;
using TGOperatorApi.Features.Players.Models.Enums;
using TGOperatorApi.Features.Vendors.Models;

namespace TgOperator.Features.Players.Handlers
{
    public class SignInCommandHandler : IRequestHandler<SignInCommand, SignInResponse>
    {
        private readonly IHub _sentryHub;
        private readonly VendorConfig _VendorConfig;

        public SignInCommandHandler(
            IMongoDbFactory mongoDbFactory,
            IOptions<MongoDbSettings> mongoDbSettings,
            IOptions<VendorConfig> VendorConfig,
            IHub sentryHub)
        {
            _sentryHub = sentryHub;
            _VendorConfig = VendorConfig.Value;
        }

        public async Task<SignInResponse> Handle(SignInCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var now = DateTime.UtcNow;

            try
            {
                var signInRequest = CreateVGSignInRequest(request);
                var header = CreateRequestHeader(signInRequest, _VendorConfig.OperatorSecret);
                
                var result = await CallVendorApi(signInRequest, header);
                
                return ParseSignInResult(result, request, now);
            }
            catch (Exception ex)
            {
                _sentryHub.CaptureException(ex, scope =>
                {
                    scope.SetExtra("signInRequest", request);
                });

                return new SignInResponse
                {
                    TimeStamp = now,
                    Result = new SignInResult
                    {
                        Seq = request.Seq,
                        ErrorCode = ErrorCode.Fail,
                        ErrorDescription = ex.Message
                    }
                };
            }
        }

        private VGSignInRequest CreateVGSignInRequest(SignInRequest request)
        {
            return new VGSignInRequest
            {
                Auth = new Auth
                {
                    OperatorId = _VendorConfig.OperatorId,
                    OperatorSecret = _VendorConfig.OperatorSecret,
                },
                SiteId = _VendorConfig.SiteId,
                PlayerId = request.PlayerId,
                Currency = request.CurrencyId,
                IP = request.Inputs.GetValueOrDefault("IP", ""),
                BrowserUserAgent = request.Inputs.GetValueOrDefault("BrowserUserAgent", ""),
                SiteName = _VendorConfig.SiteName,
                GameId = request.Inputs.GetValueOrDefault("GameId", ""),
                ReturnUrl = request.Inputs.GetValueOrDefault("ReturnUrl", ""),
                Domain = request.Inputs.GetValueOrDefault("Domain", ""),
                Location = request.Inputs.GetValueOrDefault("Location", ""),
                GameTypeId = int.Parse(request.Inputs.GetValueOrDefault("GameTypeId", "")),
                OperatorToken = _VendorConfig.OperatorToken,
                Ref = _VendorConfig.Ref,
            };
        }

        private Dictionary<string, string> CreateRequestHeader(VGSignInRequest request, string operatorSecret)
        {
            var stringData = JsonConvert.SerializeObject(request);
            var hmac = new HMACSHA256(Encoding.ASCII.GetBytes(operatorSecret));
            var hashResult = hmac.ComputeHash(Encoding.ASCII.GetBytes(stringData));
            var accessKey = Convert.ToBase64String(hashResult);

            return new Dictionary<string, string>
            {
                { "Access-Key", accessKey }
            };
        }

        private async Task<string> CallVendorApi(VGSignInRequest request, Dictionary<string, string> header)
        {
            var postdata = JsonConvert.SerializeObject(request, Formatting.None);
            return VendorHelperBase.HttpPost(_VendorConfig.ApiUrl, postdata, "application/json", header);
        }

        private SignInResponse ParseSignInResult(string result, SignInRequest signInRequest, DateTime now)
        {
            if (!IsJSON(result))
            {
                return new SignInResponse
                {
                    TimeStamp = now,
                    Result = new SignInResult
                    {
                        Seq = signInRequest.Seq,
                        ErrorMessage = "No Result",
                        ErrorCode = ErrorCode.Fail
                    }
                };
            }

            var json = JsonConvert.DeserializeObject<SignInResult>(result);
            return json.ErrorCode == 0
                ? new SignInResponse
                {
                    TimeStamp = now,
                    Result = new SignInResult
                    {
                        Seq = signInRequest.Seq,
                        ErrorCode = ErrorCode.Success,
                        ErrorMessage = "Success",
                        RedirectUrl = json.RedirectUrl,
                        Token = json.Token
                    }                    
                }
                : new SignInResponse
                {
                    TimeStamp = now,
                    Result = new SignInResult
                    {
                        Seq = signInRequest.Seq,
                        ErrorCode = ErrorCode.Fail,
                        ErrorMessage = json.ErrorMessage
                    }
                };
        }

        private bool IsJSON(string jsonString)
        {
            var value = jsonString.Trim();

            return (value.StartsWith("{") && value.EndsWith("}")) || (value.StartsWith("[") && value.EndsWith("]"));
        }
    }
} 