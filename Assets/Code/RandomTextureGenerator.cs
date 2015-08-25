using UnityEngine;
using System.Collections;

public class RandomTextureGenerator:TextureGenerator
{
    [HideInInspector]
    public Texture2D texture;

    NoiseForm noiseForm;
    // Use this for initialization

    public override Texture2D GenerateTexture()
    {
        int width = (int)noiseForm.samplesPerTile.x;
        int height = (int)noiseForm.samplesPerTile.y;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        texture.filterMode = FilterMode.Point;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, Pallette.Random());
            }
        }

        texture.Apply();
        return texture;
    }

    
    void Start()
    {
        noiseForm = GetComponent<NoiseForm>();
        if (noiseForm == null)
        {
            Debug.LogError("RandomTextureGenerator with no NoiseForm");
        }
    }
}
