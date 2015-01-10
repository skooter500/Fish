using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace BGE
{
    public class Fish : MonoBehaviour
    {
        public float headHeight;
        public float headLength;

        public float bodyHeight;
        public float bodyLength;

        public float tailHeight;
        public float tailLength;        
        public float finLength;

        Vector3[] vertexSeq = new Vector3[] { new Vector3(0, 0.5f, -0.5f), new Vector3(0, 0, 0.5f), new Vector3(0, -0.5f, -0.5f)
                                        , new Vector3(0, -0.5f, -0.5f), new Vector3(0, 0, 0.5f), new Vector3(0, 0.5f, -0.5f) 
                                        , new Vector3(0, -0.5f, 0.5f), new Vector3(0, 0, -0.5f), new Vector3(0, 0.5f, 0.5f) 
                                        , new Vector3(0, 0.5f, 0.5f), new Vector3(0, 0, -0.5f), new Vector3(0, -0.5f, 0.5f) 
                                    };

        Vector3[] finSeq = new Vector3[] { new Vector3(0, 0.5f, 0.0f), new Vector3(0, 0.5f, -0.5f), new Vector3(0, 0.0f, 0.0f)
                                          , new Vector3(0, 0.0f, 0.0f), new Vector3(0, -0.5f, -0.5f), new Vector3(0, -0.5f, 0.0f)
                                          , new Vector3(0, 0.0f, 0.0f), new Vector3(0, 0.5f, -0.5f), new Vector3(0, 0.5f, 0.0f) 
                                          , new Vector3(0, -0.5f, 0.0f), new Vector3(0, -0.5f, -0.5f),new Vector3(0, 0.0f, 0.0f)
                                    };
        Vector2[] uvSeq = new Vector2[] { new Vector2(1, -1), new Vector2(0, 1), new Vector2(-1, -1) };

        public float extents = 0;
        public float border = 0;
        [Range(0, 2)]
        public float speedMultiplier;
        public float theta;
        float field = Mathf.PI / 10;
        float rotSpeed = 0.25f;
        public Color color;
                
        Fish()
        {
            extents = 50;
            border = extents * 0.1f;
            theta = 0;
            speedMultiplier = 1.0f;
            color = Color.cyan;
        }

        Vector3[] initialVertices;
        Vector3[] initialNormals;
        Vector2[] meshUv;
        int[] meshTriangles;
            
        // Use this for initialization
        void Start()
        {
            
            gameObject.AddComponent<MeshFilter>();
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            if (renderer == null)
            {
                Debug.Log("Renderer is null 1");
            }
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            mesh.Clear();
            int numSegments = 3;
            // We need 6 because every three vertices we reverse the normals
            initialVertices = new Vector3[6 * numSegments + finSeq.Length];
            initialNormals = new Vector3[6 * numSegments + finSeq.Length];
            meshUv = new Vector2[6 * numSegments + finSeq.Length];
            meshTriangles = new int[6 * numSegments + finSeq.Length];

            float[] heights = { headHeight, bodyHeight, tailHeight };
            float[] lengths = { headLength, bodyLength, tailLength };

            Vector3 offset = Vector3.zero;
            for (int i = 0; i < initialVertices.Length - finSeq.Length; i ++ )
            {
                int segment = i / 6;
                offset = - Vector3.forward * extents;
                initialVertices[i] = (vertexSeq[i % 12] * extents) + (offset * segment);
                initialNormals[i] = ((i / 3) % 2 == 1) ? Vector3.left : Vector3.right;
                meshUv[i] = uvSeq[i % 3];
                meshTriangles[i] = i;
            }

            // Add the fins
            int startAt = initialVertices.Length - finSeq.Length;
            offset = -Vector3.forward * (extents * 2.5f);
            for (int i = startAt; i < initialVertices.Length; i++)
            {
                int segment = i / 6;
                initialVertices[i] = (finSeq[i - startAt] * extents) + (offset);
                initialNormals[i] = ((i / 6) % 2 == 1) ? Vector3.left : Vector3.right;
                meshUv[i] = uvSeq[i % 3];
                meshTriangles[i] = i;
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
            renderer.material = material;
        }

        Vector3 RotateAroundPoint(Vector3 toRotate, Quaternion q, Vector3 point)
        {
            toRotate -= point;
            toRotate = q * toRotate;
            toRotate += point;
            return toRotate;
        }

        // Update is called once per frame
        void Update()
        {
            Boid steering = GetComponent<Boid>();
            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] meshVertices = mesh.vertices;
            Vector3[] meshNormals = mesh.normals;

            // Rotate the head
            Quaternion q = Quaternion.AngleAxis(Mathf.Rad2Deg * (Mathf.Sin(theta) * field * 1.5f), Vector3.up);
            
            for (int i = 0; i < meshVertices.Length; i++)
            {
                meshVertices[i] = initialVertices[i];
                meshNormals[i] = initialNormals[i];
            }

            // Rotate the head
            Vector3 rotationPoint = new Vector3(0, 0, -0.5f * extents);
            meshVertices[1] = RotateAroundPoint(initialVertices[1], q, rotationPoint);
            meshNormals[1] = RotateAroundPoint(initialNormals[1], q, Vector3.zero);
            meshVertices[4] = RotateAroundPoint(initialVertices[4], q, rotationPoint);
            meshNormals[4] = RotateAroundPoint(initialNormals[4], q, Vector3.zero);

            // Rotate the body    
            q = Quaternion.AngleAxis(Mathf.Rad2Deg * (Mathf.Sin(theta) * field), Vector3.up);
            for (int i = 5; i < meshVertices.Length; i++)
            {
                meshVertices[i] = RotateAroundPoint(initialVertices[i], q, rotationPoint);
                meshNormals[i] = RotateAroundPoint(initialNormals[i], q, Vector3.zero);
            }
            // Rotate the tail
            rotationPoint = meshVertices[13];
            

            for (int i = 12; i < meshVertices.Length; i++)
            {

                meshVertices[i] = RotateAroundPoint(meshVertices[i], q, rotationPoint);
                meshNormals[i] = RotateAroundPoint(meshNormals[i], q, Vector3.zero);

                meshVertices[i] = RotateAroundPoint(meshVertices[i], q, rotationPoint);
                meshNormals[i] = RotateAroundPoint(meshNormals[i], q, Vector3.zero);
            }
            float speed = steering.acceleration.magnitude;
            theta += speed * rotSpeed * Time.deltaTime * speedMultiplier;
            if (theta > Mathf.PI * 2.0f)
            {
                theta = 0.0f;
            }
            
            mesh.vertices = meshVertices;
            mesh.normals = meshNormals;
        }
    }
}