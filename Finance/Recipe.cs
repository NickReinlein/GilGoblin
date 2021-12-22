using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Functions;

namespace GilGoblin.Finance
{
    public class Recipe<T> : TreeNode<T>
    {
        public Recipe(T value) : base(value)
        {
            _value = value;
        }

        public Recipe(T value, TreeNode<T> parent) : this(value)
        {
            this.Parent = parent;
        }

        public Recipe(T value, TreeNode<T> parent, List<TreeNode<T>> list)
            : this(value, parent)
        {
            if (list != null)
            {
                this._children = list;
            }
        }

    }
}
