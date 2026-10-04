using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Models;

public class ODataResponse<T>
{
    [JsonPropertyName("@odata.count")]
    public int? Count { get; set; }

    [JsonPropertyName("value")]
    public List<T> Value { get; set; } = new();
}
