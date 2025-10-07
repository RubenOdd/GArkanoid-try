// (c) 2025 Sardorbek Mukhudinov
// License: 3-clause BSD license

using System.Text;
using Godot;

namespace GA.Garkanoid;

/// <summary>
/// Store information about collision between two 2D objects.
/// </summary>
/// <param name="point">Point of the collision.</param>
/// <param name="normal">The surface normal, perpendicular direction at the collsion point.</param>
/// <param name="delta">How far the colliding object penetrated into the surface.</param>
public class Hit(Vector2 point, Vector2 normal, Vector2 delta)
{
    public Vector2 Point { get; set; } = point;
    public Vector2 Normal { get; set; } = normal;
    public Vector2 Delta { get; set; } = delta;
}

/// <summary>
/// Store information about collision between two 3D objects
/// </summary>
/// <param name="point">Point of the collision.</param>
/// <param name="normal">The surface normal, perpendicular direction at the collsion point.</param>
/// <param name="delta">How far the colliding object penetrated into the surface.</param>
public class Hit3D(Vector3 point, Vector3 normal, Vector3 delta)
{
    public Vector3 Point { get; set; } = point;
    public Vector3 Normal { get; set; } = normal;
    public Vector3 Delta { get; set; } = delta;
}

public static class CustomPhysics
{
    // ===========================================================
    // Point vs Rectangle
    // ===========================================================

    /// <summary>
    /// Checks if a single point is inside the rectangle
    /// </summary>
    /// <param name="rect">The rectangle to check collision against</param>
    /// <param name="point">The point to test for collision</param>
    /// <returns>Hit object with collision info, or null if no collision</returns>
    public static Hit Intersects(Rect2 rect, Vector2 point)
    {
        // Get rect's center and half-dimensions
        Vector2 center = rect.Position + rect.Size / 2.0f;
        Vector2 extents = rect.Size / 2.0f;

        // Calculate vector from rectangle center to the test point
        Vector2 delta = point - center;

        // Calculate how far the point penetrates into the rectangle
        // pen == penetration
        float penX = extents.X - Mathf.Abs(delta.X);
        float penY = extents.Y - Mathf.Abs(delta.Y);

        // Cech if point is actually inside the rectangle
        if (penX < 0 || penY < 0) return null;

        Vector2 normal;
        Vector2 penVector;
        Vector2 collisionPoint;

        // Determine which edge of the rectangle is closest to the point
        // This tells us the collision normal (which direction to bounce)
        if (penX < penY)
        {
            // Point is closer to left/right edge
            float signX = Mathf.Sign(delta.X);      // -1 for left, +1 for right
            normal = new(signX, 0);                 // Points left or right
            penVector = new(penX * signX, 0);
            collisionPoint = new(center.X + (extents.X * signX), point.Y);
        }
        else
        {
            // Point is closer to top/bottom edge
            float signY = Mathf.Sign(delta.Y);
            normal = new(0, signY);
            penVector = new(0, penY * signY);
            collisionPoint = new(point.X, center.Y + (extents.Y * signY));
        }

        // Create and return collision info
        return new Hit(collisionPoint, normal, penVector);
    }

    // ===========================================================
    // Circle vs Rectangle
    // ===========================================================

    /// <summary>
    /// Checks if a circle collides with the rectangle
    /// </summary>
    /// <param name="rect">The rectangle to check collision against</param>
    /// <param name="point">The center of the circle</param>
    /// <param name="radius">The radius of the circle</param>
    /// <returns>Hit object with collision info, or null if no collision</returns>
    public static Hit Intersects(Rect2 rect, Vector2 point, float radius)
    {
        // Find the closest point on the rect to the circle center
        float closestX = Mathf.Clamp(point.X, rect.Position.X, rect.Position.X + rect.Size.X);
        float closestY = Mathf.Clamp(point.Y, rect.Position.Y, rect.Position.Y + rect.Size.Y);
        Vector2 closestPoint = new(closestX, closestY);

        // calculate distance from the circle center to closest point
        Vector2 delta = point - closestPoint;
        float distanceSquared = delta.LengthSquared();

        // Check if circle is close enough to touch the rectangle
        if (distanceSquared > (radius * radius)) return null;

        // Calculate collision normal (direction circle should bounce)
        // Normal points from the collision point toward the circle center
        Vector2 normal = delta.Normalized();

        // Calculate how far the circle penetrated into the rectangle
        float distance = Mathf.Sqrt(distanceSquared);
        float penDepth = radius - distance;
        Vector2 penVector = normal * penDepth;

        // Create and return collision information
        return new Hit(closestPoint, normal, penVector);
    }

    // ===========================================================
    // Sphere vs Box
    // ===========================================================

    /// <summary>
    /// Chech if a sphere collides with an Axis-Aligned Bounding Box
    /// </summary>
    /// <param name="box">The box to check the collision with</param>
    /// <param name="center">The center of the sphere</param>
    /// <param name="radius">The radius of the sphere</param>
    /// <returns>Hit object with collision info, or null if no collision</returns>
    public static Hit3D Intersects(Aabb box, Vector3 center, float radius)
    {
        float closestX = Mathf.Clamp(center.X, box.Position.X, box.End.X);
        float closestY = Mathf.Clamp(center.Y, box.Position.Y, box.End.Y);
        float closestZ = Mathf.Clamp(center.Z, box.Position.Z, box.End.Z);
        Vector3 closestPoint = new(closestX, closestY, closestZ);

        // the ditance from the center of the sphere to closest point
        Vector3 delta = center - closestPoint;
        float distanceSquared = delta.LengthSquared();

        if (distanceSquared > (radius * radius)) return null;

        Vector3 normal;

        if (distanceSquared > 0)
        {
            normal = delta.Normalized();
        }
        else
        {
            // if sphere center is inside box
            normal = (center - box.GetCenter()).Normalized();
        }

        // penetration
        float distance = Mathf.Sqrt(distanceSquared);
        float penDepth = radius - distance;
        Vector3 penVector = normal * penDepth;

        return new Hit3D(closestPoint, normal, penVector);
    }


    // ===========================================================
    // PHYSICS CALCULATION: Bounce/Reflection
    // ===========================================================

    public static Vector2 Bounce(Vector2 direction, Vector2 normal)
    {
        // Calculate the component of direction parallel to normal
        // This is the part if the movement "into" the surface
        // Vector2 parallelComponent = direction.Dot(normal) * normal;

        // Calcualte the component of direction perpendicular to the normal
        // Moving "along" the surface
        // Vector2 perpendicularComponent = direction - parallelComponent;

        // Reflect the direction by reversing the parallel component
        // Keep the perpendicular compoonent the same (sliding along the surface)
        // The parallel component gets reversed (bouncing away from the surface)
        // Vector2 reflectedDirection = perpendicularComponent - parallelComponent;
        // return reflectedDirection;

        // the simplified version using the standard reflection formula used in comp. graphics
        // R = L - 2(L·N)N 
        return direction - 2f * direction.Dot(normal) * normal;
    }
}