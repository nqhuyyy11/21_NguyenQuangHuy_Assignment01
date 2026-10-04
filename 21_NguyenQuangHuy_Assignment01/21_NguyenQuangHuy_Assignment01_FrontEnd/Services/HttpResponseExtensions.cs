using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Services;

public static class HttpResponseExtensions
{
    public static async Task<string> ReadErrorMessageAsync(this HttpResponseMessage response)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content))
            {
                return $"Error: {response.StatusCode}";
            }

            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                if (doc.RootElement.TryGetProperty("message", out var msgProp) && msgProp.ValueKind == JsonValueKind.String)
                {
                    return msgProp.GetString() ?? content;
                }

                if (doc.RootElement.TryGetProperty("errors", out var errorsProp) && errorsProp.ValueKind == JsonValueKind.Object)
                {
                    var errorList = new System.Collections.Generic.List<string>();
                    foreach (var prop in errorsProp.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var err in prop.Value.EnumerateArray())
                            {
                                errorList.Add(err.GetString() ?? "");
                            }
                        }
                    }
                    if (errorList.Count > 0)
                    {
                        return string.Join(" ", errorList);
                    }
                }

                if (doc.RootElement.TryGetProperty("title", out var titleProp) && titleProp.ValueKind == JsonValueKind.String)
                {
                    return titleProp.GetString() ?? content;
                }
            }
            return content;
        }
        catch
        {
            return $"Error: {response.StatusCode}";
        }
    }
}
