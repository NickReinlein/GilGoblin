using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.WebAPI
{
    internal class ItemInfoWeb : ItemInfo
    {
        public ICollection<ItemRecipeShortAPI> recipes { get; set; } = new List<ItemRecipeShortAPI>();

        public ItemInfoWeb() { } : base()
    }
}
