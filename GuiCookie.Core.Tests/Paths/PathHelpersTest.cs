using FluentAssertions;
using GuiCookie.Core.Helpers;

namespace GuiCookie.Core.Tests.Paths;

[TestClass]
public class PathHelpersTest
{
    [TestMethod]
    [DataRow(["xml", "txt"])]
    [DataRow(["xml", "txt", "bmp"])]
    [DataRow([".xml", ".txt", ".bmp"])]
    public void Test_ResolvePaths_ResolvesWildcardNoExtension(string[] possibleExtensions)
    {
        // Arrange.
        var paths = new List<string>() { "*" };
        var rootPath = "Paths/TestFiles/Wildcard";
        var failedPaths = new List<string>();

        // Act.
        var result = PathHelpers.ResolveFilePaths(paths, possibleExtensions, rootPath, failedPaths);

        // Assert.
        result.Should().NotBeEmpty();
        failedPaths.Should().BeEmpty();
        result.Should().AllSatisfy(x => Path.HasExtension(x).Should().BeTrue());
        result.Should().AllSatisfy(x => possibleExtensions.Should().Contain(y => y[0] == '.' ? Path.GetExtension(x) == y : Path.GetExtension(x).Substring(1) == y));
    }

    [TestMethod]
    [DataRow(new string[] {"xml", "txt"}, "xml")]
    [DataRow(new string[] {"txt"}, "xml")]
    [DataRow(new string[0], "xml")]
    [DataRow(new string[0], "txt")]
    public void Test_ResolvePaths_ResolvesWildcardExtension(string[] possibleExtensions, string extension)
    {
        // Arrange.
        var paths = new List<string>() { Path.ChangeExtension("*", extension) };
        var rootPath = "Paths/TestFiles/Wildcard";
        var failedPaths = new List<string>();

        // Act.
        var result = PathHelpers.ResolveFilePaths(paths, possibleExtensions, rootPath, failedPaths);

        // Assert.
        result.Should().NotBeEmpty();
        failedPaths.Should().BeEmpty();
        result.Should().AllSatisfy(x => Path.HasExtension(x).Should().BeTrue());
        result.Should().AllSatisfy(x => Path.GetExtension(x).Should().Be(Path.GetExtension(Path.ChangeExtension("_", extension))));
    }

    [TestMethod]
    public void Test_ResolvePaths_ResolvesPathNoExtension()
    {
        // Arrange.
        var paths = new List<string>() { "TestFile1" };
        var rootPath = "Paths/TestFiles/Wildcard";
        var possibleExtensions = new string[] { "xml", "txt" };
        var failedPaths = new List<string>();

        // Act.
        var result = PathHelpers.ResolveFilePaths(paths, possibleExtensions, rootPath, failedPaths);

        // Assert.
        result.Should().HaveCount(2);
        failedPaths.Should().BeEmpty();
        result.Should().AllSatisfy(x => Path.HasExtension(x).Should().BeTrue());
        result.Should().AllSatisfy(x => possibleExtensions.Should().Contain(y => y[0] == '.' ? Path.GetExtension(x) == y : Path.GetExtension(x).Substring(1) == y));
    }

    [TestMethod]
    public void Test_ResolvePaths_ResolvesPathExtension()
    {
        // Arrange.
        var extension = "xml";
        var paths = new List<string>() { Path.ChangeExtension("TestFile1", extension) };
        var rootPath = "Paths/TestFiles/Wildcard";
        var possibleExtensions = new string[] { extension };
        var failedPaths = new List<string>();

        // Act.
        var result = PathHelpers.ResolveFilePaths(paths, possibleExtensions, rootPath, failedPaths);

        // Assert.
        result.Should().HaveCount(1);
        failedPaths.Should().BeEmpty();
        result.Should().AllSatisfy(x => Path.HasExtension(x).Should().BeTrue());
        result.Should().AllSatisfy(x => Path.GetExtension(x).Should().Be(Path.GetExtension(Path.ChangeExtension("_", extension))));
    }
}
