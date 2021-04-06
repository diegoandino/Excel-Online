using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FormulaTests
{
    /*
     * Diego Andino
     * September 19th, 2020
     */

    /// <summary>
    /// MSTest class to test the Formula class.
    /// </summary>
    [TestClass]
    public class FormulaTests
    {
        /// <summary>
        /// Evaluates a simple addition. 
        /// </summary>
        [TestMethod]
        public void SimpleAdditionFormulaEvaluation()
        {
            Assert.AreEqual(20.0, new Formula("x + 10").Evaluate(LookUp));
        }


        /// <summary>
        /// Evaluates a simple substraction. 
        /// </summary>
        [TestMethod]
        public void SimpleSubstractionFormulaEvaluation()
        {
            Assert.AreEqual(0.0, new Formula("x - 10").Evaluate(LookUp));
        }

        /// <summary>
        /// Evaluates a simple multiplication. 
        /// </summary>
        [TestMethod]
        public void SimpleMultiplicationFormulaEvaluation()
        {
            Assert.AreEqual(100.0, new Formula("x * 10").Evaluate(LookUp));
        }


        /// <summary>
        /// Evaluates a simple division. 
        /// </summary>
        [TestMethod]
        public void SimpleDivisionFormulaEvaluation()
        {
            Assert.AreEqual(1.0, new Formula("x / 10").Evaluate(LookUp));
        }


        /// <summary>
        /// Evaluates a more standard formula. 
        /// </summary>
        [TestMethod]
        public void RegularFormulaEvaluation()
        {
            Assert.AreEqual(1.0, new Formula("20 * 2 / (80 - 40)").Evaluate(LookUp));
        }
        
        
        /// <summary>
        /// Evaluates a standard formula. 
        /// </summary>
        [TestMethod]
        public void RegularFormulaEvaluation2()
        {
            Assert.AreEqual(62.0, new Formula("20 + 2 + (80 - 40)").Evaluate(LookUp));
        }
        
        
        /// <summary>
        /// Evaluates a standard formula. 
        /// </summary>
        [TestMethod]
        public void RegularFormulaEvaluation3()
        {
            Assert.AreEqual(1.0, new Formula("x * x / (x * x)").Evaluate(LookUp));
        }


        /// <summary>
        /// Evaluates a longer standard formula. 
        /// </summary>
        [TestMethod]
        public void LongerRegularFormulaEvaluation()
        {
            Assert.AreEqual(-5300.0, new Formula("(20 / 2 * (190 - 40 * (9 + 9)))").Evaluate(LookUp));
        }


        /// <summary>
        /// Evaluates a longer standard formula. 
        /// </summary>
        [TestMethod]
        public void ComplexFormulaEvaluation()
        {
            Assert.AreEqual(1100000000000.0, new Formula("1e8 * 110e2 + (10 / 3e10)").Evaluate(LookUp));
        }


        /// <summary>
        /// Evaluates a longer standard formula. 
        /// </summary>
        [TestMethod]
        public void LongDoubleFormulaEvaluation()
        {
            Assert.AreEqual(3.0, new Formula("(4.0 * 2 + (10.0 / (10.0 + (2 - 2)))) / 3").Evaluate(LookUp));
        }


        /// <summary>
        /// Tests dividing by zero.
        /// </summary>
        [TestMethod]
        public void DivideByZeroException()
        {
            Formula f = new Formula("100 / 0");
            Assert.IsInstanceOfType(f.Evaluate(s => 0), typeof(FormulaError));
        }
        
        
        /// <summary>
        /// Tests null lookup.
        /// </summary>
        [TestMethod]
        public void CheckForNullLookUp()
        {
            Formula f = new Formula("100 / 100");
            Assert.IsInstanceOfType(f.Evaluate(null), typeof(FormulaError));

            FormulaError err = new FormulaError();
            string reason = err.Reason;
        }
        
        
        /// <summary>
        /// Tests invalid formulas.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidFormulaCheck()
        {
            Formula f1 = new Formula("100 + ");
            Formula f2 = new Formula("100 * + (20)");
            Formula f3 = new Formula("* + (20)");
        }


        /// <summary>
        /// Tests the Get Variables method in the Formula class. 
        /// </summary>
        [TestMethod]
        public void GetVariablesFromFormula()
        {
            Formula f = new Formula("X1 + 20 * (B2 + _D10) / a_10");

            List<string> actual = new List<string>(f.GetVariables());
            List<string> expected = new List<string>() { "X1", "B2", "_D10", "a_10" };

            Assert.IsTrue(expected.SequenceEqual(actual));
        }


        /// <summary>
        /// Tests the Get Variables method in the Formula class.
        /// Checks for normalized variables.
        /// </summary>
        [TestMethod]
        public void GetNormalizedVariablesFromFormula()
        {
            Formula f = new Formula("x1 + 20 * (B2 + _d10) / a_10", N, V);

            List<string> actual = new List<string>(f.GetVariables());
            List<string> expected = new List<string>() { "X1", "B2", "_D10", "A_10" };

            Assert.IsTrue(expected.SequenceEqual(actual));
        }


        /// <summary>
        /// Tests the Equals method for the Formula class. 
        /// </summary>
        [TestMethod]
        public void CheckFormulaEquality()
        {
            Formula f0 = new Formula("x1 + 20 * (B2 + _d10) / a_10", N, V);
            Formula f1 = new Formula("x1 + 20 * (B2 + _d10) / a_10", N, V);

            Assert.IsTrue(f0.Equals(f1));
        }


        /// <summary>
        /// Tests inequality for Equals method.
        /// </summary>
        [TestMethod]
        public void CheckFormulaInequality()
        {
            Formula f0 = new Formula("B1 - 90 * (B2 + _d10) / Y_10", N, V);
            Formula f1 = new Formula("x1 + 20 * (B2 + _d10) / a_10", N, V);

            Assert.IsFalse(f0.Equals(f1));
        }


        /// <summary>
        /// Tests inequality with Equals method for unequal parenthesis.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidParenthesis()
        {
            Formula f0 = new Formula("x1 + 20 * (B2 + _d10) / a_10)", N, V);
        }
        
        
        /// <summary>
        /// Tests inequality with Equals method for unequal parenthesis.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidParenthesis2()
        {
            Formula f0 = new Formula("x1 + 20 * ((B2 + _d10))))) / a_10)", N, V);
        }


        /// <summary>
        /// Tests the Equals method with scientific notation for the Formula class. 
        /// </summary>
        [TestMethod]
        public void ScientificNotationEvaluationEquality()
        {
            Formula f0 = new Formula("1e10 + 1e10", N, V);
            Formula f1 = new Formula("1e10 + 1e10", N, V);

            Assert.IsTrue(f0.Equals(f1));
        }


        /// <summary>
        /// Tests the Equals method with scientific notation for the Formula class. 
        /// </summary>
        [TestMethod]
        public void ScientificNotationEvaluationEquality2()
        {
            Formula f0 = new Formula("1e10 + 1e10", N, V);
            Formula f1 = new Formula("10000000000 + 10000000000");

            Assert.IsTrue(f0.Equals(f1));
        }


        /// <summary>
        /// Tests the Equals method with scientific notation for the Formula class. 
        /// </summary>
        [TestMethod]
        public void ScientificNotationEvaluationEquality3()
        {
            Formula f0 = new Formula("1e10 + 1e10", N, V);
            Formula f1 = f0;

            Assert.IsTrue(f0.Equals(f1));
        }


        /// <summary>
        /// Tests the Equals and ToString methods with scientific notation for the Formula class. 
        /// </summary>
        [TestMethod]
        public void CheckFormulaEqualityScientificNotationVariant()
        {
            Formula f0 = new Formula("1e10 + 1e10", N, V);
            Formula f1 = new Formula("10000000000 + 10000000000", N, V);

            Assert.IsTrue(f0.ToString().Equals(f1.ToString()));
        }


        /// <summary>
        /// Tests the GetHashCode method for the Formula class. 
        /// </summary>
        [TestMethod]
        public void CheckGetHashCodeEquality()
        {
            Formula f0 = new Formula("1e10 + 1e10", N, V);
            Formula f1 = new Formula("10000000000 + 10000000000", N, V);

            Assert.IsTrue(f0.GetHashCode().Equals(f1.GetHashCode()));
        }


        /// <summary>
        /// Tests the GetHashCode and ToString methods together for the Formula class. 
        /// </summary>
        [TestMethod]
        public void CheckGetHashCodeEquality2()
        {
            Formula f0 = new Formula("1e10 + 1e10", N, V);
            Formula f1 = new Formula("10000000000 + 10000000000", N, V);

            Assert.IsTrue(f0.ToString().GetHashCode().Equals(f1.ToString().GetHashCode()));
        }


        /// <summary>
        /// Tests unequal hashcodes. 
        /// </summary>
        [TestMethod]
        public void CheckGetHashCodeInequality()
        {
            Formula f0 = new Formula("1e10 + 1e1", N, V);
            Formula f1 = new Formula("10000000000 + 10000000000", N, V);

            Assert.IsFalse(f0.GetHashCode().Equals(f1.GetHashCode()));
        }


        /// <summary>
        /// Checks if it returns FormulaError when trying to create invalid formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void OriginalFormulaIsNotValid()
        {
            Formula f = new Formula("10A1_ + 2.0 - 4");
        }


        /// <summary>
        /// Checks if it returns FormulaError when passed in false as constructor argument.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ConstructorDelegateInvalidFormula()
        {
            Formula f = new Formula("A1_ + 2.0 - 4", N, falseValidator);
        }


        /// <summary>
        /// Checks for basic parse rule.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CheckBasicParseRule()
        {
            Formula f1 = new Formula(")A1_ + 2.0 - 4", N, V);
        }


        /// <summary>
        /// Checks for basic parse rule.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CheckBasicParseRule2()
        {
            Formula f1 = new Formula("A1_ + 2.0 - 4(", N, V);
        }
        
        
        /// <summary>
        /// Checks for basic parse rule.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CheckBasicParseRule3()
        {
            Formula f1 = new Formula("A10 + $ + 2.0 - 4", N, V);
        }


        /// <summary>
        /// Checks for empty formula.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void CheckEmptyFormula()
        {
            Formula f1 = new Formula("", N, V);
        }


        /// <summary>
        /// Checks if obj is not of type Formula for Equals method.
        /// </summary>
        [TestMethod]
        public void CheckObjectTypeInequality()
        {
            Formula f1 = new Formula("10.0 + 20", N, V);
            string f2 = "Hello World";

            Assert.IsFalse(f1.Equals(f2));
            Assert.IsFalse(f1.Equals(null));
        }


        /// <summary>
        /// Checks object equality using the overloaded ==.
        /// </summary>
        [TestMethod]
        public void CheckEqualityOverloadedOperator()
        {
            Formula f1 = new Formula("10.0 + 100 - 20", N, V);
            Formula f2 = new Formula("10.0 + 100 - 20", N, V);
            Formula f3 = null;
            Formula f4 = null;

            Assert.IsTrue(f1 == f2);
            Assert.IsTrue(f3 == f4);
        }


        /// <summary>
        /// Checks object equality using the overloaded !=.
        /// </summary>
        [TestMethod]
        public void CheckInequalityOverloadedOperator()
        {
            Formula f1 = new Formula("10.0 + 10 - 20", N, V);
            Formula f2 = new Formula("10.0 + 1000 - 20", N, V);
            Formula f3 = null;
            Formula f4 = null;

            Assert.IsTrue(f1 != f2);
        }


        #region Method delegates for constructor and Evaluate method
        /// <summary>
        /// Delegate parameter method to find variable
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static double LookUp(string s)
        {
            if (s == "x")
                return 10.0;

            throw new ArgumentException("Variable not found.");
        }


        /// <summary>
        /// Simple normalize parameter for the Formula constructor.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static string N(string s)
        {
            return s.ToUpper();
        }


        /// <summary>
        /// Simple validator for the Formula constructor.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool V(string s)
        {
            return true;
        }


        /// <summary>
        /// Simple validator for the Formula constructor.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool falseValidator(string s)
        {
            return false;
        }
        #endregion
    }
}
