#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace Galytix.Test.Data.Csv.Configuration;

/// <summary>Configuration options for CSV repository</summary>
public class CsvRepositoryOptions
{
    /// <summary>Gets or sets file path where CSV data are stored</summary>
    /// <remarks>Path can be relative (to application working directory) or absolute</remarks>
    public string FilePath { get; set; }
}
