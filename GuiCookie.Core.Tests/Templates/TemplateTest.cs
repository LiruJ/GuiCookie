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

    [TestMethod]
    public void Test_SingleInheritance_SameFile()
    {
        // Arrange.
        var templateManager = new TemplateManager();
        using var stream = new MemoryStream(TemplateResources.TestSingleInheritance_Templates);
        var sheet = XmlSheetDataSource.Load(stream);
        
        // Act.
        templateManager.LoadFromSheets([sheet]);
        var baseTemplate = templateManager.GetTemplateFromName("TestA");
        var derivedTemplate = templateManager.GetTemplateFromName("TestB");

        // Assert.
        derivedTemplate.BaseTemplate.Should().Be(baseTemplate);

        baseTemplate.Attributes.HasAttribute("TestValue").Should().BeTrue();
        derivedTemplate.Attributes.HasAttribute("TestValue").Should().BeTrue();
        baseTemplate.Attributes.GetAttribute("TestValue", int.Parse).Should().Be(5);
        derivedTemplate.Attributes.GetAttribute("TestValue", int.Parse).Should().Be(10);

        baseTemplate.Attributes.HasAttribute("TestBaseValue").Should().BeTrue();
        derivedTemplate.Attributes.HasAttribute("TestBaseValue").Should().BeTrue();

        var baseTestValue = baseTemplate.Attributes.GetAttribute("TestBaseValue", int.Parse);
        baseTestValue.Should().Be(35);
        derivedTemplate.Attributes.GetAttribute("TestBaseValue", int.Parse).Should().Be(baseTestValue);
    }
}
