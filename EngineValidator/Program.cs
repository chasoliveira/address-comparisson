using EngineValidator;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapPost("/", handler: (RequestValidate address) =>
{
  var result = new Validator().Validate(address.One, address.Two);
  return TypedResults.Ok(result);
});

app.Run();

public class RequestValidate{
  public AddressModel One { get; set; }
  public AddressModel Two { get; set; }
}