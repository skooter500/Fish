using UnityEngine;
using System.Collections.Generic;
using BGE;
using System.Threading;
using com.youvisio;

public class NoiseForm : MonoBehaviour {

    public Vector3 size; 
    public Vector2 samplesPerTile; 
    public Color color;
    [Range(0.0f, 1.0f)]
    public float probabilityOfMountains;

    [Range(0.0f, 1.0f)]
    public float probabilityOfCraters;

    Vector2 tileSize; // The size of each tile in cells

    
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

    Dictionary<string, Deformation> deformations = new Dictionary<string, Deformation>();

    Sampler[] samplers;

    public float GetHeight(Vector3 pos)
    {
        pos.y = float.MaxValue;
        RaycastHit hitInfo;
        bool collided = Physics.Raycast(pos, Vector3.down, out hitInfo);
        if (collided)
        {
            return hitInfo.point.y;
        }
        else
        {
            return 0;
        }
    }
    
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
                tile.AddComponent<MeshCollider>();
                Vector3 tilePos = new Vector3();
                tilePos.x = botomLeft.x + (x * size.x) + (size.x / 2.0f);
                tilePos.z = botomLeft.z + (z * size.z) + (size.z / 2.0f);
                tilePos.y = transform.position.y;
                tile.transform.position = tilePos;
                
                GenerateTile(tile, new Vector2(x, z));
                tile.GetComponent<MeshCollider>().sharedMesh = mesh;
                tiles[tileIndex ++] = tile;

                // Add Physics and colliders
                //Rigidbody rigidBody = tile.AddComponent<Rigidbody>();
                //rigidBody.isKinematic = true;
               
                
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
        samplesPerTile = new Vector3(10, 10);
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

    class Arg
    {
        public GameObject t;
        public Vector2 tile;
    }

    class GeneratedMesh
    {
        public Vector3[] initialVertices;
        public Vector3[] initialNormals;
        public Vector2[] meshUv;
        public Color[] colours;
        public int[] meshTriangles;
    }


    List<BackgroundWorker> workers = new List<BackgroundWorker>();

    void GenerateTile(GameObject tileGameObject, Vector2 tile)
    {
        BackgroundWorker backgroundWorker = new BackgroundWorker();

        MeshRenderer renderer = tileGameObject.GetComponent<MeshRenderer>();
        Mesh mesh = tileGameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        backgroundWorker.DoWork += (o, a) =>
        {
            Arg aa = (Arg)a.Argument;            
            a.Result = GenerateTileAsync(aa.t, aa.tile);
        };

        backgroundWorker.RunWorkerCompleted += (o, a) =>
        {
            Debug.Log("Complete...");        
            GeneratedMesh gm = (GeneratedMesh)a.Result;
            for (int i = 0; i < gm.meshUv.Length; i++)
            {
                Vector3 v = gm.initialVertices[i] + (size / 2.0f);
                gm.meshUv[i] = new Vector2(v.x / size.x, v.z / size.z);
            }

            mesh.vertices = gm.initialVertices;
            mesh.uv = gm.meshUv;
            mesh.triangles = gm.meshTriangles;
            mesh.RecalculateNormals();

            //renderer.material.color = color;

            Shader shader = Shader.Find("Diffuse");

            Material material = null;
            if (renderer.material == null)
            {
                material = new Material(shader);
                renderer.material = material;
            }

            tileGameObject.GetComponent<MeshCollider>().sharedMesh = null;
            tileGameObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        };

        Arg args = new Arg();
        args.t = tileGameObject;
        args.tile = tile;

        workers.Add(backgroundWorker);
        backgroundWorker.RunWorkerAsync(args);
    }
    

