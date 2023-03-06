using System.Text;

namespace EngineValidator;

public record ResultValidation(string Target, string Message, bool IsValid, IDictionary<string, IDictionary<string, string>> Validations);

public interface IValidator
{
  ResultValidation Validate(AddressModel address, AddressModel addressTwo);
}

public class Validator : IValidator
{
  private readonly ISettingService settingService;
  private readonly ISanitize sanitize;
  private readonly ISimilarityComparisson comparisson;

  public Validator(ISettingService settingService, ISanitize sanitize, ISimilarityComparisson comparisson)
  {
    ArgumentNullException.ThrowIfNull(settingService);
    ArgumentNullException.ThrowIfNull(sanitize);
    ArgumentNullException.ThrowIfNull(comparisson);

    this.settingService = settingService;
    this.sanitize = sanitize;
    this.comparisson = comparisson;
  }

  public ResultValidation Validate(AddressModel address, AddressModel addressTwo)
  {
    var messageBuilder = new StringBuilder();
    var validations = new Dictionary<string, IDictionary<string, string>>();

    validations.Add("line1", CheckEquality(address, one => one.Line1, addressTwo, two => two.Line1, messageBuilder));
    validations.Add("line2", CheckEquality(address, one => one.Line2, addressTwo, two => two.Line2, messageBuilder));
    validations.Add("line3", CheckEquality(address, one => one.Line3, addressTwo, two => two.Line3, messageBuilder));

    return new(string.Format("{0:0.00} %", settingService.Similarity), messageBuilder.ToString(), messageBuilder.Length == 0, validations);
  }

  private IDictionary<string, string> CheckEquality<T1, T2>(T1 one, Func<T1, string> funcOne, T2 two, Func<T2, string> funcTwo, StringBuilder builder)
  {
    var valueOne = sanitize.Execute(funcOne.Invoke(one));
    var valueTwo = sanitize.Execute(funcTwo.Invoke(two));

    var (hasSimilarity, similarityPercent) = comparisson.AreSimilar(valueOne, valueTwo, settingService.Similarity);
    if (!hasSimilarity)
      builder.AppendFormat("The similarity is: '{0}%' and '{1}' is not similar to '{2}'", similarityPercent, valueOne, valueTwo);

    var result = new Dictionary<string, string>();
    result.Add("one", valueOne);
    result.Add("two", valueTwo);
    result.Add("similarity", string.Format("{0:0.00} %", similarityPercent));

    return result;
  }
}
