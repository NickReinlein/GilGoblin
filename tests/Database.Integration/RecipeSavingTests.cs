using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace GilGoblin.Tests.Database.Integration;

public class RecipeSavingTests : SaveEntityToDbTests<RecipePoco>
{
    protected override RecipePoco GetEntity() => new()
    {
        AmountIngredient0 = 111,
        ItemIngredient0TargetId = 232,
        CanHq = true,
        CraftType = 3,
        CanQuickSynth = true,
        ResultQuantity = 3,
        RecipeLevelTable = 34,
        TargetItemId = 331
    };

    protected override RecipePoco GetModifiedEntity(RecipePoco entity) =>
        new() { Id = entity.Id, CanHq = false };

    protected override async Task ValidateResultSavedToDatabase(RecipePoco entity)
    {
        await using var ctx = GetDbContext();

        var result = await ctx.Recipe.SingleAsync(x => x.Id == entity.Id);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(entity.Id));
            Assert.That(result.CanHq, Is.EqualTo(entity.CanHq));
            Assert.That(result.CraftType, Is.EqualTo(entity.CraftType));
            Assert.That(result.CanQuickSynth, Is.EqualTo(entity.CanQuickSynth));
            Assert.That(result.ResultQuantity, Is.EqualTo(entity.ResultQuantity));
            Assert.That(result.RecipeLevelTable, Is.EqualTo(entity.RecipeLevelTable));
            Assert.That(result.TargetItemId, Is.EqualTo(entity.TargetItemId));
            Assert.That(result.AmountIngredient0, Is.EqualTo(entity.AmountIngredient0));
            Assert.That(result.AmountIngredient1, Is.EqualTo(entity.AmountIngredient1));
            Assert.That(result.AmountIngredient2, Is.EqualTo(entity.AmountIngredient2));
            Assert.That(result.AmountIngredient3, Is.EqualTo(entity.AmountIngredient3));
            Assert.That(result.AmountIngredient4, Is.EqualTo(entity.AmountIngredient4));
            Assert.That(result.AmountIngredient5, Is.EqualTo(entity.AmountIngredient5));
            Assert.That(result.AmountIngredient6, Is.EqualTo(entity.AmountIngredient6));
            Assert.That(result.AmountIngredient7, Is.EqualTo(entity.AmountIngredient7));
            Assert.That(result.AmountIngredient8, Is.EqualTo(entity.AmountIngredient8));
            Assert.That(result.AmountIngredient9, Is.EqualTo(entity.AmountIngredient9));
            Assert.That(result.ItemIngredient0TargetId, Is.EqualTo(entity.ItemIngredient0TargetId));
            Assert.That(result.ItemIngredient1TargetId, Is.EqualTo(entity.ItemIngredient1TargetId));
            Assert.That(result.ItemIngredient2TargetId, Is.EqualTo(entity.ItemIngredient2TargetId));
            Assert.That(result.ItemIngredient3TargetId, Is.EqualTo(entity.ItemIngredient3TargetId));
            Assert.That(result.ItemIngredient4TargetId, Is.EqualTo(entity.ItemIngredient4TargetId));
            Assert.That(result.ItemIngredient5TargetId, Is.EqualTo(entity.ItemIngredient5TargetId));
            Assert.That(result.ItemIngredient6TargetId, Is.EqualTo(entity.ItemIngredient6TargetId));
            Assert.That(result.ItemIngredient7TargetId, Is.EqualTo(entity.ItemIngredient7TargetId));
            Assert.That(result.ItemIngredient8TargetId, Is.EqualTo(entity.ItemIngredient8TargetId));
            Assert.That(result.ItemIngredient9TargetId, Is.EqualTo(entity.ItemIngredient9TargetId));
        });
    }
}