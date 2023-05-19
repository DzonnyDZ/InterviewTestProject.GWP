namespace Galytix.Test.Business.Abstractions;

/// <summary>LOB statistics business layer</summary>
public interface ILobBusiness
{
    /// <summary>Calculates gross written premium (GWP) averages per requested country and line(s) of business</summary>
    /// <param name="country">The country to count averages for</param>
    /// <param name="lobs">One or more lines of business to count the averages for</param>
    /// <returns>Averages per lines of business in given country</returns>
    Task<IReadOnlyDictionary<string, decimal>> CountGwpAveragesAsync(string country, string[] lobs);
}
