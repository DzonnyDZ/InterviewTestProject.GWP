using Galytix.Test.Business.Abstractions;
using Galytix.Test.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Galytix.Test.API.Controllers;

/// <summary>Provides WebAPI access to gross written premium statistics</summary>
[ApiController]
[Route("server/api/gwp")]
public class CountryGwpController : ControllerBase
{
    private readonly ILogger<CountryGwpController> logger;
    private readonly ILobBusiness bll;
    private readonly IMemoryCache cache;

    /// <summary>Initializes a new instance of the <see cref="CountryGwpController"/> class.</summary>
    /// <param name="bll">Business layer</param>
    /// <param name="cache">In-memory cache</param>
    /// <param name="logger">Logging sink</param>
    public CountryGwpController(ILobBusiness bll, IMemoryCache cache, ILogger<CountryGwpController> logger)
    {
        this.bll = bll ?? throw new ArgumentNullException(nameof(bll));
        this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>  an average gross written premium (GWP) over configured period</summary>
    /// <param name="request">Indicates country and lines of business to get GWP averages for</param>
    /// <returns>GWP averages for given country an lines of business from <paramref name="request"/></returns>
    [HttpPost("avg", Name = "Avg")]
    [Produces(typeof(IReadOnlyDictionary<string, decimal>))]
    public async Task<IActionResult> AvgAsync([FromBody] LobRequest request)
    {
        logger.LogDebug("Stats requested");
        string country = request.Country;
        string[] lobs = request.Lob.Order().ToArray();

        string key = $"CountryGwp::Avg::{country}|{string.Join(',', lobs)}";

        var averages = await cache.GetOrCreateAsync(key, async (_) => await bll.CountGwpAveragesAsync(request.Country, request.Lob));
        return Ok(averages);
    }
}