using System.Text.Json;

namespace ItemStorage.Util;

public static class JsonElementHelper
{
    public static bool TryConvert(JsonElement element, Type type, out object result)
    {
        result = null;
        try
        {
            switch (type.Name)
            {
                case "String":
                    result = element.GetString();
                    return true;
                case "Int32":
                    var success = element.TryGetInt32(out var value);
                    if (success)
                        result = value;
                    return success;
                default:
                    return false;
            }
        }
        catch(Exception ex)
        {
            return false;
        }
            
    }
}