    GeneratedMesh GenerateTileAsync(GameObject tileGameObject, Vector2 tile)
    {
        Debug.Log("Generating tile:" + tile);
        tileSize = new Vector2(size.x / samplesPerTile.x, size.z / samplesPerTile.y);

        int verticesPerSegment = 6;

        int vertexCount = verticesPerSegment * ((int)samplesPerTile.x) * ((int)samplesPerTile.y);

        GeneratedMesh gm = new GeneratedMesh();

        gm.initialVertices = new Vector3[vertexCount];
        gm.initialNormals = new Vector3[vertexCount];
        gm.meshUv = new Vector2[vertexCount];
        gm.meshTriangles = new int[vertexCount];

        gm.colours = new Color[vertexCount];

        Vector3 bottomLeft = -(size / 2);

        int vertex = 0;
        Deformation deformation = Deformation.none;

        int maxCellsPerFrame = 5000;
        int cellCount = 0;
        // What cell is the origin for this tile
        Vector2 tileCellOrigin = new Vector2(tile.x * samplesPerTile.x, tile.y * samplesPerTile.y);
        for (int z = 0; z < samplesPerTile.y; z++)
        {
            for (int x = 0; x < samplesPerTile.x; x++)
            {
                int startVertex = vertex;
                // Calculate some stuff
                Vector2 cell = new Vector2(tileCellOrigin.x + x, tileCellOrigin.y + z);
                Vector3 sliceBottomLeft = bottomLeft + new Vector3(x * tileSize.x, 0, z * tileSize.y);
                Vector3 sliceTopLeft = bottomLeft + new Vector3(x * tileSize.x, 0, (z + 1) * tileSize.y);
                Vector3 sliceTopRight = bottomLeft + new Vector3((x + 1) * tileSize.x, 0 , (z + 1) * tileSize.y);
                Vector3 sliceBottomRight = bottomLeft + new Vector3((x + 1) * tileSize.x, 0, z * tileSize.y);
                MaxY(sliceTopLeft.y); MaxY(sliceTopRight.y);

                // Add all the samplers together to make the height
                foreach(Sampler sampler in samplers)
                {
                    sliceBottomLeft.y += sampler.SampleCell(cell.x, cell.y);
                    sliceTopLeft.y += sampler.SampleCell(cell.x, cell.y + 1);
                    sliceTopRight.y += sampler.SampleCell(cell.x + 1, cell.y + 1);
                    sliceBottomRight.y += sampler.SampleCell(cell.x + 1, cell.y);
                }

                //int hashcode = tileGameObject.transform.position.GetHashCode();
                

                //float deformHeight = (deformation == Deformation.mountain) ? noiseHeight * 3.0f : - noiseHeight * 3.0f;
                //if (deformation == Deformation.mountain || deformation == Deformation.crater)
                //{
                //    float angle = Mathf.PI;
                //    sliceBottomLeft.y +=
                //    Mathf.Sin(Utilities.Map(x, 0, samplesPerTile.x, 0, angle))
                //    * Mathf.Sin(Utilities.Map(z, 0, samplesPerTile.y, 0, angle))
                //    * deformHeight;
                //    sliceTopLeft.y +=
                //        Mathf.Sin(Utilities.Map(x, 0, samplesPerTile.x, 0, angle))
                //        * Mathf.Sin(Utilities.Map(z + 1, 0, samplesPerTile.y, 0, angle))
                //        * deformHeight;
                //    sliceTopRight.y +=
                //        Mathf.Sin(Utilities.Map(x + 1, 0, samplesPerTile.x, 0, angle))
                //        * Mathf.Sin(Utilities.Map(z + 1, 0, samplesPerTile.y, 0, angle))
                //        * deformHeight;
                //    sliceBottomRight.y +=
                //        Mathf.Sin(Utilities.Map(x + 1, 0, samplesPerTile.x, 0, angle))
                //        * Mathf.Sin(Utilities.Map(z, 0, samplesPerTile.y, 0, angle))
                //        * deformHeight;
                //}               
                    
                // Make the vertices
                gm.initialVertices[vertex++] = sliceBottomLeft;
                gm.initialVertices[vertex++] = sliceTopLeft;
                gm.initialVertices[vertex++] = sliceTopRight;
                gm.initialVertices[vertex++] = sliceTopRight;
                gm.initialVertices[vertex++] = sliceBottomRight;
                gm.initialVertices[vertex++] = sliceBottomLeft;

                // Make the normals, UV's and triangles                
                for (int i = 0; i < 6; i++)
                {
                    //initialNormals[startVertex + i] = (i < 6) ? Vector3.forward : -Vector3.forward;
                    gm.meshUv[startVertex + i] = uvSeq[i % 6];
                    gm.meshTriangles[startVertex + i] = startVertex + i;
                }
            }            
        }
        return gm;
    }
    
    //private Deformation EnsureAdjacentAreNot(Vector2 noiseStart, Deformation deformation)
    //{
    //    Deformation ret = deformation;
    //    Vector2 noiseVal;
    //    // Check Forward
    //    noiseVal = CalculateNoiseTileStartFromTile(Vector2.up, noiseStart);
    //    if (deformations.ContainsKey("" + noiseVal) && deformations["" + noiseVal] == deformation && (deformation == Deformation.mountain || deformation == Deformation.crater))
    //    {
    //        ret = Deformation.none;
    //    }
    //    // Check behind
    //    noiseVal = CalculateNoiseTileStartFromTile(Vector2.down, noiseStart);
    //    if (deformations.ContainsKey("" + noiseVal) && deformations["" + noiseVal] == deformation && (deformation == Deformation.mountain || deformation == Deformation.crater))
    //    {
    //        ret = Deformation.none;
    //    }
    //    // Check left
    //    noiseVal = CalculateNoiseTileStartFromTile(Vector2.left, noiseStart);
    //    if (deformations.ContainsKey("" + noiseVal) && deformations["" + noiseVal] == deformation && (deformation == Deformation.mountain || deformation == Deformation.crater))
    //    {
    //        ret = Deformation.none;
    //    }
    //    // Check right
    //    noiseVal = CalculateNoiseTileStartFromTile(Vector2.right, noiseStart);
    //    if (deformations.ContainsKey("" + noiseVal) && deformations["" + noiseVal] == deformation && (deformation == Deformation.mountain || deformation == Deformation.crater))
    //    {
    //        ret = Deformation.none;
    //    }
    //    deformations["" + noiseStart] = ret;
    //    return ret;
    //}

