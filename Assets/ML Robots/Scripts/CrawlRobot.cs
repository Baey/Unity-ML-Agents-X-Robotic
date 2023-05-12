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

    [HideInInspector] public Dictionary<Transform, bool> legsContactDictionary = new Dictionary<Transform, bool>();
    [HideInInspector] public float bodyToGroundDistance;

    private JointsController m_JointsController;

    private const float m_MaxBodyToGroundDistance = 1.5f;
    // private float current


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var rayStartPosition = body.position - new Vector3(0, .5f, 0);

        RaycastHit hit;
        if (Physics.Raycast(rayStartPosition, Vector3.down, out hit, m_MaxBodyToGroundDistance))
        {
            Debug.DrawRay(rayStartPosition, body.TransformDirection(Vector3.down) * hit.distance, Color.red);
            bodyToGroundDistance = hit.distance;
        }
        else
        {
            Debug.DrawRay(rayStartPosition, body.TransformDirection(Vector3.down) * m_MaxBodyToGroundDistance,
                Color.green);
            bodyToGroundDistance = 1.45f;
        }

        var rewardValue = CalculateReward();
        AddReward(rewardValue * Time.deltaTime);


        if (bodyToGroundDistance / targetBodyHeight < .2f)
        {
            AddReward(-10f);
        }
    }

    public override void Initialize()
    {
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

        legsContactDictionary.Add(legFrontRight2, false);
        legsContactDictionary.Add(legFrontLeft2, false);
        legsContactDictionary.Add(legBackRight2, false);
        legsContactDictionary.Add(legBackLeft2, false);
    }

    public override void OnEpisodeBegin()
    {
        m_JointsController.resetElements();
        targetBodyHeight = Random.Range(1.1f, 1.35f);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Body info -- 10
        sensor.AddObservation(bodyToGroundDistance / targetBodyHeight);
        sensor.AddObservation(body.GetComponent<ArticulationBody>().velocity);
        sensor.AddObservation(body.GetComponent<ArticulationBody>().angularVelocity);
        sensor.AddObservation(Mathf.InverseLerp(-180f,180f,body.rotation.x));
        sensor.AddObservation(Mathf.InverseLerp(-180f,180f,body.rotation.y));
        sensor.AddObservation(Mathf.InverseLerp(-180f,180f,body.rotation.z));
        
        // Remaining elements rotations info -- 12
        sensor.AddObservation(observeLeg(legFrontRight));
        sensor.AddObservation(observeLeg(legFrontLeft));
        sensor.AddObservation(observeLeg(legBackRight));
        sensor.AddObservation(observeLeg(legBackLeft));
        
        sensor.AddObservation(observeLeg2(legFrontRight2));
        sensor.AddObservation(observeLeg2(legFrontLeft2));
        sensor.AddObservation(observeLeg2(legBackRight2));
        sensor.AddObservation(observeLeg2(legBackLeft2));
        

        // sensor.AddObservation(legFrontRight.GetComponent<ArticulationBody>().jointPosition[0]);
        // sensor.AddObservation(legFrontRight.GetComponent<ArticulationBody>().jointPosition[1]);
        // sensor.AddObservation(legFrontRight2.GetComponent<ArticulationBody>().jointPosition[0]);
        // sensor.AddObservation(legFrontLeft.GetComponent<ArticulationBody>().jointPosition[0]);
        // sensor.AddObservation(legFrontLeft.GetComponent<ArticulationBody>().jointPosition[1]);
        // sensor.AddObservation(legFrontLeft2.GetComponent<ArticulationBody>().jointPosition[0]);
        // sensor.AddObservation(legBackRight.GetComponent<ArticulationBody>().jointPosition[0]);
        // sensor.AddObservation(legBackRight.GetComponent<ArticulationBody>().jointPosition[1]);
        // sensor.AddObservation(legBackRight2.GetComponent<ArticulationBody>().jointPosition[0]);
        // sensor.AddObservation(legBackLeft.GetComponent<ArticulationBody>().jointPosition[0]);
        // sensor.AddObservation(legBackLeft.GetComponent<ArticulationBody>().jointPosition[1]);
        // sensor.AddObservation(legBackLeft2.GetComponent<ArticulationBody>().jointPosition[0]);
        
        // Leg contact with ground info -- 4
        foreach (var ele in legsContactDictionary)
        {
            sensor.AddObservation(ele.Value);
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
        // Gauss
        var bodyToGroundPenalty = 1f - Mathf.Exp(-.5f * Mathf.Pow((bodyToGroundDistance - targetBodyHeight) / .15f, 2));

        // Sigmoid
        var orientationPenalty = 1f / (1 + Mathf.Exp(-(.3f * (Mathf.Abs(body.rotation.x) + Mathf.Abs(body.rotation.z)) - 3.5f)));
        var speedPenalty = 1f / (1 + Mathf.Exp(-(10 * body.GetComponent<ArticulationBody>().velocity.magnitude - 5f)));
        return 1f - bodyToGroundPenalty * .7f - orientationPenalty * .27f - speedPenalty * .03f;
    }

    private Vector2 observeLeg(Transform leg)
    {
        var joint = leg.GetComponent<ArticulationBody>();
        
        // Y axis
        float yLowerLimit = joint.yDrive.upperLimit, yUpperLimit = joint.yDrive.lowerLimit;
        float yActual = joint.jointPosition[0] * Mathf.Rad2Deg;
        float yRelative = Mathf.InverseLerp(yLowerLimit, yUpperLimit, yActual);
        
        // Z axis
        float zLowerLimit = joint.zDrive.upperLimit, zUpperLimit = joint.zDrive.lowerLimit;
        float zActual = joint.jointPosition[1] * Mathf.Rad2Deg;
        float zRelative = Mathf.InverseLerp(zLowerLimit, zUpperLimit, zActual);

        return new Vector2(yRelative, zRelative);
    }
    
    private float observeLeg2(Transform leg)
    {
        var joint = leg.GetComponent<ArticulationBody>();
        
        // X axis
        float xLowerLimit = joint.xDrive.upperLimit, xUpperLimit = joint.xDrive.lowerLimit;
        float xActual = joint.jointPosition[0] * Mathf.Rad2Deg;
        float xRelative = Mathf.InverseLerp(xLowerLimit, xUpperLimit, xActual);

        return xRelative;
    }
}