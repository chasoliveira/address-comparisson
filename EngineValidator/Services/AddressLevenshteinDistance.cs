namespace EngineValidator;

public interface ISimilarityComparisson
{
  (bool yesNo, double similarity) AreSimilar(string valueOnem, string valueTwo, double minimalAccept);
}
public class LevenshteinDistance : ISimilarityComparisson
{
  public (bool yesNo, double similarity) AreSimilar(string valueOne, string valueTwo, double acceptPercentagem)
  {
    var distance = CalculateDistance(valueOne, valueTwo);
    int maxLength = Math.Max(valueOne.Length, valueTwo.Length);
    var normalizedDistance = 1.0 - ((double)distance / maxLength);
    var similarityPercentage = normalizedDistance * 100.0;
    return (similarityPercentage >= acceptPercentagem, similarityPercentage);
  }
  
  private int CalculateDistance(string valueOne, string valueTwo)
  {
    int[,] distances = new int[valueOne.Length + 1, valueTwo.Length + 1];

    for (int i = 0; i <= valueOne.Length; i++)
    {
      distances[i, 0] = i;
    }

    for (int j = 0; j <= valueTwo.Length; j++)
    {
      distances[0, j] = j;
    }

    for (int j = 1; j <= valueTwo.Length; j++)
    {
      for (int i = 1; i <= valueOne.Length; i++)
      {
        if (valueOne[i - 1] == valueTwo[j - 1])
        {
          distances[i, j] = distances[i - 1, j - 1];
        }
        else
        {
          distances[i, j] = Math.Min(distances[i - 1, j], Math.Min(distances[i, j - 1], distances[i - 1, j - 1])) + 1;
        }
      }
    }

    return distances[valueOne.Length, valueTwo.Length];
  }
}
