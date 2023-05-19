namespace Galytix.Test.Dto;

/// <summary>Request to provide data per country and Line of Business</summary>
public class LobRequest
{
    /// <summary>Gets or sets 2-letters country ISO code (lowercase)</summary>
    public string Country { get; set; }

    /// <summary>Gets or sets array of lines of business to get data for</summary>
    public string[] Lob { get; set; }
}
