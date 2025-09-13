using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp3.Tests
{
    [TestClass()]
    public class WordsTests
    {
        [TestMethod()]
        public void GetValidWords_GivenFirstGreen_Test()
        {
            var words = new Words();
            var judge = new Judge();
            var entry = judge.MakeGuess("audit", "apple");
            var entries = new List<Entry> { entry };
            var validWords = words.GetValidWords(entries, true);
            int count = 0;
            foreach (var w in words.AllValidWords)
            {
                if (w.StartsWith("a") &&
                    !w.Contains('u') &&
                    !w.Contains('d') &&
                    !w.Contains('i') &&
                    !w.Contains('t'))
                {
                    CollectionAssert.Contains(validWords, w, $"Should contain {w}");
                    count++;
                }
                else
                {
                    CollectionAssert.DoesNotContain(validWords, w, $"Should not contain {w}");
                }
            }
            Assert.AreEqual(count, validWords.Count);
        }

        [TestMethod()]
        public void GetValidAnswers_GivenFirstGreen_Test()
        {
            var words = new Words();
            var judge = new Judge();
            var entry = judge.MakeGuess("audit", "apple");
            var entries = new List<Entry> { entry };
            var validAnswers = words.GetValidAnswers(entries, words.AllAnswers);
            int count = 0;
            foreach (var w in words.AllAnswers)
            {
                if (w.StartsWith("a") &&
                    !w.Contains('u') &&
                    !w.Contains('d') &&
                    !w.Contains('i') &&
                    !w.Contains('t'))
                {
                    CollectionAssert.Contains(validAnswers, w);
                    count++;
                }
            }
            Assert.AreEqual(count, validAnswers.Count);
        }

        [TestMethod()]
        public void GetValidAnswers_GivenTwoFirstGreens_Test()
        {
            var words = new Words();
            var judge = new Judge();
            var entry1 = judge.MakeGuess("audit", "apple");
            var entry2 = judge.MakeGuess("aapas", "after");
            var entries = new List<Entry> { entry1, entry2 };
            var validAnswers = words.GetValidAnswers(entries, words.AllAnswers);
            int count = 0;
            foreach (var w in words.AllAnswers)
            {
                if (w.StartsWith("a") &&
                    !w.Skip(1).Contains('a') &&
                    !w.Contains('p') &&
                    !w.Contains('s') &&
                    !w.Contains('u') &&
                    !w.Contains('d') &&
                    !w.Contains('i') &&
                    !w.Contains('t'))
                {
                    CollectionAssert.Contains(validAnswers, w);
                    count++;
                }
                else
                {
                    CollectionAssert.DoesNotContain(validAnswers, w);
                }
            }
            Assert.AreEqual(count, validAnswers.Count);
        }

        [TestMethod()]
        public void GetValidWords_GivenFirstAndSecondYellow_Test()
        {
            var words = new Words();
            var judge = new Judge();
            var entry = judge.MakeGuess("trunk", "shirt");
            var entries = new List<Entry> { entry };
            var validWords = words.GetValidWords(entries, true);
            int count = 0;
            foreach (var w in words.AllValidWords)
            {
                if (w == "trunk")
                    continue;

                if (w.Contains("r") && w.Contains("t") && !w.Contains("u") && !w.Contains("n") && !w.Contains("k"))
                {
                    CollectionAssert.Contains(validWords, w);
                    count++;
                }
            }
            Assert.AreEqual(count, validWords.Count);
        }

        [TestMethod()]
        public void GetValidAnswers_GivenFirstAndSecondYellow_Test()
        {
            var words = new Words();
            var judge = new Judge();
            var entry = judge.MakeGuess("trunk", "shirt");
            var entries = new List<Entry> { entry };
            var validAnswers = words.GetValidAnswers(entries, words.AllAnswers);
            int count = 0;
            foreach (var w in words.AllAnswers)
            {
                if (!w.Contains('u') &&
                    !w.Contains('n') &&
                    !w.Contains('k') &&
                    w.Contains('r') && w[1] != 'r' &&
                    w.Contains('t') && w[0] != 't')
                {
                    CollectionAssert.Contains(validAnswers, w);
                    count++;
                }
            }
            Assert.AreEqual(count, validAnswers.Count);
        }
    }
}