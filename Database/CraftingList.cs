using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;


namespace GilGoblin.Database
{
    public enum CraftingClass
    {
        carpenter, blacksmith, armorer, goldsmith, 
        leatherworker, weaver, alchemist, culinarian
    }
    public enum GatheringClass
    {
        miner,
        botanist,
        fisher
    }

    public class CraftingList
    {
        /// <summary>
        /// Gets a list of crafting ID's for a specific crafting job (ie: Armorer)
        /// </summary>
        /// <returns></returns>
        public static List<RecipeLookup> getListOfCraftingLookup(){
            List<RecipeLookup> list = RecipeLookup.getRecipeList();            
            return list;        
        }

        public static List<int> getListOfAllCraftableItemIDs()
        {
            List<int> craftables = new List<int>();
            List<RecipeLookup> list = getListOfCraftingLookup();
            foreach (RecipeLookup item in list)
            {
                craftables.Add(item.itemID);
            }
            return craftables;
        }

        public static List<int> getListOfCraftableItemIDsByClass(CraftingClass crafter)
        {
            List<int> craftables = new List<int>();
            List<RecipeLookup> list = getListOfCraftingLookup();
            foreach (RecipeLookup item in list)
            {
                bool addMe = false;

                switch (crafter)
                {
                    case CraftingClass.alchemist:
                        if (item.alchemistRecipe > 0) { addMe = true; }
                        break;
                    case CraftingClass.blacksmith:
                        if (item.blacksmithRecipe > 0) { addMe = true; }
                        break;
                    case CraftingClass.goldsmith:
                        if (item.goldsmithRecipe > 0) { addMe = true; }
                        break;
                    case CraftingClass.carpenter:
                        if (item.carpenterRecipe > 0) { addMe = true; }
                        break;
                    case CraftingClass.armorer:
                        if (item.armorerRecipe > 0) { addMe = true; }
                        break;
                    case CraftingClass.culinarian:
                        if (item.culinarianRecipe > 0) { addMe = true; }
                        break;
                    case CraftingClass.leatherworker:
                        if (item.leatherworkerRecipe > 0) { addMe = true; }
                        break;
                    case CraftingClass.weaver:
                        if (item.weaverRecipe > 0) { addMe = true; }
                        break;
                    default:
                        addMe = false;
                        break;
                }

                if (addMe) { craftables.Add(item.itemID); }
            }
            return craftables;
        }
    }

    [IgnoreFirst]
    [DelimitedRecord(",")]
    public class RecipeLookup
    {
        //#,CRP,BSM,ARM,GSM,LTW,WVR,ALC,CUL
        public int itemID, carpenterRecipe = 0, blacksmithRecipe = 0, armorerRecipe = 0, goldsmithRecipe = 0, leatherworkerRecipe = 0, weaverRecipe = 0, alchemistRecipe = 0, culinarianRecipe = 0;

        public static List<RecipeLookup> getRecipeList()
        {
            List<RecipeLookup> list = new List<RecipeLookup>();

            var engine = new FileHelperEngine<RecipeLookup>();
            var result = engine.ReadFile("RecipeLookup.csv");

            foreach (var recipe in result)
            {
                list.Add(recipe);
            }

            return list;
        }
    }
}
