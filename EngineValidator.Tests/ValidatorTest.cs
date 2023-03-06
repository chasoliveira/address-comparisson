
using FluentAssertions;
using Moq;

namespace EngineValidator.Tests;

public class ValidatorTest
{
  private readonly Mock<ISanitize> sanitize;
  private readonly Mock<ISimilarityComparisson> comparisson;
  private readonly Mock<ISettingService> setting;

  public ValidatorTest()
  {
    setting = new();
    sanitize = new();
    comparisson = new();
  }

  private IValidator ValidatorInstance() => new Validator(setting.Object, sanitize.Object, comparisson.Object);

  [Theory]
  [MemberData(nameof(ValidatorParameters))]
  public void Constructor_IsRequired(ISettingService settingService, ISanitize sanitize, ISimilarityComparisson comparisson, string parameter)
  {
    //Arrange
    //Act
    Func<Validator> funcIntance = () => new Validator(settingService, sanitize, comparisson);

    //Assert
    funcIntance.Should().Throw<ArgumentNullException>().WithMessage($"Value cannot be null. (Parameter '{parameter}')");
  }
  public static IEnumerable<object[]> ValidatorParameters => new[]{
    new object[]{ (ISettingService)null!, new Mock<ISanitize>().Object, new Mock<ISimilarityComparisson>().Object,"settingService"},
    new object[]{ new Mock<ISettingService>().Object, (ISanitize)null!, new Mock<ISimilarityComparisson>().Object,"sanitize"},
    new object[]{ new Mock<ISettingService>().Object, new Mock<ISanitize>().Object, (ISimilarityComparisson)null!,"comparisson"},
  };

  [Fact]
  public void Should_Return_False()
  {
    //Arrange
    var addressOne = new AddressModel()
    {
      Line1 = "R. Sâo Luis",
      Line2 = "Qd 23, Lt 15, Jardim América",
      Line3 = "A 77800999, São Paulo, Brazil"
    };

    var addressTwo = new AddressModel()
    {
      Line1 = "R Sâo Luis",
      Line2 = "Q 23, Lot 15, Jardim América",
      Line3 = "B 77800999, São Paulo, Brazil"
    };

    sanitize.Setup(s => s.Execute(addressOne.Line1)).Returns("A");
    sanitize.Setup(s => s.Execute(addressTwo.Line1)).Returns("A");
    sanitize.Setup(s => s.Execute(addressOne.Line2)).Returns("A");
    sanitize.Setup(s => s.Execute(addressTwo.Line2)).Returns("A");
    sanitize.Setup(s => s.Execute(addressOne.Line3)).Returns("A");
    sanitize.Setup(s => s.Execute(addressTwo.Line3)).Returns("B");

    comparisson.Setup(c => c.AreSimilar(It.Is<string>(v => v == "A"), It.Is<string>(v => v == "A"), It.IsAny<double>())).Returns((true, 99));
    comparisson.Setup(c => c.AreSimilar(It.Is<string>(v => v == "A"), It.Is<string>(v => v == "B"), It.IsAny<double>())).Returns((false, 89));

    var expectedMessage = "The similarity is: '89%' and 'A' is not similar to 'B'";

    var sut = ValidatorInstance();

    //Act
    var result = sut.Validate(addressOne, addressTwo);

    // //Assert
    result.IsValid.Should().BeFalse();
    result.Message.Should().Be(expectedMessage);
    result.Validations.Should().SatisfyRespectively(v1 =>
    {
      v1.Key.Should().Be("line1");
      v1.Value.Should().SatisfyRespectively(vv1 =>
      {
        vv1.Key.Should().Be("one");
        vv1.Value.Should().Be("A");
      },
      vv2 =>
      {
        vv2.Key.Should().Be("two");
        vv2.Value.Should().Be("A");
      },
      vv3 =>
      {
        vv3.Key.Should().Be("similarity");
        vv3.Value.Should().Be("99.00 %");
      });
    },
    v2 =>
    {
      v2.Key.Should().Be("line2");
      v2.Value.Should().SatisfyRespectively(vv1 =>
      {
        vv1.Key.Should().Be("one");
        vv1.Value.Should().Be("A");
      },
      vv2 =>
      {
        vv2.Key.Should().Be("two");
        vv2.Value.Should().Be("A");
      },
      vv3 =>
      {
        vv3.Key.Should().Be("similarity");
        vv3.Value.Should().Be("99.00 %");
      });
    },
    v3 =>
    {
      v3.Key.Should().Be("line3");
      v3.Value.Should().SatisfyRespectively(vv1 =>
      {
        vv1.Key.Should().Be("one");
        vv1.Value.Should().Be("A");
      },
      vv2 =>
      {
        vv2.Key.Should().Be("two");
        vv2.Value.Should().Be("B");
      },
      vv3 =>
      {
        vv3.Key.Should().Be("similarity");
        vv3.Value.Should().Be("89.00 %");
      });
    });
  }

  [Fact]
  public void Should_Return_True()
  {
    //Arrange
    var address = new AddressModel();

    sanitize.Setup(s => s.Execute(It.IsAny<string>())).Returns("A");

    comparisson.Setup(c => c.AreSimilar(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>())).Returns((true, 99));

    var sut = ValidatorInstance();

    //Act
    var result = sut.Validate(address, address);

    // //Assert
    result.IsValid.Should().BeTrue();
    result.Message.Should().BeEmpty();
  }
}
