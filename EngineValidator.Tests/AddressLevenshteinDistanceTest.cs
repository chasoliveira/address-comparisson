
using FluentAssertions;

namespace EngineValidator.Tests;

public class AddressLevenshteinDistanceTest
{
  [Fact]
  public void AreSimilar_WithIdenticalStrings_Returns100Percent()
  {
    //Arrange
    string a = "123 Main St";
    string b = "123 Main St";
    var acceptPercentage = 90.0;

    //Act
    var (hasSimilarity, similarity) = new LevenshteinDistance().AreSimilar(a, b, acceptPercentage);

    //Assert
    hasSimilarity.Should().BeTrue();
    similarity.Should().Be(100.0);
  }

  [Fact]
  public void AreSimilar_WithDifferentStrings_ReturnsLessThan100Percent()
  {
    //Arrange
    string a = "123 Main St";
    string b = "124 Main St";
    var acceptPercentage = 90.0;

    //Act
    var (hasSimilarity, similarity) = new LevenshteinDistance().AreSimilar(a, b, acceptPercentage);

    //Assert
    hasSimilarity.Should().BeTrue();
    similarity.Should().BeGreaterThanOrEqualTo(acceptPercentage);
  }

  [Fact]
  public void AreSimilar_WithCompletelyDifferentStrings_Returns0Percent()
  {
    //Arrange
    string a = "123 Main St";
    string b = "456 Maple Ave";
    var acceptPercentage = 90.0;

    //Act
    var (hasSimilarity, similarity) = new LevenshteinDistance().AreSimilar(a, b, acceptPercentage);

    //Assert
    hasSimilarity.Should().BeFalse();
    similarity.Should().BeLessThan(acceptPercentage);
  }
}
