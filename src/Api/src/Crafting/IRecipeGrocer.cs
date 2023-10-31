using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Crafting;

public interface IRecipeGrocer
{
    public Task<IEnumerable<IngredientPoco>> BreakdownRecipeById(int recipeId);
    public Task<IEnumerable<IngredientPoco>?> BreakdownItem(int itemId);
}
