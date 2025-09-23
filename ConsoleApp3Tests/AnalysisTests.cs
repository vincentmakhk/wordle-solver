using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Tests;

[TestClass()]
public class AnalysisTests
{
    [TestMethod()]
    public void FindMaxDistinctTest()
    {
        var analysis = new Analysis(new Judge(), new Words());
        var max = analysis.FindMaxDistinct("ab___", new List<string>
        {
            "ababc",
            "abbcd",
            "abcde",
            "abefg",
        });
        Assert.AreEqual(2, max);
    }

    [TestMethod()]
    public void FindMaxDistinctTest_GivenAllDistinct()
    {
        var analysis = new Analysis(new Judge(), new Words());
        var max = analysis.FindMaxDistinct("abcd_", new List<string>
        {
            "abcde",
            "abcdf",
            "abcdh",
            "abcdi",
        });
        Assert.AreEqual(4, max);
    }
}