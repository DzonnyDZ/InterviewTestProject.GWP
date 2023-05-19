using System.Net;
using System.Text.RegularExpressions;
using Galytix.Test.Business.Abstractions;
using Galytix.Test.Business.Configuration;
using Galytix.Test.Common.Exceptions;
using Galytix.Test.Data.Abstractions;
using Microsoft.Extensions.Options;

namespace Galytix.Test.Business;

/// <summary>Implementation of line of business logic</summary>
internal partial class LobBusiness : ILobBusiness
{
    private static readonly Regex Iso2Regex = GetIso2Regex();

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
        if (country is null) throw new ArgumentNullException(nameof(country));
        if (lobs is null) throw new ArgumentNullException(nameof(lobs));
        if (country.Length != 2)
        {
            throw new ArgumentException("Country name must be 2-letters ISO code, length must be 2 characters", nameof(country))
            { Data = { [ExceptionData.HttpStatus] = HttpStatusCode.BadRequest } };
        }

        if (!Iso2Regex.IsMatch(country))
        {
            throw new ArgumentException("Country name must be 2-letters ISO code, letters must be lowercase a-z", nameof(country))
            { Data = { [ExceptionData.HttpStatus] = HttpStatusCode.BadRequest } };
        }

        if (lobs.Length == 0)
        {
            throw new ArgumentException("At least one line of business must be specified", nameof(lobs))
            { Data = { [ExceptionData.HttpStatus] = HttpStatusCode.BadRequest } };
        }

        if (lobs.Length > lobs.Distinct().Count())
        {
            throw new ArgumentException("Lines of business must be unique", nameof(lobs))
            { Data = { [ExceptionData.HttpStatus] = HttpStatusCode.BadRequest } };
        }

        var ret = await repo.CountAveragesAsync(country, "gwp", lobs, yearFrom, yearTo);
        return ret.Select(kvp => (key: kvp.Key, value: kvp.Value))
                  .Concat(from l in lobs where !ret.ContainsKey(l) select (key: l, value: 0m))
                  .ToDictionary(v => v.key, v => v.value);
    }

    [GeneratedRegex("^[a-z]{2}$", RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex GetIso2Regex();
}
