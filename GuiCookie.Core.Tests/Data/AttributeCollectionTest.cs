using FluentAssertions;
using GuiCookie.Core.Data;

namespace GuiCookie.Core.Tests.Data;

[TestClass]
public class AttributeCollectionTest
{
    [TestMethod]
    public void Test_ReadOnlyDerived_GetsBase()
    {
        // Arrange
        var baseAttributes = new AttributeCollection();
        var derivedAttributes = new AttributeCollection() { { "TestKey", "TestValue" } };

        var combinedAttributes = new ReadOnlyDerivedAttributeCollection(baseAttributes, derivedAttributes);

        // Act
        var result = combinedAttributes.GetAttributeOrDefault("TestKey", (string?)null);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("TestValue");
    }

    [TestMethod]
    public void Test_ReadOnlyDerived_GetsDerived()
    {
        // Arrange
        var baseAttributes = new AttributeCollection() { { "TestKey", "TestValue" } };
        var derivedAttributes = new AttributeCollection();

        var combinedAttributes = new ReadOnlyDerivedAttributeCollection(baseAttributes, derivedAttributes);

        // Act
        var result = combinedAttributes.GetAttributeOrDefault("TestKey", (string?)null);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("TestValue");
    }

    [TestMethod]
    public void Test_ReadOnlyDerived_OverridesBase()
    {
        // Arrange
        var baseAttributes = new AttributeCollection() { {"TestKey", "TestValue"}};
        var derivedAttributes = new AttributeCollection() { { "TestKey", "TestValue" } };

        var combinedAttributes = new ReadOnlyDerivedAttributeCollection(baseAttributes, derivedAttributes);

        // Act
        var result = combinedAttributes.GetAttributeOrDefault("TestKey", (string?)null);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be("TestValue");
    }

    [TestMethod]
    public void Test_ReadOnlyDerived_EnumeratesAllUniqueKeys()
    {
        // Arrange
        const int totalKeyCount = 50;
        const int attributesKeyCount = 30;

        var keys = new List<string>(totalKeyCount);
        for (int i = 0; i < totalKeyCount; i++)
            keys.Add($"TestKey{i}");

        var baseAttributes = new AttributeCollection();
        for (int i = 0; i < attributesKeyCount; i++)
            baseAttributes.Add(keys[i], $"TestValue{i}_base");
        var derivedAttributes = new AttributeCollection();
        for (int i = totalKeyCount - attributesKeyCount; i < totalKeyCount; i++)
            derivedAttributes.Add(keys[i], $"TestValue{i}_derived");

        var combinedAttributes = new ReadOnlyDerivedAttributeCollection(baseAttributes, derivedAttributes);

        // Act
        var result = new List<string>();
        foreach (var combinedKey in combinedAttributes)
            result.Add(combinedKey);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().OnlyHaveUniqueItems();

        combinedAttributes.Should().HaveCount(totalKeyCount);
        result.Should().HaveCount(totalKeyCount);

        for (int i = 0; i < result.Count; i++)
        {
            string? combinedKey = result[i];
            var attribute = combinedAttributes.GetAttributeOrDefault(combinedKey, (string?)null);
            attribute.Should().NotBeNullOrWhiteSpace();
            attribute.Should().EndWith(i < totalKeyCount - attributesKeyCount ? "_base" : "_derived");
        }
    }
}
