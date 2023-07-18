using System.Text.Json.Serialization;

namespace GilGoblin.Pocos;

public class RecipeClassLevelsPoco
{
    public int ID { get; set; }
    public int ClassJobID { get; set; }
    public int RecipeID { get; set; }
    public int Level { get; set; }

    public RecipeClassLevelsPoco() { }

    [JsonConstructor]
    public RecipeClassLevelsPoco(int id, int classJobID, int level)
    {
        this.ClassJobID = classJobID;
        this.RecipeID = id;
        this.Level = level;
    }
}
