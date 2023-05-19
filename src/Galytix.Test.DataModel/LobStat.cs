namespace Galytix.Test.DataModel;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

/// <summary>STatistic data for Line of Business</summary>
public class LobStat
{
    /// <summary>Gets or sets 2 letter ISO country code (lower case)</summary>
    public string Country { get; set; }

    /// <summary>Gets or sets ID of the variable the statistic is for</summary>
    public string VariableId { get; set; }

    /// <summary>Gets or sets name of the variable the statistic is for</summary>
    public string VariableName { get; set; }

    /// <summary>Gets or sets line of business identifier</summary>
    public string LineOfBusiness { get; set; }

    /// <summary>Gets variable values per year</summary>
    public IDictionary<int, decimal> Values { get; } = new Dictionary<int, decimal>();
}
