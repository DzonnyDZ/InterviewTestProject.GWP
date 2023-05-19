using Galytix.Test.Business.Abstractions;
using Galytix.Test.Business.Configuration;
using Galytix.Test.Data.Abstractions;
using Microsoft.Extensions.Options;

namespace Galytix.Test.Business;

/// <summary>Implementation of line of business logic</summary>
internal class LobBusiness : ILobBusiness
{
    private readonly ILobStatsRepository repo;
    private readonly int yearFrom;
    private readonly int yearTo;

    /// <summary>Initializes a new instance of the <see cref="LobBusiness"/> class.</summary>
    /// <param name="repo">Provides access to stored statistics</param>
    /// <param name="options">Configuration options</param>
    public LobBusiness(ILobStatsRepository repo, IOptions<LobStatsBusinessOptions> options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
        yearFrom = options.Value.YearFrom;
        yearTo = options.Value.YearTo;
    }

    /// <summary>Calculates gross written premium (GWP) averages per requested country and line(s) of business</summary>
    /// <param name="country">The country to count averages for</param>
    /// <param name="lobs">One or more lines of business to count the averages for</param>
    /// <returns>Averages per lines of business in given country</returns>
    public async Task<IReadOnlyDictionary<string, decimal>> CountGwpAveragesAsync(string country, string[] lobs)
    {
        var ret = await repo.CountAveragesAsync(country, "gwp", lobs, yearFrom, yearTo);
        return ret;
    }
}
