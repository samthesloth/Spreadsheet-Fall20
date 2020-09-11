// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
// Everything after - Sam Peters

using System;
using System.Collections.Generic;
using System.Linq;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    ///
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a
    /// set, and the element is already in the set, the set remains unchanged.
    ///
    /// Given a DependencyGraph DG:
    ///
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)
    ///
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on)
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// Dictionary to hold nodes and their dependees
        /// </summary>
        private Dictionary<string, HashSet<string>> dependees;

        /// <summary>
        /// Dictionary to hold nodes and their dependents
        /// </summary>
        private Dictionary<string, HashSet<string>> dependents;

        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependees = new Dictionary<string, HashSet<string>>();
            dependents = new Dictionary<string, HashSet<string>>();
            Size = 0;
        }

        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get
            {
                CheckValid(s);

                if (dependees.ContainsKey(s))
                    return dependees[s].Count;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            CheckValid(s);

            if (dependents.ContainsKey(s))
                return (dependents[s].Count > 0);
            return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {
            CheckValid(s);

            if (dependees.ContainsKey(s))
                return (dependees[s].Count > 0);
            return false;
        }

        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            CheckValid(s);

            if (dependents.ContainsKey(s))
                return dependents[s];
            else
                return new List<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            CheckValid(s);

            if (dependees.ContainsKey(s))
                return dependees[s];
            else
                return new List<string>();
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        ///
        /// <para>This should be thought of as:</para>
        ///
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>
        public void AddDependency(string s, string t)
        {
            CheckValid(s);
            CheckValid(t);

            if (dependents.ContainsKey(s) && !dependents[s].Contains(t))
            {
                dependents[s].Add(t);

                if (dependees.ContainsKey(t))
                    dependees[t].Add(s);
                else
                {
                    dependees.Add(t, new HashSet<string>());
                    dependees[t].Add(s);
                }

                Size++;
            }
            else if (!dependents.ContainsKey(s))
            {
                dependents.Add(s, new HashSet<string>());
                dependents[s].Add(t);

                if (dependees.ContainsKey(t))
                    dependees[t].Add(s);
                else
                {
                    dependees.Add(t, new HashSet<string>());
                    dependees[t].Add(s);
                }

                Size++;
            }
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            CheckValid(s);
            CheckValid(t);

            if (dependents.ContainsKey(s) && dependents[s].Contains(t))
            {
                dependents[s].Remove(t);
                dependees[t].Remove(s);
                Size--;
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        /// <param name="s"> s is node with dependants being replaced</param>
        /// <param name="newDependents"> Enumerable of new dependents</param>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            CheckValid(s);

            if (!dependents.ContainsKey(s))
                dependents.Add(s, new HashSet<string>());
            else
                foreach (string t in dependents[s].ToList())
                    this.RemoveDependency(s, t);

            foreach (string t in newDependents)
            {
                CheckValid(t);
                this.AddDependency(s, t);
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        /// <param name="s"> s is node with dependees being replaced</param>
        /// <param name="newDependees"> Enumerable of new dependees</param>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            CheckValid(s);

            if (!dependees.ContainsKey(s))
                dependees.Add(s, new HashSet<string>());
            else
                foreach (string t in dependees[s].ToList())
                    this.RemoveDependency(t, s);

            foreach (string t in newDependees)
            {
                CheckValid(t);
                this.AddDependency(t, s);
            }
        }

        /// <summary>
        /// Checks if given string is null or empty, and throws exception if applicable
        /// </summary>
        /// <param name="s"> s is string to be checked</param>
        /// <exception cref="System.ArgumentException">Thrown when null or empty string is given</exception>
        private void CheckValid(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("Method was called with empty or null string");
        }
    }
}