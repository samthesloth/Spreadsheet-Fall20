//Author - Sam Peters

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentTests
{
    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MyDependencyGraphTest
    {
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void SimpleNullTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency(null, "hi");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void ReplaceDependentsNullTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("A", "B");
            List<string> list = new List<string>();
            list.Add("W");
            list.Add(null);
            t.ReplaceDependents("A", list);
        }

        [TestMethod()]
        public void HasDependentsAndSizeAfterRemove()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("A", "B");
            Assert.AreEqual(1, t.Size);
            Assert.IsTrue(t.HasDependents("A"));
            t.RemoveDependency("A", "B");
            Assert.AreEqual(0, t.Size);
            Assert.IsFalse(t.HasDependents("A"));
        }

        [TestMethod()]
        public void SimpleIndexer()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("A", "B");
            t.AddDependency("C", "B");
            Assert.AreEqual(2, t["B"]);
            t.RemoveDependency("C", "B");
            Assert.AreEqual(1, t["B"]);
            t.RemoveDependency("A", "B");
            Assert.AreEqual(0, t["B"]);
        }

        [TestMethod()]
        public void IndexerWithUndefinedString()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t["A"]);
        }

        [TestMethod()]
        public void TrickingRemovalWithInvalidPair()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("A", "B");
            t.AddDependency("C", "B");
            Assert.AreEqual(2, t["B"]);
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("B", "C");
            Assert.AreEqual(2, t["B"]);
            Assert.AreEqual(2, t.Size);
            t.RemoveDependency("B", "A");
            Assert.AreEqual(2, t["B"]);
            Assert.AreEqual(2, t.Size);
        }

        [TestMethod()]
        public void SimpleHasDependents()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("A", "B");
            t.AddDependency("C", "B");
            Assert.IsTrue(t.HasDependents("A"));
            Assert.IsTrue(t.HasDependents("C"));
            Assert.IsFalse(t.HasDependents("B"));
            t.RemoveDependency("A", "B");
            t.RemoveDependency("C", "B");
            t.AddDependency("B", "A");
            Assert.IsFalse(t.HasDependents("A"));
            Assert.IsFalse(t.HasDependents("C"));
            Assert.IsTrue(t.HasDependents("B"));
        }

        [TestMethod()]
        public void SimpleHasDependees()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("A", "B");
            t.AddDependency("C", "B");
            Assert.IsFalse(t.HasDependees("A"));
            Assert.IsFalse(t.HasDependees("C"));
            Assert.IsTrue(t.HasDependees("B"));
            t.RemoveDependency("A", "B");
            t.RemoveDependency("C", "B");
            t.AddDependency("B", "A");
            t.AddDependency("B", "C");
            Assert.IsTrue(t.HasDependees("A"));
            Assert.IsTrue(t.HasDependees("C"));
            Assert.IsFalse(t.HasDependees("B"));
        }

        [TestMethod()]
        public void ReplaceDependentsWithUndefinedString()
        {
            DependencyGraph t = new DependencyGraph();
            List<string> list = new List<string>();
            list.Add("A");
            list.Add("C");
            list.Add("E");
            t.ReplaceDependents("B", list);
            Assert.IsTrue(t.HasDependents("B"));
            Assert.IsFalse(t.HasDependees("B"));
            Assert.AreEqual(0, t["B"]);
            List<string> result = t.GetDependents("B").ToList();
            foreach (string s in list)
                Assert.IsTrue(result.Contains(s));
        }

        [TestMethod()]
        public void ReplaceDependeesWithUndefinedString()
        {
            DependencyGraph t = new DependencyGraph();
            List<string> list = new List<string>();
            list.Add("A");
            list.Add("C");
            list.Add("E");
            t.ReplaceDependees("B", list);
            Assert.IsTrue(t.HasDependees("B"));
            Assert.IsFalse(t.HasDependents("B"));
            Assert.AreEqual(3, t["B"]);
            List<string> result = t.GetDependees("B").ToList();
            foreach (string s in list)
                Assert.IsTrue(result.Contains(s));
        }
    }
}