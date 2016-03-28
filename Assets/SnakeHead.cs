using UnityEngine;
using System.Collections.Generic;

public class SnakeHead : MonoBehaviour {    
    public List<GameObject> followers = new List<GameObject>();

    List<float> bondDistances = new List<float>();

    [Range(0, 1000)]
    public float bondDamping = 30.0f;

    [Range(0, 50)]
    public float angularBondDamping = 7.0f;

    void Start()
    {
        Transform prevFollower;
        for (int i = 0; i < followers.Count; i++)
        {
            if (i == 0)
            {
                prevFollower = this.transform;
            }
            else
            {
                prevFollower = followers[i - 1].transform;
            }

            Transform follower = followers[i].transform;
            bondDistances.Add(Vector3.Distance(prevFollower.position, follower.position));
        }
    }
    
	void Update ()
    {
        Transform prevFollower;

        for (int i = 0 ; i < followers.Count; i++)
        {
            if (i == 0)
            {
                prevFollower = this.transform;
            }
            else
            {
                prevFollower = followers[i - 1].transform;
            }

            Transform follower = followers[i].transform;

            DelayedMovement(prevFollower, follower, bondDistances[i]);            
        }
    }

    void DelayedMovement(Transform prevFollower, Transform follower, float bondDistance)
    {
        SnakePart snakePart = follower.gameObject.GetComponent<SnakePart>();
        float bondDamping = this.bondDamping;
        float angularBondDamping = this.angularBondDamping;

        if (snakePart != null)
        {
            bondDamping = snakePart.bondDamping;
            angularBondDamping = snakePart.angularBondDamping;
        }

        Vector3 wantedPosition = prevFollower.TransformPoint(0, 0, -bondDistance);
        follower.transform.position = Vector3.Lerp(follower.transform.position, wantedPosition, Time.deltaTime * bondDamping);

        Quaternion wantedRotation = Quaternion.LookRotation(prevFollower.position - follower.transform.position, prevFollower.up);
        follower.transform.rotation = Quaternion.Slerp(follower.transform.rotation, wantedRotation, Time.deltaTime * angularBondDamping);
    }
}
