using UnityEngine;
using System.Collections;


public class PerlinNoiseSampler:Sampler
{
    public PerlinNoiseSampler()
    {
        origin = new Vector2(0, 0);
        delta = new Vector2(0.2f, 0.2f);
    }

    public override float Sample(float x, float y)
    {
        return Mathf.PerlinNoise(x, y);
    }
}
