using UnityEngine;
using System.Collections;

public abstract class Sampler : MonoBehaviour
{
    public Vector2 origin;
    public Vector2 delta;

    [HideInInspector]
    public Vector2 samplesPerTile;

    [HideInInspector]
    public Vector2 tileOrigin;

    public float height;

    public Sampler()
    {
        height = 1000.0f;
    }

    public abstract float Sample(float x, float y);
    public float SampleCell(float x, float y)
    {
        Vector2 p = CellToSample(x, y);
        return Sample(p.x, p.y) * height;
    }

    public Vector2 CellToSample(float x, float y)
    {
        return new Vector2(origin.x + (x * delta.x), origin.y + (y * delta.y));
    }

    public void TranslateOriginByTile(float x, float y)
    {
        origin.x = origin.x + samplesPerTile.x * delta.x * x;
        origin.y = origin.y + samplesPerTile.y * delta.y * y;
    }
}
