using System.Text.Json.Serialization;

namespace GilGoblin.Pocos;

public class RecipeClassLevelsPoco
{
    public int Id { get; set; }
    public int ClassJobId { get; set; }
    public int RecipeId { get; set; }
    public int Level { get; set; }

    public RecipeClassLevelsPoco() { }

    [JsonConstructor]
    public RecipeClassLevelsPoco(int id, int classJobId, int level)
    {
        ClassJobId = classJobId;
        RecipeId = id;
        Level = level;
    }
}
