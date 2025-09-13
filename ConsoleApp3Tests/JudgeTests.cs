using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace ConsoleApp3.Tests;

[TestClass()]
public class JudgeTests
{
    [TestMethod]
    [DataRow("aaaaa", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green)]
    [DataRow("abcde", "bccbe", State.Yellow, State.Gray, State.Green, State.Gray, State.Green)]
    [DataRow("ababc", "cbaba", State.Yellow, State.Green, State.Green, State.Green, State.Yellow)]
    [DataRow("abcda", "eaaba", State.Gray, State.Yellow, State.Gray, State.Yellow, State.Green)]
    [DataRow("abcda", "eadbc", State.Gray, State.Yellow, State.Yellow, State.Yellow, State.Yellow)]
    public void CheckTest(string answer, string guess, State s1, State s2, State s3, State s4, State s5)
    {
        var judge = new Judge();
        var state = judge.Check(answer, guess);
        var expected = new State[] { s1, s2, s3, s4, s5 };
        CollectionAssert.AreEqual(expected, state);
    }

    [TestMethod]
    [DataRow("aapas", "audit", State.Green, State.Gray, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("aahed", "audit", State.Green, State.Gray, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("after", "aapas", State.Green, State.Gray, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("aater", "aapas", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("afaer", "aapas", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("aaaaa", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, true)]
    [DataRow("ababa", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, true)]
    [DataRow("abbba", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, false)]
    [DataRow("bbcce", "bbcce", State.Yellow, State.Green, State.Green, State.Gray, State.Gray, true)]
    [DataRow("abcde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, true)]
    [DataRow("accde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, false)]
    [DataRow("aecde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, false)]
    public void RelaxedFulfillTest(string word, string guess, State s1, State s2, State s3, State s4, State s5, bool valid)
    {
        var entry = new Entry(guess, new State[] { s1, s2, s3, s4, s5 });
        var judge = new Judge();
        var actual = judge.Fulfill(entry, word, false, false);
        Assert.AreEqual(valid, actual);
    }

    [TestMethod]
    [DataRow("aapas", "audit", State.Green, State.Gray, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("aahed", "audit", State.Green, State.Gray, State.Gray, State.Gray, State.Gray, false)]
    [DataRow("after", "aapas", State.Green, State.Gray, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("aater", "aapas", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("afaer", "aapas", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("aaaaa", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, true)]
    [DataRow("ababa", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, false)]
    [DataRow("abbba", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, false)]
    [DataRow("bbcce", "bbcce", State.Yellow, State.Green, State.Green, State.Gray, State.Gray, false)]
    [DataRow("abcde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, true)]
    [DataRow("accde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, false)]
    [DataRow("aecde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, false)]
    public void FulfillTest(string word, string guess, State s1, State s2, State s3, State s4, State s5, bool valid)
    {
        var entry = new Entry(guess, new State[] { s1, s2, s3, s4, s5 });
        var judge = new Judge();
        var actual = judge.Fulfill(entry, word);
        Assert.AreEqual(valid, actual);
    }

    [TestMethod]
    [DataRow("blank", "baals", State.Green, State.Yellow, State.Gray, State.Yellow, State.Gray, false)]
    [DataRow("plaza", "aapas", State.Yellow, State.Yellow, State.Yellow, State.Gray, State.Gray, true)]
    [DataRow("after", "aapas", State.Green, State.Gray, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("aater", "aapas", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, false)]
    [DataRow("afaer", "aapas", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, true)]
    [DataRow("aaaaa", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, true)]
    [DataRow("ababa", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, false)]
    [DataRow("adghe", "abcde", State.Green, State.Gray, State.Gray, State.Gray, State.Green, false)]
    [DataRow("aghde", "abcde", State.Green, State.Gray, State.Gray, State.Yellow, State.Green, false)]
    [DataRow("adghe", "abcde", State.Green, State.Gray, State.Gray, State.Yellow, State.Green, true)]
    [DataRow("aighe", "abcde", State.Green, State.Gray, State.Gray, State.Gray, State.Green, true)]
    [DataRow("abbba", "ababa", State.Green, State.Gray, State.Green, State.Gray, State.Green, false)]
    [DataRow("bbcce", "bbcce", State.Yellow, State.Green, State.Green, State.Gray, State.Gray, false)]
    [DataRow("cbcbe", "bbcce", State.Yellow, State.Green, State.Green, State.Gray, State.Gray, false)]
    [DataRow("cbccc", "bbcce", State.Yellow, State.Green, State.Green, State.Gray, State.Gray, false)]
    [DataRow("abcde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, true)]
    [DataRow("accde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, false)]
    [DataRow("aecde", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, false)]
    [DataRow("bdcae", "bcdea", State.Yellow, State.Yellow, State.Yellow, State.Yellow, State.Yellow, false)]
    public void FulfillAsAnswerTest(string word, string guess, State s1, State s2, State s3, State s4, State s5, bool valid)
    {
        var entry = new Entry(guess, new State[] { s1, s2, s3, s4, s5 });
        var judge = new Judge();
        var actual = judge.FulfillAsAnswer(entry, word);
        Assert.AreEqual(valid, actual);
    }

    [TestMethod]
    [DataRow("plate", "plate", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, true, DisplayName = "same")]
    [DataRow("olate", "plate", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, false, DisplayName = "missing green")]
    [DataRow("prate", "plate", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, false, DisplayName = "missing yellow")]
    [DataRow("prale", "plate", State.Green, State.Yellow, State.Gray, State.Gray, State.Gray, true, DisplayName = "yellow changed")]

    [DataRow("blala", "blala", State.Gray, State.Yellow, State.Green, State.Gray, State.Gray, true, DisplayName = "same")]
    [DataRow("blela", "blala", State.Gray, State.Yellow, State.Green, State.Gray, State.Gray, false, DisplayName = "missing green")]
    [DataRow("beala", "blala", State.Gray, State.Yellow, State.Green, State.Gray, State.Gray, true, DisplayName = "yellow changed")]
    
    [DataRow("blala", "blala", State.Gray, State.Yellow, State.Green, State.Yellow, State.Gray, true, DisplayName = "same, two yellow")]
    [DataRow("blaak", "blala", State.Gray, State.Yellow, State.Green, State.Yellow, State.Gray, false, DisplayName = "missing one green")]
    [DataRow("blall", "blala", State.Gray, State.Yellow, State.Green, State.Yellow, State.Gray, true, DisplayName = "more than two greens")]
    
    [DataRow("blala", "blala", State.Gray, State.Yellow, State.Green, State.Gray, State.Yellow, true, DisplayName = "same")]
    [DataRow("balaa", "blala", State.Gray, State.Yellow, State.Green, State.Gray, State.Yellow, false, DisplayName = "missing green")]
    [DataRow("blall", "blala", State.Gray, State.Yellow, State.Green, State.Gray, State.Yellow, false, DisplayName = "missing yellow")]
    [DataRow("bnaal", "blala", State.Gray, State.Yellow, State.Green, State.Gray, State.Yellow, true, DisplayName = "yellow changed")]
    [DataRow("baaal", "blala", State.Gray, State.Yellow, State.Green, State.Gray, State.Yellow, true, DisplayName = "yellow changed")]

    [DataRow("blank", "baals", State.Green, State.Yellow, State.Gray, State.Yellow, State.Gray, true, DisplayName = "yellow changed")]
    [DataRow("plaza", "aapas", State.Yellow, State.Yellow, State.Yellow, State.Gray, State.Gray, true)]
    public void IsLegitimateGuessTest(string word, string guess, State s1, State s2, State s3, State s4, State s5, bool valid)
    {
        var entry = new Entry(guess, new State[] { s1, s2, s3, s4, s5 });
        var judge = new Judge();
        var actual = judge.IsLegitimateGuess(entry, word);
        Assert.AreEqual(valid, actual);
    }
}
