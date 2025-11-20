// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using Godot;
using System;
using System.Collections.Generic;

namespace GA.Common;

public static class NodeExtensions
{
    /// <summary>
    /// Gets a child node of the specified type.
    /// </summary>
    /// <typeparam name="TNode">The type of the child node to retrieve.</typeparam>
    /// <param name="parent">The parent node to search within (not included in the search).</param>
    /// <param name="recursive">Should the search include children's descendants?</param>
    /// <returns>The first child node of the specified type, or null if not found.</returns>
    public static TNode GetNode<TNode>(this Node parent, bool recursive = true)
        where TNode : Node
    {
        int childCount = parent.GetChildCount();

        for (int i = 0; i < childCount; ++i)
        {
            Node child = parent.GetChild(i);

            if (child is TNode result)
            {
                return result;
            }

            if (recursive && child.GetChildCount() > 0)
            {
                TNode recursiveResult = GetNode<TNode>(child, recursive);
                if (recursiveResult != null)
                {
                    return recursiveResult;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all child nodes of the specified type.
    /// </summary>
    /// <typeparam name="TNode">The type of the child nodes to retrieve.</typeparam>
    /// <param name="parent">The parent node to search within (not included in the search).</param>
    /// <param name="recursive">Should the search include children's descendants?</param>
    /// <returns>A list of all child nodes of the specified type (empty list if no child nodes found).
    /// </returns>
    public static IList<TNode> GetNodesInChildren<TNode>(this Node parent, bool recursive = true)
        where TNode : Node
    {
        List<TNode> results = new List<TNode>();
        int childCount = parent.GetChildCount();

        for (int i = 0; i < childCount; i++)
        {
            Node child = parent.GetChild(i);

            if (child is TNode result)
            {
                results.Add(result);
            }

            if (recursive && child.GetChildCount() > 0)
            {
                results.AddRange(GetNodesInChildren<TNode>(child, recursive));
            }
        }

        return results;
    }

    /// <summary>
    /// Returns the half of the rectangle size
    /// </summary>
    /// <param name="rectangle">The rectangle which extents are being calculated</param>
    /// <returns>Extents as Vector2</returns>
    public static Vector2 GetExtents(this Rect2 rectangle)
    {
        return rectangle.Size / 2;
    }

    public static Rect2 GetBoundingBox(this Sprite2D sprite)
    {
        // There's no texture , the resulting rect hsould be empty as well.
        if (sprite.Texture == null)
        {
            return new Rect2();
        }

        Vector2 textureSize = sprite.Texture.GetSize();

        // Scale the texture by sprite's scale
        // TODO: Should a local or Global scale be used?
        Vector2 scaledSize = textureSize * sprite.Scale;
        Vector2 offset = sprite.Offset;

        Vector2 topLeft = sprite.GlobalPosition -
            (scaledSize * (sprite.Centered ? 0.5f : 0.0f)) + offset;

        return new Rect2(topLeft, scaledSize);
    }

    /// <summary>
    /// The typed version of Resource's Duplicate method.
    /// </summary>
    /// <typeparam name="TResource">The type of the resource to duplicate.</typeparam>
    /// <param name="resource">The resource to duplicate.</param>
    /// <param name="deepCopy">If true, a deep copy is performed. Otherwise, a shallow copy is made.
    /// </param>
    /// <returns>The duplicated resource, or null if the original was null.</returns>
    public static TResource Duplicate<TResource>(this TResource resource, bool deepCopy = false)
        where TResource : Resource
    {
        if (resource == null)
        {
            return null;
        }

        return resource.Duplicate(subresources: deepCopy) as TResource;
    }
}
