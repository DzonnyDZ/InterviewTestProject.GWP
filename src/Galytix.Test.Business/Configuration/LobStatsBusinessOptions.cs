namespace Galytix.Test.Business.Configuration;

/// <summary>Configuration options for Line of business statistics business layer</summary>
public class LobStatsBusinessOptions
{
    /// <summary>Gets or sets the minimal inclusive year to calculate data for</summary>
    public int YearFrom { get; set; }

    /// <summary>Gets or sets the maximal inclusive year to calculate data for</summary>
    public int YearTo { get; set; }
}
