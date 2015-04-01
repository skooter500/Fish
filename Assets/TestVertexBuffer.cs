using UnityEngine;
using System.Collections;

public class TestVertexBuffer : MonoBehaviour
{

    public Vector3 size;
    public Vector2 sliceCount; // The number of slices on each axis
    public Color horizontalColour;
    public Color verticalColour;

    public bool closed;

    Vector2 sliceSize; // The size of each slice

    Vector3[] initialVertices;
    Vector3[] initialNormals;
    Vector2[] meshUv;
    Color[] colours;
    int[] meshTriangles;
    //Vector2[] uvSeq = new Vector2[] { new Vector2(1, -1), new Vector2(0, 1), new Vector2(-1, -1) };
    Vector2[] uvSeqHoriz = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0.5f, 1)
                                      , new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(0, 0)
                                };
    Vector2[] uvSeqVert = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0.5f, 1)
                                       , new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(0, 0)
                                };

    public TestVertexBuffer()
    {
        size = new Vector3(100, 100, 100);
        sliceCount = new Vector3(10, 10);

        horizontalColour = Color.green;
        verticalColour = Color.red;
        closed = true;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, size);
    }

    public Texture2D CreateTexture()
    {
        int width = 2;
        int height = 1;

        return Resources.Load<Texture2D>("halfred");

        /*
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        texture.filterMode = FilterMode.Point;

        texture.SetPixel(0, 0, Color.red);
        texture.SetPixel(1, 0, Color.green);

        texture.Apply();
        return texture;
         */
    }

    void Start()
    {

        sliceSize = new Vector2(size.x / sliceCount.x, size.z / sliceCount.y);


        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        renderer.receiveShadows = true;
        if (renderer == null)
        {
            Debug.Log("Renderer is null 1");
        }

        Mesh mesh = gameObject.AddComponent<MeshFilter>().mesh;
        mesh.Clear();


        int vertexCount = 3;

        initialVertices = new Vector3[vertexCount];
        initialNormals = new Vector3[vertexCount];
        meshUv = new Vector2[vertexCount];
        meshTriangles = new int[vertexCount];

        int vertex = 0;

        // Make the vertices
        initialVertices[0] = new Vector3(-50, -50, 0);
        initialVertices[1] = new Vector3(-50, 50, 0);
        initialVertices[2] = new Vector3(50, 50, 0);
        initialNormals[0] = Vector3.forward;
        initialNormals[1] = Vector3.forward;
        initialNormals[2] = Vector3.forward;
        meshTriangles[0] = 0;
        meshTriangles[1] = 1;
        meshTriangles[2] = 2;
        meshUv[0] = new Vector2(0, 0);
        meshUv[1] = new Vector2(0, 1);
        meshUv[2] = new Vector2(1, 1);

        mesh.vertices = initialVertices;
        mesh.uv = meshUv;
        mesh.normals = initialNormals;
        mesh.triangles = meshTriangles;

        //mesh.RecalculateNormals();


        Shader shader = Shader.Find("Diffuse");
        Material material = new Material(shader);
        //material.color = color;
        material.mainTexture = CreateTexture();
        if (renderer == null)
        {
            Debug.Log("Renderer is null 2");
        }
        else
        {
            renderer.material = material;
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
