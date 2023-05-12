using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BallBounceAgent : Agent
{
    // [SerializeField] private Transform targetBall;
    [SerializeField] private Rigidbody targetBall;
    [SerializeField] private Transform ground;
    Rigidbody m_Rigidbody;

    public override void Initialize()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        transform.position = ground.position + new Vector3(0, 0.8f, 0);
        transform.rotation = new Quaternion(0, 0, 0, 1);

        targetBall.position = ground.position + new Vector3(0, Random.Range(3f, 6f), 0);
        targetBall.velocity = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(transform.position - targetBall.position);
        sensor.AddObservation(targetBall.velocity);

        float dist = Vector3.Distance(ground.position, transform.position);
        sensor.AddObservation(dist);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        float rotateX = actions.ContinuousActions[2];
        float rotateY = actions.ContinuousActions[3];
        float rotateZ = actions.ContinuousActions[4];

        // int jump = actions.DiscreteActions[0];

        float moveSpeed = 2f, rotateSpeed = 1f;
        m_Rigidbody.velocity = new Vector3(moveX, 0, moveZ) * moveSpeed;

        m_Rigidbody.angularVelocity = new Vector3(rotateX, rotateY, rotateZ) * rotateSpeed;
        Vector3 ballLocalPosition = targetBall.position - ground.position;
        if (ballLocalPosition.y < 1.0f || Mathf.Abs(ballLocalPosition.x) > 6.0f ||
            Mathf.Abs(ballLocalPosition.y) > 6.0f)
        {
            AddReward(-5f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "target")
        {
            float dist = Vector3.Distance(ground.position, transform.position);

            AddReward(1.5f * (Mathf.Pow(6.3f - dist, 2) / Mathf.Pow(6.3f, 2)));
        }

        if (collision.gameObject.tag == "wall")
        {
            AddReward(-5f);
            EndEpisode();
        }
    }
}