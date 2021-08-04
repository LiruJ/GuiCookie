using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace GuiCookie.Helpers.Tests
{
    [TestClass()]
    public class GridLayoutHelperTests
    {
        [TestMethod()]
        public void AdjustPixelPositionTest()
        {
            Point cellSize = new Point(32);
            Point gridSize = new Point(30);

            for (int x = -gridSize.X; x < gridSize.X; x++)
                for (int y = -gridSize.Y; y < gridSize.Y; y++)
                    for (int subX = 0; subX < cellSize.X; subX++)
                        for (int subY = 0; subY < cellSize.Y; subY++)
                            Assert.AreEqual(new Point(x, y) * cellSize, GridLayoutHelper.SnapPixelPosition((new Point(x, y) * cellSize) + new Point(subX, subY), cellSize));
        }
    }
}