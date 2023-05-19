using Galytix.Test.Business.Abstractions;
using Galytix.Test.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Galytix.Test.API.Controllers;

/// <summary>Provides WebAPI access to gross written premium statistics</summary>
[ApiController]
[Route("server/api/gwp")]
public class CountryGwpController : ControllerBase
{
    private readonly ILogger<CountryGwpController> logger;
    private readonly ILobBusiness bll;

    /// <summary>Initializes a new instance of the <see cref="CountryGwpController"/> class.</summary>
    /// <param name="bll">Business layer</param>
    /// <param name="logger">Logging sink</param>
    public CountryGwpController(ILobBusiness bll, ILogger<CountryGwpController> logger)
    {
        this.bll = bll ?? throw new ArgumentNullException(nameof(bll));
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
        var averages = await bll.CountGwpAveragesAsync(request.Country, request.Lob);
        return Ok(averages);
    }
}