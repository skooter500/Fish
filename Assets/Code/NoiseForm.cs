using UnityEngine;
using System.Collections.Generic;
using BGE;

public class NoiseForm : MonoBehaviour {

    public Vector3 size; 
    public Vector2 noiseCount; 
    public Vector2 noiseStart;
    public Vector2 noiseDelta;
    public Color color;

    [Range(0.0f, 1.0f)]
    public float probabilityOfMountains;

    [Range(0.0f, 1.0f)]
    public float probabilityOfCraters;

    Vector2 tileSize; // The size of each tile

    Vector3[] initialVertices;
    Vector3[] initialNormals;
    Vector2[] meshUv;
    Color[] colours;
    int[] meshTriangles;
    Vector2[] uvSeq = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1f, 1)
                                      , new Vector2(1f, 1), new Vector2(1f, 0), new Vector2(0, 0)
                                };
    [HideInInspector]
    public float maxY;


    private bool generated = false;
    TextureGenerator textureGenerator;

    GameObject[] tiles = new GameObject[9];
    GameObject player;
    Texture2D texture;

    public enum Deformation { none, mountain, crater};

    Dictionary<Vector2, Deformation> deformations = new Dictionary<Vector2, Deformation>();
    
    private void CreateTiles()
    {

        Vector3 botomLeft = (transform.position - (size / 2.0f)) - size;
        int tileIndex = 0;
        for (int z = 0; z < 3; z ++)
        {
            for (int x = 0; x < 3; x ++)
            {
                GameObject tile = new GameObject();
                tile.transform.parent = this.transform;
                MeshRenderer renderer = tile.AddComponent<MeshRenderer>();
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = true;
                Mesh mesh = tile.AddComponent<MeshFilter>().mesh;
                mesh.Clear();
                Vector3 tilePos = new Vector3();
                tilePos.x = botomLeft.x + (x * size.x) + (size.x / 2.0f);
                tilePos.z = botomLeft.z + (z * size.z) + (size.z / 2.0f);
                tilePos.y = transform.position.y;
                tile.transform.position = tilePos;

                Vector2 noiseXY = new Vector2();
                noiseXY.x = noiseStart.x + (noiseCount.x * noiseDelta.x * x);
                noiseXY.y = noiseStart.y + (noiseCount.y * noiseDelta.y * z);                
                GenerateTile(tile, noiseXY, noiseDelta, color);
                tiles[tileIndex ++] = tile;
            }
        }
    }

    private int FindTile(Vector3 pos)
    {
        for (int i = 0; i < tiles.Length; i ++)
        {
            GameObject tile = tiles[i];
            Vector3 tileBottomLeft = tile.transform.position - (size / 2);
            Vector3 tileTopRight = tile.transform.position + (size / 2);
            if (pos.x > tileBottomLeft.x && pos.x <= tileTopRight.x && pos.z > tileBottomLeft.z && pos.z <= tileTopRight.z)
            {
                return i;
            }
        }
        return -1;
    }

    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }

    public NoiseForm()
    {
        size = new Vector3(100, 100, 100);
        noiseCount = new Vector3(10, 10);
        noiseStart = new Vector2(0, 0);
        noiseDelta = new Vector2(0.1f, 0.1f);
        probabilityOfCraters = 0.3f;
        probabilityOfMountains = 0.3f;
        color = HexToColor("0x2A59AD");
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, size);
    }

    void MaxY(float y)
    {
        if (y > maxY)
        {
            maxY = y;
        }
    }
    
    public void GenerateTile(GameObject tile, Vector2 noiseStart, Vector2 noiseDelta, Color color)
    {
        if (generated)
        {
            //return;
        }
        tileSize = new Vector2(size.x / noiseCount.x, size.z / noiseCount.y);

        MeshRenderer renderer = tile.GetComponent<MeshRenderer>();
        Mesh mesh = tile.GetComponent<MeshFilter>().mesh;
        mesh.Clear();

        int verticesPerSegment = 6;

        int vertexCount = verticesPerSegment * ((int)noiseCount.x) * ((int)noiseCount.y);
        
        initialVertices = new Vector3[vertexCount];
        initialNormals = new Vector3[vertexCount];
        meshUv = new Vector2[vertexCount];
        meshTriangles = new int[vertexCount];
        colours = new Color[vertexCount];

        Vector3 bottomLeft = -(size / 2);

        Vector2 noiseXY = noiseStart;
        int vertex = 0;
        float noiseHeight = size.y;
        Deformation deformation = CalculateDeformationType(noiseStart);
        for (int z = 0; z < noiseCount.y; z++)
        {
            noiseXY.x = noiseStart.x;
            for (int x = 0; x < noiseCount.x; x++)
            {
                int startVertex = vertex;
                // Calculate some stuff
                Vector3 sliceBottomLeft = bottomLeft + new Vector3(x * tileSize.x, Mathf.PerlinNoise(noiseXY.x, noiseXY.y) * noiseHeight, z * tileSize.y);
                Vector3 sliceTopLeft = bottomLeft + new Vector3(x * tileSize.x, Mathf.PerlinNoise(noiseXY.x, noiseXY.y + noiseDelta.y) * noiseHeight, (z + 1) * tileSize.y);
                Vector3 sliceTopRight = bottomLeft + new Vector3((x + 1) * tileSize.x, Mathf.PerlinNoise(noiseXY.x + noiseDelta.x, noiseXY.y + noiseDelta.y) * noiseHeight, (z + 1) * tileSize.y);
                Vector3 sliceBottomRight = bottomLeft + new Vector3((x + 1) * tileSize.x, Mathf.PerlinNoise(noiseXY.x + noiseDelta.x, noiseXY.y) * noiseHeight, z * tileSize.y);
                MaxY(sliceTopLeft.y); MaxY(sliceTopRight.y);

                int hashcode = tile.transform.position.GetHashCode();
                

                float deformHeight = (deformation == Deformation.mountain) ? noiseHeight * 3.0f : - noiseHeight * 3.0f;
                if (deformation == Deformation.mountain || deformation == Deformation.crater)
                {
                    sliceBottomLeft.y +=
                    Mathf.Sin(Utilities.Map(x, 0, noiseCount.x, 0, Mathf.PI))
                    * Mathf.Sin(Utilities.Map(z, 0, noiseCount.y, 0, Mathf.PI))
                    * deformHeight;
                    sliceTopLeft.y +=
                        Mathf.Sin(Utilities.Map(x, 0, noiseCount.x, 0, Mathf.PI))
                        * Mathf.Sin(Utilities.Map(z + 1, 0, noiseCount.y, 0, Mathf.PI))
                        * deformHeight;
                    sliceTopRight.y +=
                        Mathf.Sin(Utilities.Map(x + 1, 0, noiseCount.x, 0, Mathf.PI))
                        * Mathf.Sin(Utilities.Map(z + 1, 0, noiseCount.y, 0, Mathf.PI))
                        * deformHeight;
                    sliceBottomRight.y +=
                        Mathf.Sin(Utilities.Map(x + 1, 0, noiseCount.x, 0, Mathf.PI))
                        * Mathf.Sin(Utilities.Map(z, 0, noiseCount.y, 0, Mathf.PI))
                        * deformHeight;
                }               
                    
                // Make the vertices
                initialVertices[vertex++] = sliceBottomLeft;
                initialVertices[vertex++] = sliceTopLeft;
                initialVertices[vertex++] = sliceTopRight;
                initialVertices[vertex++] = sliceTopRight;
                initialVertices[vertex++] = sliceBottomRight;
                initialVertices[vertex++] = sliceBottomLeft;

                // Make the normals, UV's and triangles                
                for (int i = 0; i < 6; i++)
                {
                    //initialNormals[startVertex + i] = (i < 6) ? Vector3.forward : -Vector3.forward;
                    meshUv[startVertex + i] = uvSeq[i % 6];
                    meshTriangles[startVertex + i] = startVertex + i;
                }                
                noiseXY.x += noiseDelta.x;
            }
            noiseXY.y += noiseDelta.y;
        }

        for (int i = 0; i < meshUv.Length; i++)
        {
            Vector3 v = initialVertices[i] + (size / 2.0f);
            meshUv[i] = new Vector2(v.x / size.x, v.z / size.z);
        }

        mesh.vertices = initialVertices;
        mesh.uv = meshUv;
        mesh.triangles = meshTriangles;
        mesh.RecalculateNormals();

        //renderer.material.color = color;

        Shader shader = Shader.Find("Diffuse");
        Material material = new Material(shader);
        
        if (texture != null)
        {
            material.mainTexture = texture;
        }
        else
        {
            material.color = color;
        }
        renderer.material = material;
    }

    private Deformation EnsureAdjacentAreNot(Vector2 noiseStart, Deformation deformation)
    {
        Deformation ret = deformation;
        Vector2 noiseVal;
        // Check Forward
        noiseVal = CalculateNoiseTileStartFromTile(Vector2.up, noiseStart);
        if (deformations.ContainsKey(noiseVal) && deformations[noiseVal] == deformation && (deformation == Deformation.mountain || deformation == Deformation.crater))
        {
            ret = Deformation.none;
        }
        // Check behind
        noiseVal = CalculateNoiseTileStartFromTile(Vector2.down, noiseStart);
        if (deformations.ContainsKey(noiseVal) && deformations[noiseVal] == deformation && (deformation == Deformation.mountain || deformation == Deformation.crater))
        {
            ret = Deformation.none;
        }
        // Check left
        noiseVal = CalculateNoiseTileStartFromTile(Vector2.left, noiseStart);
        if (deformations.ContainsKey(noiseVal) && deformations[noiseVal] == deformation && (deformation == Deformation.mountain || deformation == Deformation.crater))
        {
            ret = Deformation.none;
        }
        // Check right
        noiseVal = CalculateNoiseTileStartFromTile(Vector2.right, noiseStart);
        if (deformations.ContainsKey(noiseVal) && deformations[noiseVal] == deformation && (deformation == Deformation.mountain || deformation == Deformation.crater))
        {
            ret = Deformation.none;
        }
        deformations[noiseStart] = ret;
        return ret;
    }

    private Deformation CalculateDeformationType(Vector2 noiseStart)
    {
        if (deformations.ContainsKey(noiseStart))
        {
            return deformations[noiseStart];
        }
        Deformation deformation = Deformation.none; 
        float p = Random.Range(0.0f, 1.0f);
        Debug.Log(p);
        if (p < probabilityOfMountains)
        {
            deformation = Deformation.mountain;
        } 
        else if (p >= probabilityOfMountains && p < probabilityOfMountains + probabilityOfCraters)
        {
            deformation = Deformation.crater;
        }
        return deformation;
        // return EnsureAdjacentAreNot(noiseStart, deformation);
    }

    void Start () {
        textureGenerator = GetComponent<TextureGenerator>();
        if (textureGenerator != null)
        {
            texture = textureGenerator.GenerateTexture();
        }
        CreateTiles();
        Random.seed = 42;
        player = GameObject.FindGameObjectWithTag("ovrplayer");
    }

    Vector2 CalculateNoiseTileStart(Vector2 tileOffset)
    {
        Vector2 noiseTileStart = new Vector2();
        noiseTileStart.x = noiseStart.x + (noiseDelta.x * noiseCount.x * tileOffset.x);
        noiseTileStart.y = noiseStart.y + (noiseDelta.y * noiseCount.y * tileOffset.y);
        return noiseTileStart;
    }

    Vector2 CalculateNoiseTileStartFromTile(Vector2 tileOffset, Vector2 tilePos)
    {
        Vector2 noiseTileStart = new Vector2();
        noiseTileStart.x = tilePos.x + (noiseDelta.x * noiseCount.x * tileOffset.x);
        noiseTileStart.y = tilePos.y + (noiseDelta.y * noiseCount.y * tileOffset.y);
        return noiseTileStart;
    }

    void Update () {
        int tileIndex = FindTile(player.transform.position);
        BoidManager.PrintFloat("Tile: ", tileIndex);
        //TODO: Diagonals
        switch (tileIndex)
        {
            case 7:
            {
                    // Player has moved forward one tile, so regenerate the 0th row
                GameObject[] newTiles = new GameObject[9];                
                newTiles[0] = tiles[3]; newTiles[1] = tiles[4]; newTiles[2] = tiles[5];
                newTiles[3] = tiles[6]; newTiles[4] = tiles[7]; newTiles[5] = tiles[8];
                newTiles[6] = tiles[0]; newTiles[7] = tiles[1]; newTiles[8] = tiles[2];
                tiles = newTiles;
                GenerateTile(tiles[6], CalculateNoiseTileStart(new Vector2(0, 3)), noiseDelta, color);
                GenerateTile(tiles[7], CalculateNoiseTileStart(new Vector2(1, 3)), noiseDelta, color);
                GenerateTile(tiles[8], CalculateNoiseTileStart(new Vector2(2, 3)), noiseDelta, color);
                tiles[6].transform.Translate(new Vector3(0, 0, size.z * 3.0f));
                tiles[7].transform.Translate(new Vector3(0, 0, size.z * 3.0f));
                tiles[8].transform.Translate(new Vector3(0, 0, size.z * 3.0f));
                noiseStart = CalculateNoiseTileStart(new Vector2(0, 1));
                break;
            }
            case 1:
            {
                // Player has moved backward one tile, so regenerate the 2nd row
                GameObject[] newTiles = new GameObject[9];
                newTiles[0] = tiles[6]; newTiles[1] = tiles[7]; newTiles[2] = tiles[8];
                newTiles[3] = tiles[0]; newTiles[4] = tiles[1]; newTiles[5] = tiles[2];
                newTiles[6] = tiles[3]; newTiles[7] = tiles[4]; newTiles[8] = tiles[5];
                tiles = newTiles;
                GenerateTile(tiles[0], CalculateNoiseTileStart(new Vector2(0, -1)), noiseDelta, Pallette.Random());
                GenerateTile(tiles[1], CalculateNoiseTileStart(new Vector2(1, -1)), noiseDelta, Pallette.Random());
                GenerateTile(tiles[2], CalculateNoiseTileStart(new Vector2(2, -1)), noiseDelta, Pallette.Random());
                tiles[0].transform.Translate(new Vector3(0, 0, -size.z * 3.0f));
                tiles[1].transform.Translate(new Vector3(0, 0, -size.z * 3.0f));
                tiles[2].transform.Translate(new Vector3(0, 0, -size.z * 3.0f));                
                noiseStart = CalculateNoiseTileStart(new Vector2(0, -1));                
                break;
            }
            case 3:
            {
                // Player has moved left one tile, so regenerate the 0th col
                GameObject[] newTiles = new GameObject[9];
                newTiles[0] = tiles[2]; newTiles[1] = tiles[0]; newTiles[2] = tiles[1];
                newTiles[3] = tiles[5]; newTiles[4] = tiles[3]; newTiles[5] = tiles[4];
                newTiles[6] = tiles[8]; newTiles[7] = tiles[6]; newTiles[8] = tiles[7];
                tiles = newTiles;
                GenerateTile(tiles[0], CalculateNoiseTileStart(new Vector2(-1, 0)), noiseDelta, Pallette.Random());
                GenerateTile(tiles[3], CalculateNoiseTileStart(new Vector2(-1, 1)), noiseDelta, Pallette.Random());
                GenerateTile(tiles[6], CalculateNoiseTileStart(new Vector2(-1, 2)), noiseDelta, Pallette.Random());
                tiles[0].transform.Translate(new Vector3(-size.x * 3.0f, 0, 0));
                tiles[3].transform.Translate(new Vector3(-size.x * 3.0f, 0, 0));
                tiles[6].transform.Translate(new Vector3(-size.x * 3.0f, 0, 0));
                noiseStart = CalculateNoiseTileStart(new Vector2(-1, 0));
                break;
            }
            case 5:
            {
                // Player has moved left one tile, so regenerate the 0th col
                GameObject[] newTiles = new GameObject[9];
                newTiles[0] = tiles[1]; newTiles[1] = tiles[2]; newTiles[2] = tiles[0];
                newTiles[3] = tiles[4]; newTiles[4] = tiles[5]; newTiles[5] = tiles[3];
                newTiles[6] = tiles[7]; newTiles[7] = tiles[8]; newTiles[8] = tiles[6];
                tiles = newTiles;
                GenerateTile(tiles[2], CalculateNoiseTileStart(new Vector2(3, 0)), noiseDelta, Pallette.Random());
                GenerateTile(tiles[5], CalculateNoiseTileStart(new Vector2(3, 1)), noiseDelta, Pallette.Random());
                GenerateTile(tiles[8], CalculateNoiseTileStart(new Vector2(3, 2)), noiseDelta, Pallette.Random());
                tiles[2].transform.Translate(new Vector3(size.x * 3.0f, 0, 0));
                tiles[5].transform.Translate(new Vector3(size.x * 3.0f, 0, 0));
                tiles[8].transform.Translate(new Vector3(size.x * 3.0f, 0, 0));
                noiseStart = CalculateNoiseTileStart(new Vector2(1, 0));
                break;
            }
        }
    }
}
