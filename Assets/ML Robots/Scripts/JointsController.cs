using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = System.Numerics.Vector3;


public struct ElementConfiguration
{
    public ArticulationReducedSpace initialJointPosition;
    public ArticulationReducedSpace initialJointAcceleration;
    public ArticulationReducedSpace initialJointForce;
    public ArticulationReducedSpace initialJointVelocity;
    public UnityEngine.Vector3 initialPosition;
}

public class JointsController : MonoBehaviour
{
    [HideInInspector] public Dictionary<Transform, ElementConfiguration> m_ElementsToControl = new();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void configureElement(Transform ele)
    {
        ElementConfiguration elementConfiguration;

        elementConfiguration.initialJointPosition = ele.GetComponent<ArticulationBody>().jointPosition;
        elementConfiguration.initialJointAcceleration = ele.GetComponent<ArticulationBody>().jointAcceleration;
        elementConfiguration.initialJointForce = ele.GetComponent<ArticulationBody>().jointForce;
        elementConfiguration.initialJointVelocity = ele.GetComponent<ArticulationBody>().jointVelocity;
        elementConfiguration.initialPosition = ele.position;

        m_ElementsToControl.Add(ele, elementConfiguration);
    }

    /// <summary>
    /// Reset all controlled elements to default position
    /// </summary>
    public void resetElements()
    {
        foreach (var ele in m_ElementsToControl)
        {
            var elementArticulationBody = ele.Key.GetComponent<ArticulationBody>();

            elementArticulationBody.jointPosition = ele.Value.initialJointPosition;
            elementArticulationBody.jointAcceleration = ele.Value.initialJointAcceleration;
            elementArticulationBody.jointForce = ele.Value.initialJointForce;
            elementArticulationBody.jointVelocity = ele.Value.initialJointVelocity;
            elementArticulationBody.velocity = new UnityEngine.Vector3(0, 0, 0);
            elementArticulationBody.angularVelocity = new UnityEngine.Vector3(0, 0, 0);

            if (elementArticulationBody.isRoot)
            {
                float x = Random.Range(-10f, 10f), y = Random.Range(-10f, 10f), z = Random.Range(-10f, 10f);
                elementArticulationBody.TeleportRoot(ele.Value.initialPosition, Quaternion.Euler(x, y, z));
            }
        }
    }


    public void driveJoint(Transform element, float x, float y, float z, float v)
    {
        // Convert standard <-1, 1> continuous action to <0, 1> range
        x = (x + 1f) * 0.5f;
        y = (y + 1f) * 0.5f;
        z = (z + 1f) * 0.5f;
        v = (v + 1f) * 0.5f;

        var articulationBody = element.GetComponent<ArticulationBody>();

        // Interpolate to target angle
        if (articulationBody.linearLockX != ArticulationDofLock.LockedMotion)
        {
            var xDrive = articulationBody.xDrive;
            xDrive.target = Mathf.Lerp(xDrive.lowerLimit, xDrive.upperLimit, x);
            xDrive.targetVelocity = Mathf.Lerp(0, transform.GetComponent<CrawlRobot>().maxTargetVelocity, v);
            articulationBody.xDrive = xDrive;
        }

        if (articulationBody.linearLockY != ArticulationDofLock.LockedMotion)
        {
            var yDrive = articulationBody.yDrive;
            yDrive.target = Mathf.Lerp(yDrive.lowerLimit, yDrive.upperLimit, y);
            yDrive.targetVelocity = Mathf.Lerp(0, transform.GetComponent<CrawlRobot>().maxTargetVelocity, v);
            articulationBody.yDrive = yDrive;
        }

        if (articulationBody.linearLockZ != ArticulationDofLock.LockedMotion)
        {
            var zDrive = articulationBody.zDrive;
            zDrive.target = Mathf.Lerp(zDrive.lowerLimit, zDrive.upperLimit, z);
            zDrive.targetVelocity = Mathf.Lerp(0, transform.GetComponent<CrawlRobot>().maxTargetVelocity, v);
            articulationBody.zDrive = zDrive;
        }
    }
}