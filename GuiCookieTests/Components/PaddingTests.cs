using GuiCookie.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GuiCookieTests.Components
{
    [TestClass()]
    public class PaddingTests
    {
        [TestMethod]
        public void EmptyFailTest() => Assert.IsFalse(Sides.TryParse(string.Empty, out _));

        [TestMethod]
        public void WhiteSpaceFailTest() => Assert.IsFalse(Sides.TryParse("   ", out _));

        [TestMethod]
        public void NullFailTest() => Assert.IsFalse(Sides.TryParse(null, out _));

        [TestMethod]
        public void ParseOneValueTest()
        {
            // The value.
            int value = 4;

            // Parse the value into sides.
            Assert.IsTrue(Sides.TryParse(value.ToString(), out Sides sides), "Sides could not parse.");

            // Ensure each side is correct.
            Assert.AreEqual(sides.Left, value, "Left was not correct.");
            Assert.AreEqual(sides.Bottom, value, "Bottom was not correct.");
            Assert.AreEqual(sides.Right, value, "Right was not correct.");
            Assert.AreEqual(sides.Top, value, "Top was not correct.");

            // Ensure each side is not relative.
            Assert.IsFalse(sides.IsLeftRelative, "Left was relative.");
            Assert.IsFalse(sides.IsBottomRelative, "Bottom was relative.");
            Assert.IsFalse(sides.IsRightRelative, "Right was relative.");
            Assert.IsFalse(sides.IsTopRelative, "Top was relative.");
        }

        [TestMethod]
        public void ParseTwoValueTest()
        {
            // The values.
            int topBottom = 4, leftRight = 2;

            // Parse the values into sides.
            Assert.IsTrue(Sides.TryParse($"{topBottom}, {leftRight}", out Sides sides), "Sides could not parse.");

            // Ensure each side is correct.
            Assert.AreEqual(sides.Left, leftRight, "Left was not correct.");
            Assert.AreEqual(sides.Bottom, topBottom, "Bottom was not correct.");
            Assert.AreEqual(sides.Right, leftRight, "Right was not correct.");
            Assert.AreEqual(sides.Top, topBottom, "Top was not correct.");

            // Ensure each side is not relative.
            Assert.IsFalse(sides.IsLeftRelative, "Left was relative.");
            Assert.IsFalse(sides.IsBottomRelative, "Bottom was relative.");
            Assert.IsFalse(sides.IsRightRelative, "Right was relative.");
            Assert.IsFalse(sides.IsTopRelative, "Top was relative.");
        }

        [TestMethod]
        public void ParseThreeValueTest()
        {
            // The values.
            int top = 4, leftRight = 2, bottom = 6;

            // Parse the values into sides.
            Assert.IsTrue(Sides.TryParse($"{top}, {leftRight}, {bottom}", out Sides sides), "Sides could not parse.");

            // Ensure each side is correct.
            Assert.AreEqual(sides.Left, leftRight, "Left was not correct.");
            Assert.AreEqual(sides.Bottom, bottom, "Bottom was not correct.");
            Assert.AreEqual(sides.Right, leftRight, "Right was not correct.");
            Assert.AreEqual(sides.Top, top, "Top was not correct.");

            // Ensure each side is not relative.
            Assert.IsFalse(sides.IsLeftRelative, "Left was relative.");
            Assert.IsFalse(sides.IsBottomRelative, "Bottom was relative.");
            Assert.IsFalse(sides.IsRightRelative, "Right was relative.");
            Assert.IsFalse(sides.IsTopRelative, "Top was relative.");
        }

        [TestMethod]
        public void ParseFourValueTest()
        {
            // The values.
            int top = 4, left = 2, bottom = 6, right = 8;

            // Parse the values into sides.
            Assert.IsTrue(Sides.TryParse($"{top}, {right}, {bottom}, {left}", out Sides sides), "Sides could not parse.");

            // Ensure each side is correct.
            Assert.AreEqual(sides.Left, left, "Left was not correct.");
            Assert.AreEqual(sides.Bottom, bottom, "Bottom was not correct.");
            Assert.AreEqual(sides.Right, right, "Right was not correct.");
            Assert.AreEqual(sides.Top, top, "Top was not correct.");

            // Ensure each side is not relative.
            Assert.IsFalse(sides.IsLeftRelative, "Left was relative.");
            Assert.IsFalse(sides.IsBottomRelative, "Bottom was relative.");
            Assert.IsFalse(sides.IsRightRelative, "Right was relative.");
            Assert.IsFalse(sides.IsTopRelative, "Top was relative.");
        }
    }
}
