using System.Text.Json;

namespace EngineValidator;


public interface ISettingService
{
  int Similarity { get; }
  IReadOnlyList<AddressPattern> AddressPatterns { get; }
}
public class SettingService: ISettingService
{
  private AddressConfig addressPatterns;

  public SettingService()
  {
    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "PatternList.json");
    var jsonText = File.ReadAllText(filePath);
    addressPatterns = JsonSerializer.Deserialize<AddressConfig>(jsonText)!;
  }

  public int Similarity => addressPatterns.Similarity;

  public IReadOnlyList<AddressPattern> AddressPatterns => addressPatterns.Values.ToList().AsReadOnly();
}