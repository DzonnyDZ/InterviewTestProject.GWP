using Galytix.Test.Business.Abstractions;
using Galytix.Test.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Galytix.Test.API.Controllers;

[ApiController]
[Route("server/api/gwp")]
public class CountryGwpController : ControllerBase
{
    private readonly ILogger<CountryGwpController> logger;
    private readonly ILobBusiness bll;

    public CountryGwpController(ILobBusiness bll, ILogger<CountryGwpController> logger)
    {
        this.bll = bll ?? throw new ArgumentNullException(nameof(bll));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost("avg", Name = "Avg")]
    [Produces(typeof(IReadOnlyDictionary<string, decimal>))]
    public async Task<IActionResult> AvgAsync([FromBody] LobRequest request)
    {
        var averages = await bll.CountGwpAveragesAsync(request.Country, request.Lob);
        return Ok(averages);
    }
}