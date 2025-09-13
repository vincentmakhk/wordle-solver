using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3.Tests
{
    [TestClass()]
    public class WordUtilTests
    {
        [TestMethod]
        [DataRow("abcde", State.Gray, State.Green, State.Green, State.Gray, State.Gray, "_bc__")]
        [DataRow("abcde", State.Green, State.Gray, State.Gray, State.Gray, State.Gray, "a____")]
        [DataRow("abcde", State.Gray, State.Gray, State.Gray, State.Gray, State.Green, "____e")]
        public void GeneratePrefixTest(string word, State s1, State s2, State s3, State s4, State s5, string prefix)
        {
            var result = WordUtil.GeneratePrefix(word, new State[] { s1, s2, s3, s4, s5 });
            Assert.AreEqual(prefix, result);
        }

        [TestMethod]
        [DataRow("a____", "abcde", "bcde")]
        [DataRow("_d_e_", "adief", "aif")]
        [DataRow("bcde_", "bcdef", "f")]
        public void StripTest(string pattern, string word, string expected)
        {
            var result = WordUtil.Strip(pattern, word);
            Assert.AreEqual(expected, result);
        }
    }
}