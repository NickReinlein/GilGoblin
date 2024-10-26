using System.ComponentModel.DataAnnotations;

namespace GilGoblin.Database.Pocos;

public record WorldPoco : IdentifiablePoco
{
    [MaxLength(100)] public string Name { get; set; } = "";
}