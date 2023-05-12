using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInfo : MonoBehaviour
{
    public Transform TracedAgent;
    private CrawlRobot tracedCrawlRobot;

    // Start is called before the first frame update
    void Start()
    {
        tracedCrawlRobot = TracedAgent.GetComponent<CrawlRobot>();
    }

    // Update is called once per frame
    void Update()
    {
        var joint = tracedCrawlRobot.legFrontLeft.GetComponent<ArticulationBody>();
        transform.GetComponent<Text>().text =
            "Current height ratio: " +
            (tracedCrawlRobot.bodyToGroundDistance / tracedCrawlRobot.targetBodyHeight).ToString("0.00") +
            "\nSample joint: " + 
            Mathf.InverseLerp(joint.zDrive.lowerLimit, joint.zDrive.upperLimit,joint.jointPosition[1] * Mathf.Rad2Deg).ToString("0.00");

    }
}