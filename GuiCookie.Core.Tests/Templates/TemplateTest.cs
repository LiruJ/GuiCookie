using FluentAssertions;
using GuiCookie.Core.Data.Xml;
using GuiCookie.Core.Templates;
using GuiCookie.Core.Tests.Resources.Templates;

namespace GuiCookie.Core.Tests.Templates;

[TestClass]
public class TemplateTest
{
    [TestMethod]
    public void Test_LoadsSheet()
    {
        // Arrange.
        var templateManager = new TemplateManager();
        using var stream = new MemoryStream(TemplateResources.TestLoad_Templates);
        var sheet = XmlSheetDataSource.Load(stream);

        // Act.
        var loadAction = () => templateManager.LoadFromSheets([sheet]);

        // Assert.
        loadAction.Should().NotThrow();
    }

    [TestMethod]
    public void Test_LoadsSheetTemplates()
    {
        // Arrange.
        var templateManager = new TemplateManager();
        using var stream = new MemoryStream(TemplateResources.TestLoad_Templates);
        var sheet = XmlSheetDataSource.Load(stream);

        // Act.
        templateManager.LoadFromSheets([sheet]);
        var getAAction = () => templateManager.GetTemplateFromName("TestA");
        var getBAction = () => templateManager.GetTemplateFromName("TestB");
        var getCAction = () => templateManager.GetTemplateFromName("TestC");

        // Assert.
        templateManager.TryGetTemplateFromName("TestA", out var testA).Should().BeTrue();
        templateManager.TryGetTemplateFromName("TestB", out var testB).Should().BeTrue();
        templateManager.TryGetTemplateFromName("TestC", out var testC).Should().BeTrue();
        testA.Should().NotBeNull();
        testB.Should().NotBeNull();
        testC.Should().NotBeNull();

        testA.Should().NotBe(testB);
        testB.Should().NotBe(testC);

        getAAction.Should().NotThrow();
        getBAction.Should().NotThrow();
        getCAction.Should().NotThrow();
    }
}
