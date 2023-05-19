using System.Net;
using Galytix.Test.Business.Configuration;
using Galytix.Test.Common.Exceptions;
using Galytix.Test.Data.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace Galytix.Test.Business.Tests;

/// <summary>Tests for <see cref="LobBusiness"/></summary>
public class LobBusinessTests
{
    private static readonly Random rnd = new();

    private LobStatsBusinessOptions options;
    private Mock<ILobStatsRepository> repoMock;
    private Mock<IOptions<LobStatsBusinessOptions>> optionsMock;
    private LobBusiness inst;

    [SetUp]
    public void Setup()
    {
        repoMock = new();
        optionsMock = new();
        options = new() { YearFrom = 2008, YearTo = 2015 };
        optionsMock.SetupGet(o => o.Value).Returns(options);
        inst = new LobBusiness(repoMock.Object, optionsMock.Object);
    }

    /// <summary>Tests that <see cref="LobBusiness.CountGwpAveragesAsync"/> works normally under standard conditions</summary>
    /// <param name="country">The country to count averages for</param>
    /// <param name="lobs">One or more lines of business to count the averages for</param>
    /// <returns>Task to await to wait for async test to finish</returns>
    [Test]
    [TestCase("bw", "freight")]
    [TestCase("my", "freight", "transport")]
    [TestCase("mt", "fishing", "shipping", "air")]
    public async Task CountGwpAveragesAsync_Success(string country, params string[] lobs)
    {
        //Arrange
        var result = lobs.ToDictionary(l => l, l => (decimal)(rnd.NextDouble() * 10000));
        repoMock.Setup(r => r.CountAveragesAsync(country, "gwp", lobs, options.YearFrom, options.YearTo)).Returns(Task.FromResult((IReadOnlyDictionary<string, decimal>)result));

        //Act
        var res = await inst.CountGwpAveragesAsync(country, lobs);

        //Assert
        Assert.That(res, Is.EqualTo(result));
        repoMock.Verify(r => r.CountAveragesAsync(country, "gwp", lobs, options.YearFrom, options.YearTo), Times.Once());
    }

    [Test]
    [TestCase("my")]
    public void CountGwpAveragesAsync_NoLobRequested_Throws(string country)
    {
        //Act & Assert
        var ex = Assert.ThrowsAsync<ArgumentException>(async () => await inst.CountGwpAveragesAsync(country, Array.Empty<string>()));
        Assert.Multiple(() =>
        {
            Assert.That(ex.ParamName, Is.EqualTo("lobs"));
            Assert.That(ex.Data.Contains(ExceptionData.HttpStatus));
            Assert.That(ex.Data[ExceptionData.HttpStatus], Is.EqualTo(HttpStatusCode.BadRequest));
        });
    }

    /// <summary>Tests that <see cref="LobBusiness.CountGwpAveragesAsync"/> works as expected when one of the LOBs does not have data in repository</summary>
    /// <param name="country">The country to count averages for</param>
    /// <param name="lobOk">The LoB present in the repo</param>
    /// <param name="lobMissing">The LoB not present in the repo</param>
    /// <returns>Task to await to wait for async test to finish</returns>
    [Test]
    [TestCase("bw", "freight", "transport")]
    public async Task CountGwpAveragesAsync_MissingFromRepo(string country, string lobOk, string lobMissing)
    {
        //Arrange
        IReadOnlyDictionary<string, decimal> result = new Dictionary<string, decimal>
        {
            [lobOk] = (decimal)(rnd.NextDouble() * 10000),
            [lobMissing] = 0m
        };
        string[] lobs = new[] { lobOk, lobMissing };
        repoMock.Setup(r => r.CountAveragesAsync(country, "gwp", lobs, options.YearFrom, options.YearTo)).Returns(Task.FromResult(result));

        //Act
        var res = await inst.CountGwpAveragesAsync(country, lobs);

        //Assert
        Assert.That(res, Is.EqualTo(result));
        repoMock.Verify(r => r.CountAveragesAsync(country, "gwp", lobs, options.YearFrom, options.YearTo), Times.Once());
    }
}