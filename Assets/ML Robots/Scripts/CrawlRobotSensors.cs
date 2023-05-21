using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawlRobotSensors : MonoBehaviour
{
    public float MaxRaycastDistance;
    public Transform Body;
    public Transform FrontRightLegEnd;
    public Transform FrontLeftLegEnd;
    public Transform BackRightLegEnd;
    public Transform BackLeftLegEnd;

    [HideInInspector] public float[] DistanceArray = new float[2];
    [HideInInspector] public bool[] GroundContact = new bool[] {false, false, false, false};
    // [HideInInspector] public float LongestGroundRay;
    // Start is called before the first frame update
    void Start()
    {
        // LongestGroundRay = MaxRaycastDistance;

        for (var i = 0; i < DistanceArray.Length; i++)
        {
            DistanceArray[i] = MaxRaycastDistance;
        }
    }

    // Update is called once per frame
    void Update()
    {
        groundRayCast(); 
    }

    public Vector2 observeLeg(Transform leg)
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
    
    public float observeLeg2(Transform leg)
    {
        var joint = leg.GetComponent<ArticulationBody>();
        
        // X axis
        float xLowerLimit = joint.xDrive.upperLimit, xUpperLimit = joint.xDrive.lowerLimit;
        float xActual = joint.jointPosition[0] * Mathf.Rad2Deg;
        float xRelative = Mathf.InverseLerp(xLowerLimit, xUpperLimit, xActual);

        return xRelative;
    }

    public bool[] checkGroundContact()
    {
        GroundContact[0] = FrontRightLegEnd.GetComponent<LegTouchdown>().hasContact;
        GroundContact[1] = FrontLeftLegEnd.GetComponent<LegTouchdown>().hasContact;
        GroundContact[2] = BackRightLegEnd.GetComponent<LegTouchdown>().hasContact;
        GroundContact[3] = BackLeftLegEnd.GetComponent<LegTouchdown>().hasContact;

        return GroundContact;
    }

    private void groundRayCast()
    {
        var pos1 = (Body.position - new Vector3(1, .5f, 0));
        var pos2 = Body.position - new Vector3(-1, .5f, 0);

        var dir = pos1 - Body.position; // get point direction relative to pivot
        dir = Quaternion.Euler(Body.rotation.eulerAngles) * dir; // rotate it
        pos1 = dir + Body.position; // calculate rotated point

        dir = pos2 - Body.position; // get point direction relative to pivot
        dir = Quaternion.Euler(Body.rotation.eulerAngles) * dir; // rotate it
        pos2 = dir + Body.position; // calculate rotated point

        RaycastHit hit1;
        if (Physics.Raycast(pos1, Vector3.down, out hit1, MaxRaycastDistance))
        {
            Debug.DrawRay(pos1, Body.TransformDirection(Vector3.down) * hit1.distance, Color.red);
            DistanceArray[0] = hit1.distance;
        }
        else
        {
            Debug.DrawRay(pos1, Body.TransformDirection(Vector3.down) * MaxRaycastDistance,Color.green);
            DistanceArray[0] = MaxRaycastDistance;
        }
        
        RaycastHit hit2;
        if (Physics.Raycast(pos2, Vector3.down, out hit2, MaxRaycastDistance))
        {
            Debug.DrawRay(pos2, Body.TransformDirection(Vector3.down) * hit2.distance, Color.red);
            DistanceArray[1] = hit2.distance;
        }
        else
        {
            Debug.DrawRay(pos2, Body.TransformDirection(Vector3.down) * MaxRaycastDistance,Color.green);
            DistanceArray[1] = MaxRaycastDistance;
        }
        // LongestGroundRay = distSum / i;
    }

}
