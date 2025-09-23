using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Tests;

[TestClass()]
public class WordGrouperTests
{
    [TestMethod()]
    public void GroupBySameStateTest()
    {
        var wordGrouper = new WordGrouper(new Words(), new Judge());
        var result = wordGrouper.GroupBySameState(new string[]
            {
                "abcde", "abmno", "abcmn", "accde"
            },
            new State[] { State.Green, State.Green, State.Gray, State.Gray, State.Gray });
        Assert.AreEqual(3, result["ab___"].Count);
        Assert.AreEqual(1, result["ac___"].Count);
        Assert.AreEqual(2, result.Count);
    }
}