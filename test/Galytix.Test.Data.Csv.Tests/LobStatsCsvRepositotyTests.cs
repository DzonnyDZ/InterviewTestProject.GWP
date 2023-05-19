using System.IO.Abstractions;
using Galytix.Test.Data.Csv.Configuration;
using Microsoft.Extensions.Options;
using Moq;

namespace Galytix.Test.Data.Csv.Tests;

/// <summary>Tests for <see cref="LobStatsCsvRepositotyTests"/></summary>
public class LobStatsCsvRepositotyTests
{
    private LobStatsCsvRepositoty inst;
    private CsvRepositoryOptions options;
    private Mock<IOptions<CsvRepositoryOptions>> optionsMock;
    private Mock<IFileSystem> fileSystemMock;
    private Mock<IFile> fileMock;

    [SetUp]
    public void Setup()
    {
        options = new() { FilePath = "not important.csv" };
        optionsMock = new();
        optionsMock.SetupGet(o => o.Value).Returns(options);
        fileSystemMock = new Mock<IFileSystem>();
        fileMock = new Mock<IFile>();
        fileSystemMock.SetupGet(fs => fs.File).Returns(fileMock.Object);
        inst = new(fileSystemMock.Object, optionsMock.Object);
    }

    private class InternalStream : FileSystemStream
    {
        public InternalStream(Stream stream, string path) : base(stream, path, true) { }
    }

    [Test]
    public async Task LoadAllDataAsync_Success()
    {
        //Arrange
        const string csv = @"country,variableId,variableName,lineOfBusiness,Y1994,Y1998,Y2000,Y2015,Y2020,Y2110
ba,xx,Unknown,smugglers,,2000,1990,1803.2,1900.2,
cz,gwp,""What, we want"",traffic,6.2E-5,0.03,1,2,2.5,130";
        using var ms = await SetupFileMockAsync(csv);

        //Act
        var data = await inst.LoadAllDataAsync().ToArrayAsync();

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Length.EqualTo(2));

            Assert.That(data[0].Country, Is.EqualTo("ba"));
            Assert.That(data[0].VariableId, Is.EqualTo("xx"));
            Assert.That(data[0].VariableName, Is.EqualTo("Unknown"));
            Assert.That(data[0].LineOfBusiness, Is.EqualTo("smugglers"));
            Assert.That(data[0].Values, Has.Count.EqualTo(4));
            Assert.That(data[0].Values.ContainsKey(1998));
            Assert.That(data[0].Values[1998], Is.EqualTo(2000m));
            Assert.That(data[0].Values.ContainsKey(2000));
            Assert.That(data[0].Values[2000], Is.EqualTo(1990m));
            Assert.That(data[0].Values.ContainsKey(2015));
            Assert.That(data[0].Values[2015], Is.EqualTo(1803.2m));
            Assert.That(data[0].Values.ContainsKey(2020));
            Assert.That(data[0].Values[2020], Is.EqualTo(1900.2m));

            Assert.That(data[1].Country, Is.EqualTo("cz"));
            Assert.That(data[1].VariableId, Is.EqualTo("gwp"));
            Assert.That(data[1].VariableName, Is.EqualTo("What, we want"));
            Assert.That(data[1].LineOfBusiness, Is.EqualTo("traffic"));
            Assert.That(data[1].Values, Has.Count.EqualTo(6));
            Assert.That(data[1].Values.ContainsKey(1994));
            Assert.That(data[1].Values[1994], Is.EqualTo(6.2E-5m));
            Assert.That(data[1].Values.ContainsKey(1998));
            Assert.That(data[1].Values[1998], Is.EqualTo(0.03m));
            Assert.That(data[1].Values.ContainsKey(2000));
            Assert.That(data[1].Values[2000], Is.EqualTo(1m));
            Assert.That(data[1].Values.ContainsKey(2015));
            Assert.That(data[1].Values[2015], Is.EqualTo(2m));
            Assert.That(data[1].Values.ContainsKey(2020));
            Assert.That(data[1].Values[2020], Is.EqualTo(2.5m));
            Assert.That(data[1].Values.ContainsKey(2110));
            Assert.That(data[1].Values[2110], Is.EqualTo(130m));
        });
    }

    private const string TestDataCsv = @"country,variableId,variableName,lineOfBusiness,Y2000,Y2001,Y2002,Y2003,Y2004,Y2005,Y2006,Y2007,Y2008,Y2009,Y2010,Y2011,Y2012,Y2013,Y2014,Y2015
ae,gwp,Direct Premiums,transport,    ,    ,     ,     ,     ,    ,     , 231441262.7, 268744928.7, 284448918.2, 314413884.1, 327740154.4, 326126300.6, 240322742.6, 234164748.7,
ae,gwp,Direct Premiums,freight  ,    ,     ,     ,     ,     ,     ,     , 217119663.1, 252114975.9, 266847201.6, 294957933.5, 307459573.3, 305945585  , 225451556.4, 219674619.6,
ao,gwp,Direct Premiums,transport,    ,     ,     ,     ,     ,     ,     ,            , 42327844.88, 23032172.17, 62974314.17,            ,            ,            ,            ,
ao,gwp,Direct Premiums,property ,    ,     ,     ,     ,     ,     ,     ,            , 167698763.3, 310172299.8, 231376228.2,            ,            ,            ,            ,
ao,gwp,Direct Premiums,liability,    ,     ,     ,     ,     ,     ,     ,            , 14778761.71, 17701152.73, 33597471.76,            ,            ,            ,            ,";
    //                           2000, 2001, 2002, 2003, 2004, 2005, 2006, 2007       , 2008       , 2009       , 2010       , 2011       , 2012       , 2013       , 2014       , 2015
    //                                                                              ╰─────────────────────────────────────────────────────────────────────────────────────────╯

    [Test]
    [TestCase("ae", new[] { "transport", "freight", "property" }, new double[] { 249495209.6625, 234056430.6625, -1 })]
    [TestCase("ae", new[] { "transport" }, new double[] { 249495209.6625 })]
    [TestCase("ao", new[] { "transport", "xxx" }, new double[] { 16041791.4025, -1 })]
    [TestCase("ao", new[] { "property", "liability" }, new double[] { 88655911.4125, 8259673.275 })]
    public async Task CountAveragesAsync(string country, string[] lobs, double[] expected)
    {
        //Arrange
        using var ms = await SetupFileMockAsync(TestDataCsv);
        var ex = (from v in expected select v < 0 ? default(decimal?) : (decimal)v).ToArray();

        //Act
        var res = await inst.CountAveragesAsync(country, "gwp", lobs, 2008, 2015);

        //Assert
        Assert.Multiple(() =>
        {
            for (int i = 0; i < lobs.Length; i++)
                if (ex[i].HasValue)
                    Assert.That(res[lobs[i]], Is.EqualTo(ex[i]));
                else
                    Assert.That(!res.ContainsKey(lobs[i]));
        });
    }

    private async Task<MemoryStream> SetupFileMockAsync(string csv)
    {
        var ms = new MemoryStream();
        using (var sw = new StreamWriter(ms, leaveOpen: true))
        {
            await sw.WriteAsync(csv);
        }
        ms.Seek(0, SeekOrigin.Begin);
        fileMock.Setup(f => f.OpenRead(options.FilePath)).Returns(new Func<FileSystemStream>(() => new InternalStream(ms, options.FilePath)));
        return ms;
    }
}