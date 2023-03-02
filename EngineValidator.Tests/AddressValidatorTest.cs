
namespace EngineValidator.Tests;

public class AddressValidatorTest
{
  [Fact]
  public void Should_Return_False()
  {
    //Arrange
    var addressOne = new AddressModel()
    {
      Line1 = "R. Sâo Luis",
      Line2 = "Qd 23, Lt 15, Jardim América",
      Line3 = "77800999, São Paulo, Brazil"
    };

    var addressTwo = new AddressModel()
    {
      Line1 = "R Sâo Luis",
      Line2 = "Q 23, Lot 15, Jardim América",
      Line3 = "77800999, São Paulo, Brazil"
    };

    var expectedMessage = "Error during the comparisson: 'Quadra 23, Lote 15, Jardim América' is not equals to 'Quadra 23, Lot 15, Jardim América'";
    var sut = new Validator();

    //Act
    var result = sut.Validate(addressOne, addressTwo);

    // //Assert
    Assert.False(result.IsValid);
    Assert.Equal(expectedMessage, result.Message);
  }

  [Fact]
  public void Should_Return_True()
  {
    //Arrange
    var addressOne = new AddressModel()
    {
      Line1 = "R. Sâo Luis",
      Line2 = "Qd 23, Lt 15, Av Jardim América",
      Line3 = "77800999, São Paulo, Brazil"
    };

    var addressTwo = new AddressModel()
    {
      Line1 = "R Sâo Luis",
      Line2 = "Q. 23, Lt. 15, Av. Jardim América",
      Line3 = "77800999, São Paulo, Brazil"
    };

    var sut = new Validator();

    //Act
    var result = sut.Validate(addressOne, addressTwo);

    // //Assert
    Assert.True(result.IsValid);
    Assert.Empty(result.Message);
  }
}