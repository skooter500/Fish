using UnityEngine;
using System.Collections;

public class SliceForm : MonoBehaviour {

    public Vector3 size; 
    public Vector2 sliceCount; // The number of slices on each axis
    public Vector2 noiseStart;
    public Vector2 noiseDelta;
    public Color horizontalColour;
    public Color verticalColour;
    public Vector3 seam;
    public bool closed;

    Vector2 sliceSize; // The size of each slice

    Vector3[] initialVertices;
    Vector3[] initialNormals;
    Vector2[] meshUv;
    Color[] colours;
    int[] meshTriangles;
    Vector2[] uvSeqHoriz = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0.5f, 1)
                                      , new Vector2(0.5f, 1), new Vector2(0.5f, 0), new Vector2(0, 0)
                                };
    Vector2[] uvSeqVert = new Vector2[] { new Vector2(0.5f, 0), new Vector2(1, 1), new Vector2(1, 1)
                                       , new Vector2(1, 1), new Vector2(1, 0), new Vector2(0.5f, 0)
                                };
    
    public SliceForm()
    {
        size = new Vector3(100, 100, 100);
        sliceCount = new Vector3(10, 10);
        noiseStart = new Vector2(0, 0);
        noiseDelta = new Vector2(0.1f, 0.1f);

        horizontalColour = Color.green;
        verticalColour = Color.red;
        closed = true;
        seam = new Vector3(0, 0, 0.1f);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, size);
    }

    public Texture2D CreateTexture()
    {
        /*
        Texture2D texture = new Texture2D(2, 1, TextureFormat.RGBAFloat, false);
        texture.filterMode = FilterMode.Point;

        texture.SetPixel(0, 0, Color.red);
        texture.SetPixel(1, 0, Color.green);
        */
        
        int width = 512;
        int height = 512;

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
        texture.filterMode = FilterMode.Point;
        for (int y = 0 ; y < height ; y ++)
        {
            for (int x = 0 ; x < width ; x ++)
            {
                texture.SetPixel(x, y, (x < width / 2) ? horizontalColour : verticalColour);                
            }
        }
        
        texture.Apply();
        return texture;
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

        int verticesPerHorizontalSlice = verticesPerSegment * (int) sliceCount.x; 
        int vertexCount = 0;
        if (closed)
        {
            // Go one extra slice horizontally and vertically
            sliceCount.x++;
            sliceCount.y++;
        }
        vertexCount = verticesPerSegment * ((int)sliceCount.x) * ((int)sliceCount.y);
        // Reduce by one vertical slice and one horizontal slice
        vertexCount -= (verticesPerSegment / 2) * (int)sliceCount.x;
        vertexCount -= (verticesPerSegment / 2) * (int)sliceCount.y;
        
        initialVertices = new Vector3[vertexCount];
        initialNormals = new Vector3[vertexCount];
        meshUv = new Vector2[vertexCount];
        meshTriangles = new int[vertexCount];
        colours = new Color[vertexCount];
    
        Vector3 bottomLeft = - (size / 2);

        Vector2 noiseXY = noiseStart;
        int vertex = 0;        

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

                int startVertex = vertex;                    
                // Make the horizontal slice
                if ((!closed && y == 0) || (closed && x == sliceCount.x - 1))
                {
                    // Skip a slice
                }
                else
                {

                    Vector3 horizSeam;
                    if (closed && (y == 0 || y == sliceCount.y - 1))
                    {
                        horizSeam = seam;
                    }
                    else
                    {
                        horizSeam = Vector3.zero;
                    }
                    // Make the vertices
                    initialVertices[vertex++] = sliceBottomLeft + horizSeam;
                    initialVertices[vertex++] = sliceTopLeft + horizSeam;
                    initialVertices[vertex++] = sliceTopRight - horizSeam;
                    initialVertices[vertex++] = sliceTopRight - horizSeam;
                    initialVertices[vertex++] = sliceBottomRight - horizSeam;
                    initialVertices[vertex++] = sliceBottomLeft + horizSeam;

                    // Back face
                    initialVertices[vertex++] = sliceTopRight - horizSeam;
                    initialVertices[vertex++] = sliceTopLeft + horizSeam;
                    initialVertices[vertex++] = sliceBottomLeft + horizSeam;
                    initialVertices[vertex++] = sliceBottomLeft + horizSeam;
                    initialVertices[vertex++] = sliceBottomRight - horizSeam;
                    initialVertices[vertex++] = sliceTopRight - horizSeam;

                    // Make the normals, UV's and triangles                
                    for (int i = 0; i < 12; i++)
                    {
                        initialNormals[startVertex + i] = (i < 6) ? Vector3.forward : -Vector3.forward;
                        meshUv[startVertex + i] = uvSeqHoriz[i % 6];
                        meshTriangles[startVertex + i] = startVertex + i;
                        colours[startVertex + i] = Color.green;
                    }
                }

                if ((!closed && x == 0) || (closed && y == sliceCount.y - 1))
                {
                    // Dont do a vertical slice
                }
                else
                {
                    startVertex = vertex;
                    Vector3 vertSeam;
                    if (closed && (x == 0 || x == sliceCount.x - 1))
                    {
                        vertSeam = Vector3.zero;
                    }
                    else
                    {
                        vertSeam = seam;
                    }
                    // Make the vertical slice
                    Vector3 sliceBottomForward = sliceBottomLeft + new Vector3(0, 0, sliceSize.y);
                    Vector3 sliceTopForward = sliceBottomLeft + new Vector3(0, Mathf.PerlinNoise(noiseXY.x, noiseXY.y + noiseDelta.y) * size.y, sliceSize.y);

                    initialVertices[vertex++] = sliceBottomLeft + vertSeam;
                    initialVertices[vertex++] = sliceTopLeft + vertSeam;
                    initialVertices[vertex++] = sliceTopForward - vertSeam;

                    initialVertices[vertex++] = sliceTopForward - vertSeam;
                    initialVertices[vertex++] = sliceBottomForward - vertSeam;
                    initialVertices[vertex++] = sliceBottomLeft + vertSeam;

                    // Back face
                    initialVertices[vertex++] = sliceTopForward - vertSeam;
                    initialVertices[vertex++] = sliceTopLeft + vertSeam;
                    initialVertices[vertex++] = sliceBottomLeft + vertSeam;

                    initialVertices[vertex++] = sliceBottomLeft + vertSeam;
                    initialVertices[vertex++] = sliceBottomForward - vertSeam;
                    initialVertices[vertex++] = sliceTopForward + vertSeam;

                    // Make the normals, UV's and triangles                
                    for (int i = 0; i < 12; i++)
                    {
                        initialNormals[startVertex + i] = (i < 6) ? Vector3.right : -Vector3.right;
                        meshUv[startVertex + i] = uvSeqVert[i % 6];
                        meshTriangles[startVertex + i] = startVertex + i;
                        colours[startVertex + i] = Color.red;
                    }
                }
                noiseXY.x += noiseDelta.x;         
            }
            noiseXY.y += noiseDelta.y;
        }


        mesh.vertices = initialVertices;
        mesh.uv = meshUv;
        mesh.normals = initialNormals;
        mesh.triangles = meshTriangles;
        mesh.colors = colours;

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
	void Update () {
	
	}
}
