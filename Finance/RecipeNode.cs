using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Functions;

namespace GilGoblin.Finance
{
    public class RecipeList
    {
        public List<RecipeNode<RecipeDB>> list { get; set; }
            = new List<RecipeNode<RecipeDB>>();

        public RecipeList(){}

        public RecipeList(List<RecipeNode<RecipeDB>> list)
        {
            this.list = list;
        }
    }
    public class RecipeNode<T> : TreeNode<T>
    {
        public RecipeNode():base(){}
        public RecipeNode(T value) : base(value)
        {
            _value = value;
        }

        public RecipeNode(T value, TreeNode<T> parent) : this(value)
        {
            this.Parent = parent;
        }

        public RecipeNode(T value, TreeNode<T> parent, List<TreeNode<T>> list)
            : this(value, parent)
        {
            if (list != null)
            {
                this._children = list;
            }
        }

    }
}
