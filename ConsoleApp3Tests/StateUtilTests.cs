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
    public class StateUtilTests
    {
        [TestMethod]
        [DataRow("plate", "plate", State.Gray, State.Green, State.Green, State.Gray, State.Green, true)]
        [DataRow("slate", "plate", State.Gray, State.Green, State.Green, State.Gray, State.Green, true)]
        [DataRow("plane", "plate", State.Gray, State.Green, State.Green, State.Gray, State.Green, true)]
        [DataRow("platz", "plate", State.Gray, State.Green, State.Green, State.Gray, State.Green, false)]
        [DataRow("prate", "plate", State.Gray, State.Green, State.Green, State.Gray, State.Green, false)]
        [DataRow("plute", "plate", State.Gray, State.Green, State.Green, State.Gray, State.Green, false)]
        public void IsSamePatternTest(string word, string pattern, State s1, State s2, State s3, State s4, State s5, bool expected)
        {
            var state = new State[] { s1, s2, s3, s4, s5 };
            var actual = StateUtil.IsSamePattern(state, pattern, word);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("plate", "plate", true)]
        [DataRow("plate", "_late", true)]
        [DataRow("plate", "pl__e", true)]
        [DataRow("slate", "____e", true)]
        [DataRow("plate", "_r___", false)]
        [DataRow("plate", "____r", false)]
        [DataRow("plate", "platr", false)]
        public void IsSamePatternTest1(string word, string pattern, bool expected)
        {
            var actual = StateUtil.IsSamePattern(pattern, word);
            Assert.AreEqual(expected, actual);
        }

    }
}