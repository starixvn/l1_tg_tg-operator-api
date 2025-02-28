using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using TgOperator.Features.Players.Models;

public partial class TgOperatorClient(HttpClient httpClient) : ITgOperatorClient
{
    private static readonly JsonSerializerOptions _SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public async Task<Player> GetPlayerBalanceAsync(PlayerBalanceRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/player/balance", request, _SerializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Player>(_SerializerOptions, cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<BetResult> PlaceBetAsync(BetRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/player/bet", request, _SerializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BetResult>(_SerializerOptions, cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<EndRoundResult> EndRoundAsync(EndRoundRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/player/endround", request, _SerializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<EndRoundResult>(_SerializerOptions, cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<VoidGameResult> VoidGameAsync(VoidGameRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/player/voidgame", request, _SerializerOptions, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<VoidGameResult>(_SerializerOptions, cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    public async Task<BetLimitResult> GetBetLimitAsync(BetLimitRequest request, CancellationToken cancellationToken = default)
    {
        var builder = new UriBuilder(new Uri(httpClient.BaseAddress!, "api/player/betlimit"));
        var query = HttpUtility.ParseQueryString(builder.Query);
        query["playerId"] = request.PlayerId;
        query["gameTypeId"] = request.GameTypeId;
        builder.Query = query.ToString();

        var response = await httpClient.GetAsync(builder.ToString(), cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<BetLimitResult>(_SerializerOptions, cancellationToken)
            ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}
