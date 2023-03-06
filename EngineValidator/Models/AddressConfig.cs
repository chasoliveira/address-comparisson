namespace EngineValidator;

public record AddressConfig
{

  public int Similarity { get; set; }
  public IEnumerable<AddressPattern> Values { get; set; }
}

public record AddressPattern
{
  public string ReplaceWith { get; set; }
  public IEnumerable<string> Patterns { get; set; }
}