using FluentAssertions;
using GuiCookie.Core.Components;
using GuiCookie.Core.Data.Xml;
using GuiCookie.Core.Elements;
using GuiCookie.Core.Templates;
using GuiCookie.Core.Tests.Resources.Templates;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GuiCookie.Core.Tests.Templates;

[TestClass]
public class TemplateInheritanceTest
{
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

    [TestMethod]
    public void Test_SingleInheritance_ControllerlessBase()
    {
        // Arrange.
        var templateManager = new TemplateManager();
        using var stream = new MemoryStream(TemplateResources.TestControllers_Templates);
        var sheet = XmlSheetDataSource.Load(stream);

        // Act.
        templateManager.LoadFromSheets([sheet]);

        var controllerlessBase = templateManager.GetTemplateFromName("ControllerlessBase");
        var controllerlessDerived = templateManager.GetTemplateFromName("ControllerlessDerived");

        // Assert.
        controllerlessBase.Should().NotBeNull();
        controllerlessDerived.Should().NotBeNull();

        controllerlessBase.ControllerName.Should().Be(nameof(Element));
        controllerlessDerived.ControllerName.Should().Be(nameof(TextBlock));
    }

    [TestMethod]
    public void Test_SingleInheritance_ControlleredBase()
    {
        // Arrange.
        var templateManager = new TemplateManager();
        using var stream = new MemoryStream(TemplateResources.TestControllers_Templates);
        var sheet = XmlSheetDataSource.Load(stream);

        // Act.
        templateManager.LoadFromSheets([sheet]);

        var controlleredBase = templateManager.GetTemplateFromName("ControlleredBase");
        var controlleredDerived = templateManager.GetTemplateFromName("ControlleredDerived");

        // Assert.
        controlleredBase.Should().NotBeNull();
        controlleredDerived.Should().NotBeNull();

        controlleredBase.ControllerName.Should().Be(nameof(TextBlock));
        controlleredDerived.ControllerName.Should().Be(nameof(TextBlock));
    }

    [TestMethod]
    public void Test_SingleInheritance_ControlleredChild()
    {
        // Arrange.
        var templateManager = new TemplateManager();
        using var stream = new MemoryStream(TemplateResources.TestControllers_Templates);
        var sheet = XmlSheetDataSource.Load(stream);

        // Act.
        templateManager.LoadFromSheets([sheet]);

        var parent = templateManager.GetTemplateFromName("TestParent");
        var controlleredChild = parent?.Children?[0];

        // Assert.
        parent.Should().NotBeNull();
        controlleredChild.Should().NotBeNull();

        parent.ControllerName.Should().Be(nameof(Element));
        controlleredChild.ControllerName.Should().Be(nameof(TextBlock));
    }
}
