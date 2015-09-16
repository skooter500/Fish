using UnityEngine;
using System.Collections;

public class ForceController : MonoBehaviour {
    Camera ovrCamera;
    float speed = 100.0f;

    public bool vrMode;

    Rigidbody rigidBody;

    public ForceController()
    {
        vrMode = true;
    }

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        rigidBody = GetComponent<Rigidbody>();

        if (vrMode && GameObject.FindGameObjectWithTag("Player") != null)
        {
            ovrCamera = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Camera>();
        }

        rigidBody.freezeRotation = true;        
    }

    void Yaw(float angle)
    {
        //rigidBody.AddTorque(Vector3.up * angle * 150);
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
        transform.rotation = rot * transform.rotation;
    }

    void Roll(float angle)
    {
        Quaternion rot = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rot * transform.rotation;
    }

    void Pitch(float angle)
    {
        float invcosTheta1 = Vector3.Dot(transform.forward, Vector3.up);
        float threshold = 0.95f;
        if ((angle > 0 && invcosTheta1 < (-threshold)) || (angle < 0 && invcosTheta1 > (threshold)))
        {
            return;
        }

        // A pitch is a rotation around the right vector
        Quaternion rot = Quaternion.AngleAxis(angle, transform.right);

        transform.rotation = rot * transform.rotation;
    }

    void Walk(float units)
    {
        if (ovrCamera != null)
        {
            rigidBody.AddForce(ovrCamera.transform.forward* units);
        }
        else
        {
            rigidBody.AddForce(transform.forward* units);
        }
    }

    void Fly(float units)
    {
        rigidBody.AddForce(Vector3.up * units);
    }

    void Strafe(float units)
    {
        if (ovrCamera != null)
        {
            rigidBody.AddForce(ovrCamera.transform.right* units);
        }
        else
        {
            rigidBody.AddForce(transform.right * units);
        }
    }

    int test = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        float mouseX, mouseY;
        float speed = this.speed;

        float runAxis = Input.GetAxis("Run Axis");
        float angularSpeed = 60.0f;

        if (Input.GetKey(KeyCode.LeftShift) || runAxis != 0)
        {
            speed *= 2f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            Walk(speed);
        }

        if (Input.GetKey(KeyCode.S))
        {
            Walk(-speed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            Strafe(-speed);
        }

        if (Input.GetKey(KeyCode.D))
        {
            Strafe(speed);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            Roll(-Time.deltaTime * angularSpeed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            Roll(Time.deltaTime * angularSpeed);
        }
        if (Input.GetKey(KeyCode.R))
        {
            Fly(speed);
        }
        if (Input.GetKey(KeyCode.F))
        {
            Fly(-speed);
        }

        //BoidManager.PrintVector("OVR Forward: ", ovrCamera.transform.forward);

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");


        Yaw(mouseX * Time.deltaTime * angularSpeed);
        float contYaw = Input.GetAxis("Yaw Axis");
        float contPitch = Input.GetAxis("Pitch Axis");
        Yaw(contYaw * Time.deltaTime * angularSpeed);

         // If in Rift mode, dont pitch
        if (ovrCamera == null)
        {
            Pitch(-mouseY * Time.deltaTime * angularSpeed);
            Pitch(contPitch * Time.deltaTime * angularSpeed);
        }
        else
        {
            Fly(-contPitch * speed);
        }

        float contWalk = Input.GetAxis("Walk Axis");
        float contStrafe = Input.GetAxis("Strafe Axis");
        if (Mathf.Abs(contWalk) > 0.1f)
        {
            Walk(-contWalk * speed);
        }
        if (Mathf.Abs(contStrafe) > 0.1f)
        {
            Strafe(contStrafe * speed);
        }      
    }
}
