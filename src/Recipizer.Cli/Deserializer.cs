using System.Text.Json;
using System.Text.Json.Nodes;
using Recipizer.Cli.Models;

namespace Recipizer.Cli;

internal sealed class Deserializer
{
    public List<RecipeInitModel>? DeserializeRecipes(string data)
    {
        return JsonNode
            .Parse(data)
            ?["recipes"].Deserialize<List<RecipeInitModel>>(
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );
    }

    public string? DeserializeRecipeSource(string data)
    {
        return JsonNode
            .Parse(data)
            ?["recipeSource"].Deserialize<string>(
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );
    }
}