
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EngineValidator;

public record ResultValidation(string Message, bool IsValid);

public record AddressModel
{
  public string Line1 { get; set; }
  public string Line2 { get; set; }
  public string Line3 { get; set; }
}

public record AddressPattern
{
  public string ReplaceWith { get; set; }
  public IEnumerable<string> Patterns { get; set; }
}

public class Validator
{
  private readonly IReadOnlyList<AddressPattern> addressPatterns;
  public Validator()
  {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PatternList.json");
    var jsonText = File.ReadAllText(filePath);
    addressPatterns = JsonSerializer.Deserialize<List<AddressPattern>>(jsonText)!;
  }

  public ResultValidation Validate(AddressModel address, AddressModel addressTwo)
  {
    var messageBuilder = new StringBuilder();

    CheckEquality(address, one => one.Line1, addressTwo, two => two.Line1, messageBuilder);
    CheckEquality(address, one => one.Line2, addressTwo, two => two.Line2, messageBuilder);
    CheckEquality(address, one => one.Line3, addressTwo, two => two.Line3, messageBuilder);

    return new(messageBuilder.ToString(), messageBuilder.Length == 0);
  }

  private void CheckEquality<T1, T2>(T1 one, Func<T1, string> funcOne, T2 two, Func<T2, string> funcTwo, StringBuilder builder)
  {
    var valueOne = GetSanitized(funcOne.Invoke(one));
    var valueTwo = GetSanitized(funcTwo.Invoke(two));

    if (!valueOne.Equals(valueTwo))
      builder.AppendFormat("Error during the comparisson: '{0}' is not equals to '{1}'", valueOne, valueTwo);
  }

  private string GetSanitized(string value)
  {

    foreach (var ap in addressPatterns)
    {
      string pattern = @"\b(" + string.Join("|", ap.Patterns.Select(Regex.Escape)) + @")\b";
      value = Regex.Replace(value, pattern, match => ap.ReplaceWith);
    }
    return value;
  }
}