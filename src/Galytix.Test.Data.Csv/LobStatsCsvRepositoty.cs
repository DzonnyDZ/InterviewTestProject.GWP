using System.Text.RegularExpressions;
using CsvHelper;
using CsvHelper.Configuration;
using Galytix.Test.Common;
using Galytix.Test.Data.Abstractions;
using Galytix.Test.Data.Csv.Configuration;
using Galytix.Test.DataModel;
using Microsoft.Extensions.Options;
using static System.Globalization.CultureInfo;
using IO = System.IO;

namespace Galytix.Test.Data.Csv;

/// <summary>Implements LOBs stats repository over CSV file</summary>
internal partial class LobStatsCsvRepositoty : ILobStatsRepository
{
    /// <summary>A regular expression which matches CSV header for per-year data (like 'Y1234')</summary>
    /// <remarks>The regular expression parses-out single named capture group 'y' containing the year number</remarks>
    private static readonly Regex YearHeaderRegex = GetYearHeaderRegex();

    private LobStat[]? allData;

    /// <summary>Initializes a new instance of the <see cref="LobStatsCsvRepositoty"/> class.</summary>
    /// <param name="options">Configuration options</param>
    public LobStatsCsvRepositoty(IOptions<CsvRepositoryOptions> options)
    {
        if (options is null) throw new ArgumentNullException(nameof(options));
        FilePath = options.Value.FilePath;
    }

    /// <summary>Gets path of file where to load CSV data from</summary>
    /// <remarks>Path can be relative (to application working directory) or absolute</remarks>
    private string FilePath { get; }

    /// <summary>Gets averages per requested country and line(s) of business or given years</summary>
    /// <param name="country">The country to count averages for</param>
    /// <param name="variable">ID of variable to calculate the average for</param>
    /// <param name="lobs">One or more lines of business to count the averages for</param>
    /// <param name="yearFrom">The minimal year to calculate data for (inclusive)</param>
    /// <param name="yearTo">The maximal year to calculate data for (inclusive)</param>
    /// <returns>Averages per lines of business in given country</returns>
    public async Task<IReadOnlyDictionary<string, decimal>> CountAveragesAsync(string country, string variable, string[] lobs, int yearFrom, int yearTo)
    {
        await EnsureDataAsync();

        var lh = new HashSet<string>(lobs);
        var years = Enumerable.Range(yearFrom, yearTo - yearFrom + 1);

        return (from d in allData
                where d.Country == country && lh.Contains(d.LineOfBusiness) && d.VariableId == variable
                let avg = (from y in years select d.Values.TryGetValue(y, out decimal yv) ? yv : 0).Average()
                select (d.LineOfBusiness, avg))
               .ToDictionary(x => x.LineOfBusiness, x => x.avg);
    }

    /// <summary>Makes sure <see cref="allData"/> is initialized</summary>
    /// <returns>Task to await to wait for the asynchronous operation to complete</returns>
    private async Task EnsureDataAsync()
    {
        if (allData == null)
            //Not using lock here because lock cannot be used in async method. The risk is that data are read unnecessarily multiple times form file
            //but always same data should be read
            allData = await LoadAllDataAsync().ToArrayAsync();
    }

    /// <summary>Pre-loads data from CSV</summary>
    /// <returns>Data loaded</returns>
    private async IAsyncEnumerable<LobStat> LoadAllDataAsync()
    {
        using var fs = IO.File.OpenRead(FilePath);
        using var tr = new IO.StreamReader(fs);
        var config = new CsvConfiguration(InvariantCulture) { HasHeaderRecord = true, PrepareHeaderForMatch = a => a.Header.FirstUpperInvariant() };
        using var r = new CsvReader(tr, config);
        var data = new List<LobStat>();
        while (await r.ReadAsync())
        {
            var stat = r.GetRecord<LobStat>()!;
            foreach (string h in r.HeaderRecord!)
            {
                var m = YearHeaderRegex.Match(h);
                if (m.Success && !string.IsNullOrEmpty(r[h]))
                    stat.Values.Add(int.Parse(m.Groups["y"].Value, InvariantCulture), (decimal)double.Parse(r[h]!, InvariantCulture));
            }

            yield return stat;
        }
    }

    /// <summary>Gets a regular expression which matches CSV header for per-year data (like 'Y1234')</summary>
    /// <remarks>The regular expression parses-out single named capture group 'y' containing the year number</remarks>
    [GeneratedRegex("^Y(?<y>[0-9]{4})$", RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex GetYearHeaderRegex();
}
