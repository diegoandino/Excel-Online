using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;


namespace DevelopmentTests
{
    /**
    *  Diego Andino
    *  September 13, 2020
    */

    /// <summary>
    ///This is a test class for DependencyGraphTest and is intended
    ///to contain all DependencyGraphTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DependencyGraphTest
    {

        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyTest()
        {
            DependencyGraph t = new DependencyGraph();
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void SimpleEmptyRemoveTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(1, t.Size);
            t.RemoveDependency("x", "y");
            Assert.AreEqual(0, t.Size);
        }


        /// <summary>
        ///Empty graph should contain nothing
        ///</summary>
        [TestMethod()]
        public void EmptyEnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            IEnumerator<string> e1 = t.GetDependees("y").GetEnumerator();
            Assert.IsTrue(e1.MoveNext());
            Assert.AreEqual("x", e1.Current);
            IEnumerator<string> e2 = t.GetDependents("x").GetEnumerator();
            Assert.IsTrue(e2.MoveNext());
            Assert.AreEqual("y", e2.Current);
            t.RemoveDependency("x", "y");
            Assert.IsFalse(t.GetDependees("y").GetEnumerator().MoveNext());
            Assert.IsFalse(t.GetDependents("x").GetEnumerator().MoveNext());
        }


        /// <summary>
        ///Replace on an empty DG shouldn't fail
        ///</summary>
        [TestMethod()]
        public void SimpleReplaceTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "y");
            Assert.AreEqual(t.Size, 1);
            t.RemoveDependency("x", "y");
            t.ReplaceDependents("x", new HashSet<string>());
            t.ReplaceDependees("y", new HashSet<string>());
        }


        ///<summary>
        ///It should be possibe to have more than one DG at a time.
        ///</summary>
        [TestMethod()]
        public void StaticTest()
        {
            DependencyGraph t1 = new DependencyGraph();
            DependencyGraph t2 = new DependencyGraph();
            t1.AddDependency("x", "y");
            Assert.AreEqual(1, t1.Size);
            Assert.AreEqual(0, t2.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");
            Assert.AreEqual(4, t.Size);
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void EnumeratorTest()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("a", "b");
            t.AddDependency("a", "c");
            t.AddDependency("c", "b");
            t.AddDependency("b", "d");

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }


        /// <summary>
        ///Non-empty graph contains something
        ///</summary>
        [TestMethod()]
        public void ReplaceThenEnumerate()
        {
            DependencyGraph t = new DependencyGraph();
            t.AddDependency("x", "b");
            t.AddDependency("a", "z");
            t.ReplaceDependents("b", new HashSet<string>());
            t.AddDependency("y", "b");
            t.ReplaceDependents("a", new HashSet<string>() { "c" });
            t.AddDependency("w", "d");
            t.ReplaceDependees("b", new HashSet<string>() { "a", "c" });
            t.ReplaceDependees("d", new HashSet<string>() { "b" });

            IEnumerator<string> e = t.GetDependees("a").GetEnumerator();
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("b").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            String s1 = e.Current;
            Assert.IsTrue(e.MoveNext());
            String s2 = e.Current;
            Assert.IsFalse(e.MoveNext());
            Assert.IsTrue(((s1 == "a") && (s2 == "c")) || ((s1 == "c") && (s2 == "a")));

            e = t.GetDependees("c").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("a", e.Current);
            Assert.IsFalse(e.MoveNext());

            e = t.GetDependees("d").GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual("b", e.Current);
            Assert.IsFalse(e.MoveNext());
        }


        /// <summary>
        ///Using lots of data
        ///</summary>
        [TestMethod()]
        public void StressTest()
        {
            // Dependency graph
            DependencyGraph t = new DependencyGraph();

            // A bunch of strings to use
            const int SIZE = 200;
            string[] letters = new string[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                letters[i] = ("" + (char)('a' + i));
            }

            // The correct answers
            HashSet<string>[] dents = new HashSet<string>[SIZE];
            HashSet<string>[] dees = new HashSet<string>[SIZE];
            for (int i = 0; i < SIZE; i++)
            {
                dents[i] = new HashSet<string>();
                dees[i] = new HashSet<string>();
            }

            // Add a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j++)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove a bunch of dependencies
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 4; j < SIZE; j += 4)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Add some back
            for (int i = 0; i < SIZE; i++)
            {
                for (int j = i + 1; j < SIZE; j += 2)
                {
                    t.AddDependency(letters[i], letters[j]);
                    dents[i].Add(letters[j]);
                    dees[j].Add(letters[i]);
                }
            }

            // Remove some more
            for (int i = 0; i < SIZE; i += 2)
            {
                for (int j = i + 3; j < SIZE; j += 3)
                {
                    t.RemoveDependency(letters[i], letters[j]);
                    dents[i].Remove(letters[j]);
                    dees[j].Remove(letters[i]);
                }
            }

            // Make sure everything is right
            for (int i = 0; i < SIZE; i++)
            {
                Assert.IsTrue(dents[i].SetEquals(new HashSet<string>(t.GetDependents(letters[i]))));
                Assert.IsTrue(dees[i].SetEquals(new HashSet<string>(t.GetDependees(letters[i]))));
            }
        }


        /// <summary>
        /// Checks if it throws exception when passed in a null argument.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddOneNullDependency()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("a", null);
        }
        
        
        /// <summary>
        /// Checks if it throws exception when passed in null to both arguments.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddTwoNullDependency()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency(null, null);
        }
        
        
        /// <summary>
        /// Checks if it returns an empty HashSet when passed in empty string.
        /// </summary>
        [TestMethod()]
        public void GetEmptyStringDependencies()
        {
            DependencyGraph t = new DependencyGraph();
            HashSet<string> list = new HashSet<string>();
            t.AddDependency("d", "h");
            t.AddDependency("b", "");
            t.AddDependency("", "g"); 

            
            Assert.IsTrue(list.SetEquals(t.GetDependents("b")));
            Assert.IsTrue(list.SetEquals(t.GetDependees("g")));
        }


        /// <summary>
        /// Checks if Get Dependents returns a list of items.
        /// </summary>
        [TestMethod()]
        public void GetDependentsExists()
        {
            DependencyGraph t = new DependencyGraph();
            HashSet<string> emptySet = new HashSet<string>();
            HashSet<string> actualSet = new HashSet<string>() { "a" };

            t.AddDependency("a", "a");
            t.AddDependency("b", "l");

            Assert.IsFalse(emptySet.SetEquals(t.GetDependents("b")));
            Assert.IsTrue(actualSet.SetEquals(t.GetDependents("a")));
        }


        /// <summary>
        /// Checks if Get Dependents throws an ArgumentNullException when
        /// passed in a null value.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDependentsNullException()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("h", "l");
            t.GetDependents(null);
        }


        /// <summary>
        /// Checks if Get Dependees returns a list of items.
        /// </summary>
        [TestMethod()]
        public void GetDependeesExists()
        {
            DependencyGraph t = new DependencyGraph();
            HashSet<string> emptySet = new HashSet<string>();
            HashSet<string> actualSet = new HashSet<string>() { "d" };

            t.AddDependency("d", "h");
            t.AddDependency("b", "l");

            Assert.IsFalse(emptySet.SetEquals(t.GetDependees("h")));
            Assert.IsTrue(actualSet.SetEquals(t.GetDependees("h")));
        }

        
        /// <summary>
        /// Checks if Get Dependees throws an ArgumentNullException when
        /// passed in a null value.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDependeesNullException()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("h", "l");
            t.GetDependees(null);
        }

        /// <summary>
        /// Checks if Has Dependents returns true.
        /// </summary>
        [TestMethod()]
        public void HasDependents()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("a", "h");
            t.AddDependency("a", "b");
            t.AddDependency("b", "");

            Assert.IsTrue(t.HasDependents("a"));
            Assert.IsTrue(t.HasDependents("b"));
        }


        /// <summary>
        /// Checks if Has Dependents returns false.
        /// </summary>
        [TestMethod()]
        public void NoDependents()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("h", "t");
            t.RemoveDependency("h", "t");

            Assert.IsFalse(t.HasDependents("h"));
        }
        
        
        /// <summary>
        /// Checks if RemoveDependents throws an ArgumentNullException when
        /// passed in a null value. 
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RemoveDependentsException()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("g", "k");
            t.RemoveDependency(null, "t");
        }


        /// <summary>
        /// Checks if Has Dependents throws ArgumentNullException
        /// for null argument.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasDependentsException()
        {
            DependencyGraph t = new DependencyGraph();

            t.HasDependents(null);
        }


        /// <summary>
        /// Checks if Has Dependees returns true.
        /// </summary>
        [TestMethod()]
        public void HasDependees()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("d", "h");
            t.AddDependency("", "l");

            Assert.IsTrue(t.HasDependees("h"));
            Assert.IsTrue(t.HasDependees("l"));
        }


        /// <summary>
        /// Checks if Has Dependees returns false.
        /// </summary>
        [TestMethod()]
        public void NoDependees()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("h", "t");
            t.RemoveDependency("h", "t");

            Assert.IsFalse(t.HasDependees("t"));
        }


        /// <summary>
        /// Checks if Has Dependees throws ArgumentNullException
        /// for null argument.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HasDependeesException()
        {
            DependencyGraph t = new DependencyGraph();

            t.HasDependees(null);
        }


        /// <summary>
        /// Checks if t[...] gets the correct number
        /// of dependees using the t[] notation
        /// where t is a Dependency Graph.
        /// </summary>
        [TestMethod()]
        public void NumberOfDependees()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("a", "h");
            t.AddDependency("a", "b");
            t.AddDependency("", "b");

            int actual = t["b"];
            int expected = 2;

            Assert.AreEqual(expected, actual);
        }


        /// <summary>
        /// Checks if Replace Dependents throws ArgumentNullException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependentsException()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("a", "b");

            t.ReplaceDependents(null, null);
        }


        /// <summary>
        /// Checks if Replace Dependees throws ArgumentNullException.
        /// </summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ReplaceDependeesException()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("a", "b");

            t.ReplaceDependees(null, null);
        }


        /// <summary>
        /// Checks that when replacing dependents it replaces the dependees
        /// correctly as well as getting rid of any other unnecessary dependencies.
        /// 
        /// For example, when replacing dependents of "x", size should go from 
        /// 6 to 4, "x" will only point to "b", and the variables: "g", "z", "d"
        /// no longer point to "x" in dependees but still keep their other dependencies. 
        /// </summary>
        [TestMethod()]
        public void ReplaceDependentsSpecialCase()
        {
            DependencyGraph t = new DependencyGraph();

            t.AddDependency("x", "g");
            t.AddDependency("a", "z");
            t.AddDependency("a", "b");
            t.AddDependency("b", "h");
            t.AddDependency("x", "z");
            t.AddDependency("x", "d");
            t.ReplaceDependents("x", new HashSet<string>() {"b"});

            Assert.IsTrue(t.GetDependees("b").Contains("x"));
            Assert.IsTrue(!t.GetDependees("g").Contains("x"));
            Assert.IsTrue(!t.GetDependees("z").Contains("x"));
            Assert.IsTrue(!t.GetDependees("d").Contains("x"));

            Assert.IsTrue(t.GetDependents("a").Contains("z"));
            Assert.IsTrue(t.GetDependents("a").Contains("b"));
            Assert.IsTrue(t.GetDependents("b").Contains("h"));
            Assert.IsTrue(t.GetDependents("x").Contains("b"));

            Assert.IsTrue(t.Size == 4);
        }
    }
}