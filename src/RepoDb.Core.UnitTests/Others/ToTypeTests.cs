using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RepoDb.Core.UnitTests.Others;

[TestClass]
public class ToTypeTests
{
    private enum WindDirection
    {
        North,
        East,
        South,
        West,
    }

    [TestMethod]
    public void PrimitiveTypeString()
    {
        Assert.AreEqual(12, Converter.ToType<int>("12"));
        Assert.AreEqual(12L, Converter.ToType<long>("12"));
        Assert.AreEqual(12, Converter.ToType<short>("12"));
        Assert.AreEqual(12, Converter.ToType<sbyte>("12"));
        Assert.AreEqual(12, Converter.ToType<int?>("12"));
        Assert.AreEqual(12L, Converter.ToType<long?>("12"));
        Assert.AreEqual((short)12, Converter.ToType<short?>("12"));
        Assert.AreEqual((sbyte)12, Converter.ToType<sbyte?>("12"));
        Assert.AreEqual("12", Converter.ToType<string>(12));
        Assert.AreEqual("12", Converter.ToType<string>((byte)12));
        Assert.AreEqual("12", Converter.ToType<string>((short)12));

        Assert.AreEqual(WindDirection.East, Converter.ToType<WindDirection>("1"));
        Assert.AreEqual(WindDirection.East, Converter.ToType<WindDirection>(1));
        Assert.AreEqual(WindDirection.East, Converter.ToType<WindDirection?>("1"));
        Assert.AreEqual(WindDirection.East, Converter.ToType<WindDirection?>(1));
    }
}
