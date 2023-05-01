using UnityEngine;

public static class TriangleColliderIntersection
{
    // Tests if a sphere collider intersects a triangle defined by its three vertices
    public static bool TestTriangle(Collider collider, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        // Calculate the sphere position in the triangle space
        Vector3 center = collider.bounds.center;
        Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
        Vector3 centerToTriangle = v1 - center;
        float distance = Vector3.Dot(centerToTriangle, normal);
        Vector3 spherePosition = center + normal * distance;

        // Test if the sphere intersects the triangle
        Vector3 closestPoint = ClosestPointOnTriangleToPoint(spherePosition, v1, v2, v3);
        return Vector3.Distance(closestPoint, spherePosition) <= collider.bounds.extents.magnitude;
    }

    // Calculates the closest point on a triangle to a given point
    private static Vector3 ClosestPointOnTriangleToPoint(Vector3 point, Vector3 v1, Vector3 v2, Vector3 v3)
    {
        Vector3 edge1 = v2 - v1;
        Vector3 edge2 = v3 - v1;
        Vector3 v1ToPoint = point - v1;

        float dot11 = Vector3.Dot(edge1, edge1);
        float dot12 = Vector3.Dot(edge1, edge2);
        float dot22 = Vector3.Dot(edge2, edge2);
        float dot1p = Vector3.Dot(edge1, v1ToPoint);
        float dot2p = Vector3.Dot(edge2, v1ToPoint);

        float denominator = dot11 * dot22 - dot12 * dot12;

        float u = (dot22 * dot1p - dot12 * dot2p) / denominator;
        float v = (dot11 * dot2p - dot12 * dot1p) / denominator;

        if (u < 0.0f)
            return v1;

        if (v < 0.0f)
            return v2;

        if (u + v > 1.0f)
            return v3;

        return v1 + u * edge1 + v * edge2;
    }
}