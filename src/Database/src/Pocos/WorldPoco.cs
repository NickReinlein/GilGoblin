using System.ComponentModel.DataAnnotations;

namespace GilGoblin.Database.Pocos;

public record WorldPoco : IIdentifiable
{
    public int Id { get; set; }
    [MaxLength(100)]
    public string Name { get; set; } = "";
    public int GetId() => Id;
}