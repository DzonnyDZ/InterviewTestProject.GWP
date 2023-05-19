namespace Galytix.Test.Data.Abstractions;

/// <summary>Repository for accessing line of business statistic data</summary>
public interface ILobStatsRepository
{
    /// <summary>Gets averages per requested country and line(s) of business or given years</summary>
    /// <param name="country">The country to count averages for</param>
    /// <param name="variable">ID of variable to calculate the average for</param>
    /// <param name="lobs">One or more lines of business to count the averages for</param>
    /// <param name="yearFrom">The minimal year to calculate data for (inclusive)</param>
    /// <param name="yearTo">The maximal year to calculate data for (inclusive)</param>
    /// <returns>Averages per lines of business in given country</returns>
    Task<IReadOnlyDictionary<string, decimal>> CountAveragesAsync(string country, string variable, string[] lobs, int yearFrom, int yearTo);
}
