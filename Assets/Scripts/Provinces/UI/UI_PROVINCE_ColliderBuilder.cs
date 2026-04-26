using System.Collections.Generic;
using UnityEngine;

public static class UI_PROVINCE_ColliderBuilder
{
    public static void Rebuild(
        PolygonCollider2D polygonCollider,
        SpriteRenderer spriteRenderer,
        Object sourceForLog)
    {
        if (polygonCollider == null || spriteRenderer == null || spriteRenderer.sprite == null)
            return;

        polygonCollider.pathCount = 0;

        int shapeCount = spriteRenderer.sprite.GetPhysicsShapeCount();

        if (shapeCount <= 0)
        {
            Debug.LogWarning($"{sourceForLog.name} : le sprite n'a pas de Physics Shape.");
            return;
        }

        polygonCollider.pathCount = shapeCount;

        List<Vector2> shape = new();

        for (int i = 0; i < shapeCount; i++)
        {
            shape.Clear();
            spriteRenderer.sprite.GetPhysicsShape(i, shape);
            polygonCollider.SetPath(i, shape);
        }
    }
}