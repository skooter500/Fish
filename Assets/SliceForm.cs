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
    Vector2[] uvSeq = new Vector2[] { new Vector2(1, -1), new Vector2(0, 1), new Vector2(-1, -1) };
    
    public SliceForm()
    {
        size = new Vector3(100, 100, 100);
        sliceCount = new Vector3(10, 10);
        noiseStart = new Vector2(0, 0);
        noiseDelta = new Vector2(0.1f, 0.1f);

        color = Color.green;

    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, size);
    }
	
	void Start () {

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


        int verticesPerSegment = 24;
        int verticesPerSlice = verticesPerSegment * (int) sliceCount.x; 

        initialVertices = new Vector3[verticesPerSegment * (int)sliceCount.x * (int)sliceCount.y];
        initialNormals = new Vector3[verticesPerSegment * (int)sliceCount.x * (int)sliceCount.y];
        meshUv = new Vector2[verticesPerSegment * (int)sliceCount.x * (int)sliceCount.y];
        meshTriangles = new int[verticesPerSegment * (int)sliceCount.x * (int)sliceCount.y];


        Vector3 bottomLeft = transform.position - (size / 2);

        Vector2 noiseXY = noiseStart;
        for (int y = 0; y < sliceCount.y; y++)
        {
            noiseXY.x = noiseStart.x;
            for (int x = 0; x < sliceCount.x; x++)
            {
               // Calculate some stuff
                Vector3 sliceBottomLeft = bottomLeft + new Vector3(x * sliceSize.x, 0, y * sliceSize.y);
                Vector3 sliceTopLeft = sliceBottomLeft + new Vector3(0, Mathf.PerlinNoise(noiseXY.x, noiseXY.y) * size.y);                
                Vector3 sliceTopRight = sliceBottomLeft + new Vector3(sliceSize.x, Mathf.PerlinNoise(noiseXY.x + noiseDelta.x, noiseXY.y) * size.y);
                Vector3 sliceBottomRight = sliceBottomLeft + new Vector3(sliceSize.x, 0, 0); 
             
                // Make the horizontal slice
                // Make the vertices
                int startVertex = (y * verticesPerSlice) + x * verticesPerSegment;                

                int vertex = startVertex;
                // Front face
                initialVertices[vertex++] = sliceBottomLeft;
                initialVertices[vertex++] = sliceTopLeft;
                initialVertices[vertex++] = sliceTopRight;
                initialVertices[vertex++] = sliceTopRight;
                initialVertices[vertex++] = sliceBottomRight;
                initialVertices[vertex++] = sliceBottomLeft;

                // Back face
                initialVertices[vertex++] = sliceTopRight;
                initialVertices[vertex++] = sliceTopLeft;
                initialVertices[vertex++] = sliceBottomLeft;
                initialVertices[vertex++] = sliceBottomLeft;
                initialVertices[vertex++] = sliceBottomRight;
                initialVertices[vertex++] = sliceTopRight;

                // Make the normals, UV's and triangles                
                for (int i = 0; i < 12; i++)
                {
                    initialNormals[startVertex + i] = (i < 6) ? -Vector3.forward : Vector3.forward;
                    meshUv[startVertex + i] = uvSeq[i % 3];
                    meshTriangles[startVertex + i] = startVertex + i;
                }           

                // Make the vertical slice
                Vector3 sliceBottomForward = sliceBottomLeft + new Vector3(0, 0, sliceSize.y);
                Vector3 sliceTopForward = sliceBottomLeft + new Vector3(0, Mathf.PerlinNoise(noiseXY.x, noiseXY.y + noiseDelta.y) * size.y, sliceSize.y);

                initialVertices[vertex++] = sliceBottomLeft;
                initialVertices[vertex++] = sliceTopLeft;
                initialVertices[vertex++] = sliceTopForward;

                initialVertices[vertex++] = sliceTopForward;
                initialVertices[vertex++] = sliceBottomForward;
                initialVertices[vertex++] = sliceBottomLeft;

                // Back face
                initialVertices[vertex++] = sliceTopForward;
                initialVertices[vertex++] = sliceTopLeft;
                initialVertices[vertex++] = sliceBottomLeft;

                initialVertices[vertex++] = sliceBottomLeft;
                initialVertices[vertex++] = sliceBottomForward;
                initialVertices[vertex++] = sliceTopForward;

                // Make the normals, UV's and triangles                
                for (int i = 0; i < 12; i++)
                {
                    initialNormals[startVertex + 12 + i] = (i < 6) ? Vector3.right: -Vector3.right;
                    meshUv[startVertex + 12 + i] = uvSeq[i % 3];
                    meshTriangles[startVertex + 12 + i] = startVertex + 12 + i;
                }     

                noiseXY.x += noiseDelta.x;
         
            }
            noiseXY.y += noiseDelta.y;
        }


        mesh.vertices = initialVertices;
        mesh.uv = meshUv;
        mesh.normals = initialNormals;
        mesh.triangles = meshTriangles;

        mesh.RecalculateNormals();

        Shader shader = Shader.Find("Diffuse");
        Material material = new Material(shader);
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
