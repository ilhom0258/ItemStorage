using System.Text.Json;
namespace ItemStorage.Data;

public class DataFilter
{
    public string PropertyName { get; set; }
    public DataFilterType DataFilterType { get; set; }
    public JsonElement Value { get; set; }
}

public enum DataFilterType
{
    Equals = 0,
    Contains = 1
}