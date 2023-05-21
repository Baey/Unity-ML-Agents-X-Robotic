using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.VisualScripting;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    [Header("Traced Agent")] [SerializeField]
    private Agent agent;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("ground"))
        {
            // agent.AddReward(-10f);
            agent.EndEpisode();
        }
    }
}