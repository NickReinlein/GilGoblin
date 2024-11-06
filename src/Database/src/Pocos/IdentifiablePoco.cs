using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Database.Pocos;

public interface IIdentifiable
{
    int GetId();
}

public record IdentifiablePoco : IIdentifiable
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }

    public int GetId() => Id;
}