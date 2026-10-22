using System.Collections.Generic;
using Godot;

namespace Godot_Util.Ext;

public static class NodeExtensions
{
#nullable enable
    
    public static IEnumerable<T> Children<T>(this Node node, bool recurse) where T : Node
    {
        for (var i = 0; i < node.GetChildCount(); i++)
        {
            var child = node.GetChildOrNull<T>(i);
            if (child == null)
            {
                continue;
            }
            
            yield return child;

            if (recurse)
            {
                foreach (var grandchild in child.Children<T>(recurse))
                {
                    yield return grandchild;
                }
            }
        }
    }

    public static IEnumerable<Node> Ancestors(this Node node)
    {
        node = node.GetParent();
        while (node != null)
        {
            yield return node;
            node = node.GetParent();
        }
    }
}