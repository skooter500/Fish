
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using BGE;

public class DolphinFactory : MonoBehaviour
{

    public float radius;
    public int numDolphinsX;
    public int numDolphinsZ;
    public int boidCount;
    public float minGap;
    public float maxGap;

    public GameObject boidPrefab;

    [Range(0, 1)]
    public float density;

    [Header("Debug")]
    public bool drawGizmos;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    DolphinFactory()
    {
        radius = 100;
    }


    void Start()
    {
        GameObject leader = GameObject.Instantiate<GameObject>(boidPrefab);

        leader.GetComponent<Boid>().randomWalkEnabled = true;
        leader.GetComponent<Boid>().randomWalkRadius = 1000;

        for (int z = 0; z < numDolphinsZ; z++)
        {
            for (int x = 0; x < numDolphinsX; x++)
            {
                GameObject newLeader = GameObject.Instantiate<GameObject>(boidPrefab);

            }
        }
    }
}
