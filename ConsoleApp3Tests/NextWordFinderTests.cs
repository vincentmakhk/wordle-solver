using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Tests;

[TestClass()]
public class NextWordFinderTests
{
    [TestMethod]
    [DataRow("aadrf", -1)]
    [DataRow("acdrf", 1)]
    [DataRow("ecdrf", 1)]
    [DataRow("icdrf", 1)]
    [DataRow("ocdrf", 1)]
    [DataRow("ucdrf", 1)]
    [DataRow("cdrfh", 3)]
    [DataRow("crrfh", 1)]
    public void ScoreTest(string word, int score)
    {
        var nextWordFinder = new NextWordFinder(new Words(), new Judge());
        var result = nextWordFinder.Score(word);
        Assert.AreEqual(score, result);
    }
}