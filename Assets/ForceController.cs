using UnityEngine;
using System.Collections;

public class ForceController : MonoBehaviour {
    Vector3 force;
    float mass = 1.0f;
    Vector3 velocity;
	Camera ovrCamera;
    float speed = 500.0f;
    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if (GameObject.FindGameObjectWithTag("ovrplayer") != null)
        {
            ovrCamera = GameObject.FindGameObjectWithTag("ovrplayer").GetComponentInChildren<Camera>();
        }
    }

    void Yaw(float angle)
    {
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
            force += ovrCamera.transform.forward * units;
        }
        else
        {
            force += transform.forward * units;
        }
    }

    void Fly(float units)
    {
        force += Vector3.up * units;
    }

    void Strafe(float units)
    {
        if (ovrCamera != null)
        {
            force += ovrCamera.transform.right * units;
        }
        else
        {
            force += transform.right * units;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX, mouseY;
        float speed = this.speed;

        float runAxis = Input.GetAxis("Run Axis");

        if (Input.GetKey(KeyCode.LeftShift) || runAxis != 0)
        {
            speed *= 10.0f;
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
            Roll(-Time.deltaTime * speed);
        }
        if (Input.GetKey(KeyCode.E))
        {
            Roll(Time.deltaTime * speed);
        }
        //BoidManager.PrintVector("OVR Forward: ", ovrCamera.transform.forward);

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");


        Yaw(mouseX);
        float contYaw = Input.GetAxis("Yaw Axis");
        float contPitch = Input.GetAxis("Pitch Axis");
        Yaw(contYaw);

        // If in Rift mode, dont pitch
        if (ovrCamera == null)
        {
            Pitch(-mouseY);
            Pitch(contPitch);
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

        Vector3 accel = force / mass;
        velocity += accel * Time.deltaTime;
        Vector3 pos = transform.position;
        pos += velocity * Time.deltaTime;
        force = Vector3.zero;
        transform.position = pos;

        velocity *= 0.92f;
    }
}