    private Deformation CalculateDeformationType(Vector2 noiseStart)
    {

        return Deformation.none;

       // if (deformations.ContainsKey("" + noiseStart))
       // {
       //     return deformations["" + noiseStart];
       // }
       // Deformation deformation = Deformation.none; 
       // float p = Random.Range(0.0f, 1.0f);
       // if (p < probabilityOfMountains)
       // {
       //     deformation = Deformation.mountain;
       // } 
       // else if (p >= probabilityOfMountains && p < probabilityOfMountains + probabilityOfCraters)
       // {
       //     deformation = Deformation.crater;
       // }
        
       //return deformation;
        //return EnsureAdjacentAreNot(noiseStart, deformation);
    }

    void Awake () {
        samplers = GetComponents<Sampler>();
        if (samplers == null)
        {
            Debug.Log("Sampler is null! Add a sampler to the NoiseForm");
        }
        else
        {
            foreach(Sampler sampler in samplers)
            {
                sampler.samplesPerTile = samplesPerTile;
            }            
        }

        CreateTiles();
        Random.seed = 42;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Start()
    {
        textureGenerator = GetComponent<TextureGenerator>();
        if (textureGenerator != null)
        {
            texture = textureGenerator.GenerateTexture();
        }
       foreach(GameObject tile in tiles)
       {
           tile.GetComponent<Renderer>().material.SetTexture(0, texture);
       }
    }

    private void TraanslateSamplersByTile(float x, float y)
    {
        foreach(Sampler sampler in samplers)
        {
            sampler.TranslateOriginByTile(x, y);
        }
    }
   

    void Update () {
        int tileIndex = FindTile(player.transform.position);
        BoidManager.PrintFloat("Tile: ", tileIndex);
        //TODO: Diagonals
        switch (tileIndex)
        {
            case 7:
            {
                // Player has moved forward one tile
                GameObject[] newTiles = new GameObject[9];                
                newTiles[0] = tiles[3]; newTiles[1] = tiles[4]; newTiles[2] = tiles[5];
                newTiles[3] = tiles[6]; newTiles[4] = tiles[7]; newTiles[5] = tiles[8];
                newTiles[6] = tiles[0]; newTiles[7] = tiles[1]; newTiles[8] = tiles[2];
                tiles = newTiles;
                // Move the sampler forward one tile
                TraanslateSamplersByTile(0, 1);
                GenerateTile(tiles[6], new Vector2(0, 2));
                GenerateTile(tiles[7], new Vector2(1, 2));
                GenerateTile(tiles[8], new Vector2(2, 2));
                // Translate the tiles forward
                tiles[6].transform.Translate(new Vector3(0, 0, size.z * 3.0f));
                tiles[7].transform.Translate(new Vector3(0, 0, size.z * 3.0f));
                tiles[8].transform.Translate(new Vector3(0, 0, size.z * 3.0f));
                
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
                // Move the sampler backward one tile
                TraanslateSamplersByTile(0, -1); 
                GenerateTile(tiles[0], new Vector2(0, 0));
                GenerateTile(tiles[1], new Vector2(1, 0));
                GenerateTile(tiles[2], new Vector2(2, 0));
                tiles[0].transform.Translate(new Vector3(0, 0, -size.z * 3.0f));
                tiles[1].transform.Translate(new Vector3(0, 0, -size.z * 3.0f));
                tiles[2].transform.Translate(new Vector3(0, 0, -size.z * 3.0f));                
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
                TraanslateSamplersByTile(-1, 0); 
                GenerateTile(tiles[0], new Vector2(0, 0));
                GenerateTile(tiles[3], new Vector2(0, 1));
                GenerateTile(tiles[6], new Vector2(0, 2));
                tiles[0].transform.Translate(new Vector3(-size.x * 3.0f, 0, 0));
                tiles[3].transform.Translate(new Vector3(-size.x * 3.0f, 0, 0));
                tiles[6].transform.Translate(new Vector3(-size.x * 3.0f, 0, 0));
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
                TraanslateSamplersByTile(1, 0); 
                GenerateTile(tiles[2], new Vector2(2, 0));
                GenerateTile(tiles[5], new Vector2(2, 1));
                GenerateTile(tiles[8], new Vector2(2, 2));
                tiles[2].transform.Translate(new Vector3(size.x * 3.0f, 0, 0));
                tiles[5].transform.Translate(new Vector3(size.x * 3.0f, 0, 0));
                tiles[8].transform.Translate(new Vector3(size.x * 3.0f, 0, 0));
                break;
            }
        }
        for(int i = workers.Count - 1 ; i >=  0 ; i --)
        {
            if (workers[i].IsBusy) 
            {
                workers[i].Update();
            }
            else
            {
                workers.Remove(workers[i]);
            }
        }
    }
}
