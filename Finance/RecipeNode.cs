using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Functions;

namespace GilGoblin.Finance
{
    public class RecipeNode<T> : TreeNode<T>
    {
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
