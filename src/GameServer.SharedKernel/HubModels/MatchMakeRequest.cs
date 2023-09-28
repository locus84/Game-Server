using System.Linq.Expressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace GameServer.SharedKernel.HubModels;

public sealed class MatchMakeRequest
{
  public int GroupCapacity { get; set; } = 2;
  public string? Condition { get; set; } // sex == male && age <= 28
  public Dictionary<string, string>? KeyValues { get; set; }
  public string? Username { get; set; }
  
  public DateTime RequestTime { get; private set; } = DateTime.UtcNow;

  public async Task<bool> Evaluate(MatchMakeRequest other)
  {
    if (string.IsNullOrEmpty(Condition) || KeyValues?.Any() != true)
    { // match make with no limitation
      return true;
    }

    if (GroupCapacity != other.GroupCapacity) return false;

    try
    {
      var code = $"{GenerateCondition(KeyValues, other.Condition!)} && {GenerateCondition(other.KeyValues!, Condition)}";
      
      return await CSharpScript.EvaluateAsync<bool>(code);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);

      return false;
    }
  }

  private string GenerateCondition(Dictionary<string, string> keyValues, string condition)
  {
    return keyValues.Keys.Aggregate(condition, (rep, key) => rep.Replace(key, keyValues[key] is string ? $"\"{keyValues[key]}\"" : keyValues[key]));
  }
}
