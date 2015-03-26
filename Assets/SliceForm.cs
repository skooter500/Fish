using UnityEngine;
using System.Collections;

public class SliceForm : MonoBehaviour {

    public Vector3 size; 
    public Vector2 sliceCount; // The number of slices on each axis
    public Vector2 noiseStart;
    public Vector2 noiseDelta;
    public Color color;

    Vector2 sliceSize; // The size of each slice

    Vector3[] initialVertices;
    Vector3[] initialNormals;
    Vector2[] meshUv;
    int[] meshTriangles;
        
    public SliceForm()
    {
        size = new Vector3(100, 100, 1);
        sliceCount = new Vector3(10, 10);
        noiseStart = new Vector2(0, 0);
        noiseDelta = new Vector2(0.01f, 0.01f);

        sliceSize = new Vector2(size.x / sliceCount.x, size.z / sliceCount.y);
    }

    
	
	void Start () {
        gameObject.AddComponent<MeshFilter>();
        MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
        if (renderer == null)
        {
            Debug.Log("Renderer is null 1");
        }
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();


        initialVertices = new Vector3[12 * (int)sliceCount.x];
        initialNormals = new Vector3[12 * (int)sliceCount.x];
        meshUv = new Vector2[12 * (int)sliceCount.x];
        meshTriangles = new int[12 * (int)sliceCount.x];

        Vector3 bottomLeft = transform.position - (size / 2);
        
        for (int y = 0; y < sliceCount.y; y++)
        {
            for (int x = 0; x < sliceCount.x; x++)
            {
                int triangle = 2 * x;
                int vertex = triangle * 12;

                Vector3 sliceBottonmLeft = bottomLeft + 
                

                // Make the front face
                initialVertices[vertex] = new Vector3();
            }
        }


        mesh.vertices = initialVertices;
        mesh.uv = meshUv;
        mesh.normals = initialNormals;
        mesh.triangles = meshTriangles;



        Shader shader = Shader.Find("Specular");
        Material material = new Material(Shader.Find("Specular"));
        material.color = color;
        material.mainTexture = Resources.Load<Texture2D>("white512x512");
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
	void Update () {
	
	}
}
