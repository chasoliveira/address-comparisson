using System.Text.RegularExpressions;

namespace EngineValidator;

public interface ISanitize
{
  string Execute(string value);
}
public class Sanitize : ISanitize
{
  private readonly ISettingService settingService;

  public Sanitize(ISettingService settingService)
  {
    ArgumentNullException.ThrowIfNull(settingService);
    this.settingService = settingService;
  }

  public string Execute(string value)
  {
    foreach (var ap in settingService.AddressPatterns)
    {
      string pattern = @"\b(" + string.Join("|", ap.Patterns.Select(Regex.Escape)) + @")\b";
      value = Regex.Replace(value, pattern, match => ap.ReplaceWith);
    }
    return value;
  }
}
