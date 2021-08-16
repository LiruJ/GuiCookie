using GuiCookie.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;

namespace GuiCookieTests.Parsers
{
    [TestClass]
    public class NineSliceParsersTest
    {
        [TestMethod]
        public void NullStringTest()
        {
            Assert.IsFalse(NineSlice.TryParse(null, out _), "TryParse returned true.");
            Assert.ThrowsException<ArgumentNullException>(() => NineSlice.Parse(null), $"Parse did not throw {nameof(ArgumentNullException)}.");
        }

        [TestMethod]
        public void EmptyStringTest()
        {
            Assert.IsFalse(NineSlice.TryParse(string.Empty, out _), "TryParse returned true.");
            Assert.ThrowsException<ArgumentNullException>(() => NineSlice.Parse(string.Empty), $"Parse did not throw {nameof(ArgumentNullException)}.");
        }

        [TestMethod]
        public void KeywordTest()
        {
            Assert.AreEqual(NineSlice.Thirds, NineSlice.Parse("Thirds"), "Thirds is not equal.");
            Assert.AreEqual(NineSlice.HSlice, NineSlice.Parse("HSlice"), "HSlice is not equal.");
            Assert.AreEqual(NineSlice.VSlice, NineSlice.Parse("VSlice"), "VSlice is not equal.");
        }

        [TestMethod]
        public void ArgumentCountTest()
        {
            Assert.IsFalse(NineSlice.TryParse("_, _, _", out _), "TryParse returned true with 3 arguments.");
            Assert.ThrowsException<ArgumentException>(() => NineSlice.Parse("_, _, _"), $"Parse did not throw {nameof(ArgumentException)} with 3 arguments.");

            Assert.IsFalse(NineSlice.TryParse("_, _, _, _, _, _", out _), "TryParse returned true with 6 arguments.");
            Assert.ThrowsException<ArgumentException>(() => NineSlice.Parse("_, _, _, _, _, _"), $"Parse did not throw {nameof(ArgumentException)} with 6 arguments.");
        }

        [TestMethod]
        public void FourValueTest()
        {
            Assert.IsTrue(NineSlice.TryParse("0.2, 0.8, 0.1, 0.9", out NineSlice nineSlice), "Parse failed.");

            Assert.AreEqual(0.2f, nineSlice.MinX, "MinX was invalid.");
            Assert.AreEqual(0.8f, nineSlice.MaxX, "MaxX was invalid.");
            Assert.AreEqual(0.1f, nineSlice.MinY, "MinY was invalid.");
            Assert.AreEqual(0.9f, nineSlice.MaxY, "MaxY was invalid.");
        }

        [TestMethod]
        public void FiveValueTest()
        {
            Assert.IsTrue(NineSlice.TryParse("0.2, 0.8, 0.1, 0.9, 01011", out NineSlice nineSlice), "Parse failed.");

            Assert.AreEqual(0.2f, nineSlice.MinX, "MinX was invalid.");
            Assert.AreEqual(0.8f, nineSlice.MaxX, "MaxX was invalid.");
            Assert.AreEqual(0.1f, nineSlice.MinY, "MinY was invalid.");
            Assert.AreEqual(0.9f, nineSlice.MaxY, "MaxY was invalid.");

            Assert.IsFalse(nineSlice.IsPieceStretched(Piece.Left), "Left edge is stretched when it shouldn't be.");
            Assert.IsTrue(nineSlice.IsPieceStretched(Piece.Bottom), "Bottom edge is not stretched when it should be.");
            Assert.IsFalse(nineSlice.IsPieceStretched(Piece.Right), "Right edge is stretched when it shouldn't be.");
            Assert.IsTrue(nineSlice.IsPieceStretched(Piece.Top), "Top edge is not stretched when it should be.");
            Assert.IsTrue(nineSlice.IsPieceStretched(Piece.Centre), "Centre is not stretched when it should be.");
        }

        [TestMethod]
        public void FiveValueDecimalTest()
        {
            // Save the current culture.
            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            // Set the culture to German, as it uses the ',' character for decimal places which should break parsing if not accounted for.
            CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");

            bool parsedCorrectly = NineSlice.TryParse("0.2, 0.8, 0.1, 0.9, 01011", out NineSlice nineSlice);

            // Set the culture back so that the error message displays correctly.
            CultureInfo.CurrentCulture = currentCulture;

            Assert.IsTrue(parsedCorrectly, $"Parse failed {nineSlice}.");

            Assert.AreEqual(0.2f, nineSlice.MinX, "MinX was invalid.");
            Assert.AreEqual(0.8f, nineSlice.MaxX, "MaxX was invalid.");
            Assert.AreEqual(0.1f, nineSlice.MinY, "MinY was invalid.");
            Assert.AreEqual(0.9f, nineSlice.MaxY, "MaxY was invalid.");

            Assert.IsFalse(nineSlice.IsPieceStretched(Piece.Left), "Left edge is stretched when it shouldn't be.");
            Assert.IsTrue(nineSlice.IsPieceStretched(Piece.Bottom), "Bottom edge is not stretched when it should be.");
            Assert.IsFalse(nineSlice.IsPieceStretched(Piece.Right), "Right edge is stretched when it shouldn't be.");
            Assert.IsTrue(nineSlice.IsPieceStretched(Piece.Top), "Top edge is not stretched when it should be.");
            Assert.IsTrue(nineSlice.IsPieceStretched(Piece.Centre), "Centre is not stretched when it should be.");
        }
    }
}
