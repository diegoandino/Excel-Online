using FormulaEvaluator;
using NUnit.Framework;

namespace EvaluatorUnitTests
{
    /**
     * Diego Andino
     * September 4, 2020
     */

    /// <summary>
    /// NUnit Test class for the Evaluator class
    /// </summary>
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Delegate parameter method to find variable
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        int LookUp(System.String s)
        {
            if (s == "A6")
                return 10;

            if (s.Contains(" "))
                throw new System.ArgumentException("Variable cannot be spaced.");

            return 0;
        }

        [Test]
        public void SimpleCalculation()
        {
            Assert.AreEqual(22, Evaluator.Evaluate("10 + 2 * 6", LookUp));
        }

        [Test]
        public void SimpleCalculation2()
        {
            Assert.AreEqual(212, Evaluator.Evaluate("100 * 2 + 12", LookUp));
        }

        [Test]
        public void SimpleCalculationWithParenthesis()
        {
            Assert.AreEqual(1400, Evaluator.Evaluate("100 * ( 2 + 12 )", LookUp));
        }

        [Test]
        public void SimpleCalculationWithParenthesis2()
        {
            Assert.AreEqual(10, Evaluator.Evaluate("10 * ( 2 + 12 ) / 14", LookUp));
        }

        [Test]
        public void OddCalculation()
        {
            Assert.AreEqual(1, Evaluator.Evaluate("1(+1) / 2", LookUp));
        }

        [Test]
        public void SimpleCalculationWithVariable()
        {
            Assert.AreEqual(20, Evaluator.Evaluate("10 + A6", LookUp));
        }

        [Test]
        public void OddCalculationWithVariable()
        {
            Assert.AreEqual(200, Evaluator.Evaluate("10(+10) * A6", LookUp));
        }

        [Test]
        public void ComplexCalculationWithVariable()
        {
            Assert.AreEqual(165, Evaluator.Evaluate("20 * (2 + A6 + (2 * 2 + 50)) / 8", LookUp));
        }

        [Test]
        public void SpacedVariableError()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("1 + A 6", LookUp));
        }

        [Test]
        public void DivideByZero()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("20 * 2 + (2 * 2) / 0", LookUp));
        }

        [Test]
        public void MissingParenthesis()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("20 * 2 + 2 * 2) / 8", LookUp));
        }

        [Test]
        public void ArgumentStringIsEmpty()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("", LookUp));
        }

        [Test]
        public void MissingNumber()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("10 + ", LookUp));
        }

        [Test]
        public void MissingNumber2()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("10 * ", LookUp));
        }

        [Test]
        public void MissingAllNumbers()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate(" * ", LookUp));
        }

        [Test]
        public void ExtraOperatorAtEnd()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("10 * 10 + (90 - 85) /", LookUp));
        }

        [Test]
        public void ExtraOperatorAtBeginning()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("* 10 * 10 + (90 - 85)", LookUp));
        }

        [Test]
        public void TwoExtraOperators()
        {
            Assert.Throws<System.ArgumentException>(() => Evaluator.Evaluate("* 10 * 10 + (90 - 85) /", LookUp));
        }
    }
}