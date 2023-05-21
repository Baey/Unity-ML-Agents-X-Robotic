using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CrawlRobot : Agent
{
    public float maxTargetVelocity;
    public float targetBodyHeight;

    [Header("Body Parts")] [Space(10)] public Transform body;
    public Transform legFrontRight;
    public Transform legFrontRight2;
    public Transform legFrontLeft;
    public Transform legFrontLeft2;
    public Transform legBackRight;
    public Transform legBackRight2;
    public Transform legBackLeft;
    public Transform legBackLeft2;

    [HideInInspector] public float WorstGroundRay;
    [HideInInspector] public float currentReward;

    private JointsController m_JointsController;
    private CrawlRobotSensors m_CrawlRobotSensors;

    private const float m_MaxBodyToGroundDistance = 1.5f;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var maxError = 0f;
        foreach (var dist in m_CrawlRobotSensors.DistanceArray)
        {
            if(Mathf.Abs(dist - targetBodyHeight) > maxError)
            {
                maxError = Mathf.Abs(dist - targetBodyHeight);
                WorstGroundRay = dist;
            }
        }
        currentReward = CalculateReward();
        AddReward(currentReward * Time.deltaTime);


        // if (maxError > .5f)
        // {
        //     AddReward(-maxError * Time.deltaTime);
        // }
    }

    public override void Initialize()
    {
        m_CrawlRobotSensors = GetComponent<CrawlRobotSensors>();
        m_JointsController = GetComponent<JointsController>();
        m_JointsController.configureElement(body);
        m_JointsController.configureElement(legFrontRight);
        m_JointsController.configureElement(legFrontRight2);
        m_JointsController.configureElement(legFrontLeft);
        m_JointsController.configureElement(legFrontLeft2);
        m_JointsController.configureElement(legBackRight);
        m_JointsController.configureElement(legBackRight2);
        m_JointsController.configureElement(legBackLeft);
        m_JointsController.configureElement(legBackLeft2);
    }

    public override void OnEpisodeBegin()
    {
        m_JointsController.resetElements();
        targetBodyHeight = Random.Range(1.1f, 1.35f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Body info -- 11
        foreach (var dist in m_CrawlRobotSensors.DistanceArray)
        {
            sensor.AddObservation((dist - targetBodyHeight) / 2.5f);
        }
        sensor.AddObservation(body.GetComponent<ArticulationBody>().velocity / 5);
        sensor.AddObservation(body.GetComponent<ArticulationBody>().angularVelocity);
        sensor.AddObservation(body.rotation.eulerAngles / 360.0f);
        
        // Remaining elements rotations info -- 12
        sensor.AddObservation(m_CrawlRobotSensors.observeLeg(legFrontRight));
        sensor.AddObservation(m_CrawlRobotSensors.observeLeg(legFrontLeft));
        sensor.AddObservation(m_CrawlRobotSensors.observeLeg(legBackRight));
        sensor.AddObservation(m_CrawlRobotSensors.observeLeg(legBackLeft));
        
        sensor.AddObservation(m_CrawlRobotSensors.observeLeg2(legFrontRight2));
        sensor.AddObservation(m_CrawlRobotSensors.observeLeg2(legFrontLeft2));
        sensor.AddObservation(m_CrawlRobotSensors.observeLeg2(legBackRight2));
        sensor.AddObservation(m_CrawlRobotSensors.observeLeg2(legBackLeft2));
        
        // Leg contact with ground info -- 4
        m_CrawlRobotSensors.checkGroundContact();
        foreach (var contact in m_CrawlRobotSensors.GroundContact)
        {
            sensor.AddObservation(contact);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        // continuousAction[0] = Input.GetAxisRaw("Horizontal");
        // continuousAction[1] = Input.GetAxisRaw("Vertical");
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var actions = actionBuffers.ContinuousActions;
        var i = -1;

        m_JointsController.driveJoint(legFrontRight, 0, actions[++i], actions[++i], actions[++i]);
        m_JointsController.driveJoint(legFrontRight2, actions[++i], 0, 0, actions[++i]);
        m_JointsController.driveJoint(legFrontLeft, 0, actions[++i], actions[++i], actions[++i]);
        m_JointsController.driveJoint(legFrontLeft2, actions[++i], 0, 0, actions[++i]);
        m_JointsController.driveJoint(legBackRight, 0, actions[++i], actions[++i], actions[++i]);
        m_JointsController.driveJoint(legBackRight2, actions[++i], 0, 0, actions[++i]);
        m_JointsController.driveJoint(legBackLeft, 0, actions[++i], actions[++i], actions[++i]);
        m_JointsController.driveJoint(legBackLeft2, actions[++i], 0, 0, actions[++i]);
    }

    private float CalculateReward()
    {
        var bodyToGroundPenalty = Mathf.Abs(WorstGroundRay - targetBodyHeight) / .5f;
        var orientationPenalty = Mathf.Abs(body.rotation.x) + Mathf.Abs(body.rotation.z) / 30f;
        var speedPenalty = body.GetComponent<ArticulationBody>().velocity.magnitude / 4f;
        return .5f - orientationPenalty * .45f - bodyToGroundPenalty * .45f - speedPenalty * .1f;
    }
}