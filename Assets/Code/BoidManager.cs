using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BGE.States;

namespace BGE
{
    public class BoidManager : MonoBehaviour
    {
        StringBuilder message = new StringBuilder();

        [Header("Cell Space Partitioning")]
        public bool cellSpacePartitioning;
        public float cellWidth;
        public float worldHalfExtents;

        [Header("Debugging")]
        public bool showMessages;

        [Header("Debugging")]

        public Space space;
        static BoidManager instance;
        // Use this for initialization
        GUIStyle style = new GUIStyle();
                
        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        void Start()
        {
            instance = this;
            Screen.showCursor = false;

            style.fontSize = 18;
            style.normal.textColor = Color.white;

            space = new Space();
            
        }

        public static BoidManager Instance
        {
            get
            {
                return instance;
            }
        }

        void OnGUI()
        {
            if (showMessages)
            {
                GUI.Label(new Rect(0, 0, Screen.width, Screen.height), "" + message, style);
            }
            if (Event.current.type == EventType.Repaint)
            {
                message.Length = 0;
            }

            if (Event.current.type == EventType.KeyDown)
            {

                if (Event.current.keyCode == KeyCode.F4)
                {
                    showMessages = ! showMessages;
                }
            
                if (Event.current.keyCode == KeyCode.F8)
                {
                    cellSpacePartitioning = !cellSpacePartitioning;
                }
                                
                if (Event.current.keyCode == KeyCode.Escape)
                {
                    Application.Quit();
                }                
            }
        }

        public static void PrintMessage(string message)
        {
            if (instance != null)
            {
                Instance.message.Append(message + "\n");
            }
        }

        public static void PrintFloat(string message, float f)
        {
            if (instance != null)
            {
                Instance.message.Append(message + ": " + f + "\n");
            }
        }

        public static void PrintVector(string message, Vector3 v)
        {
            if (instance != null)
            {
                Instance.message.Append(message + ": (" + v.x + ", " + v.y + ", " + v.z + ")\n");
            }
        }

        // Update is called once per frame
        void Update()
        {
            int fps = (int)(1.0f / Time.deltaTime);
            PrintFloat("FPS: ", fps);      
        }
    }
}
