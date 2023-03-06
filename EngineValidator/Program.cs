using EngineValidator;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ISettingService, SettingService>();
builder.Services.AddScoped<ISanitize, Sanitize>();
builder.Services.AddScoped<ISimilarityComparisson, LevenshteinDistance>();
builder.Services.AddScoped<IValidator, Validator>();
var app = builder.Build();

app.MapGet("/", () => "Hello World! This a Address Validator.");
app.MapPost("/", (
  [FromServices] IValidator validator,
  [FromBody] RequestValidate address) =>
{
  var result = validator.Validate(address.One, address.Two);
  return TypedResults.Ok(result);
});

app.Run();
