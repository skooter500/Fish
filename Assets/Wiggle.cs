using UnityEngine;
using System.Collections;
using BGE;

public class Wiggle : MonoBehaviour {

    public float amplitude = 5.0f;
    public float frequency = 5.0f;
    public float angle = 20.0f;

    float theta;
    Vector3 startPos;

    BGE.Boid boid;

    [Range(0, 180)]
    public float bodyField = 20.0f;

    [Range(0, 200)]
    public float bodyWiggleHeight;
    float oldBodyRot = 0;
    private float lastBodyWiggle = 0;

    public Wiggle()
    {
        amplitude = 20;
        frequency = 0.5f;
        theta = 0.0f;
    }
    // Use this for initialization
    void Start()
    {
        theta = 0;
        startPos = transform.position;
        boid = GetComponent<Boid>(); 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 test = transform.localPosition;
        theta += Time.deltaTime * frequency; // * (boid.velocity.magnitude / boid.maxSpeed);
        if (theta > (2.0f * Mathf.PI))
        {
            //theta = 0.0f;
        }

        // Wiggle the body
        //float wiggleThetaOffset = 0.0f; // Mathf.PI / 6.0f;
        //float bodyWiggle = Mathf.Sin(theta + wiggleThetaOffset) * amplitude;
        //Vector3 bodyPos = transform.position;
        //bodyPos -= (bodyWiggle - lastBodyWiggle) * transform.parent.up;
        //transform.position = bodyPos;
        //lastBodyWiggle = bodyWiggle;

        float bodyThetaOffset = Mathf.PI / 2.0f;
        float bodyRot = Mathf.Sin(theta + bodyThetaOffset) * bodyField;
        Vector3 rot = new Vector3(bodyRot - oldBodyRot, 0, 0);
        transform.Rotate(rot);
        //transform.Rotate(transform.right, bodyRot - oldBodyRot);
        oldBodyRot = bodyRot;

        //test.y += amp;
        
    }
}
