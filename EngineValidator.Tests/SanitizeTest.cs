
using FluentAssertions;
using Moq;

namespace EngineValidator.Tests;

public class SanitizeTest
{
  public readonly Mock<ISettingService> settingService = new();

  [Theory]
  [InlineData("R das árvores", "Rua das árvores")]
  [InlineData("R. das árvores", "Rua das árvores")]
  [InlineData("Av das árvores", "Avenida das árvores")]
  [InlineData("Av. das árvores", "Avenida das árvores")]
  [InlineData("Avn das árvores", "Avenida das árvores")]
  [InlineData("Q das árvores quadradas", "Quadra das árvores quadradas")]
  [InlineData("Qd das árvores quadradas", "Quadra das árvores quadradas")]
  [InlineData("Q. das árvores quadradas", "Quadra das árvores quadradas")]
  [InlineData("Qd. das árvores quadradas", "Quadra das árvores quadradas")]
  [InlineData("nada a ser sanitizado rua avenida e quadra", "nada a ser sanitizado rua avenida e quadra")]
  public void Execute_Replace_Successfuly(string value, string expected)
  {
    //Arrange
    settingService.Setup(s => s.AddressPatterns).Returns(new List<AddressPattern>(){
      new AddressPattern{ ReplaceWith = "Avenida ", Patterns = new[]{"Av ", "Av. ", "Avn "} },
      new AddressPattern{ ReplaceWith = "Rua ", Patterns = new[]{"R ", "R. "} },
      new AddressPattern{ ReplaceWith = "Quadra ", Patterns = new[]{"Q ", "Qd ", "Q. ", "Qd. "} },
    });
    //Act
    var result = new Sanitize(settingService.Object).Execute(value);

    //Assert
    result.Should().Be(expected);
  }

  [Fact]
  public void Execute_DotReplace_When_ThereIs_no_Settings_Successfuly()
  {
    //Arrange
    string value = "R. Address Q and L Would not be replaced";
    settingService.Setup(s => s.AddressPatterns).Returns(new List<AddressPattern>());
    //Act
    var result = new Sanitize(settingService.Object).Execute(value);

    //Assert
    result.Should().Be(value);
  }


  [Fact]
  public void Constructor_IsRequired()
  {
    //Arrange
    //Act
    Func<Sanitize> funcIntance = () => new Sanitize((ISettingService)null!);

    //Assert
    funcIntance.Should().Throw<ArgumentNullException>();
  }
}