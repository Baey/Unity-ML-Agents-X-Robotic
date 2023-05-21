using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInfo : MonoBehaviour
{
    public Transform TracedAgent;
    private CrawlRobot tracedCrawlRobot;
    private CrawlRobotSensors tracedCrawlRobotSensors;

    // Start is called before the first frame update
    void Start()
    {
        tracedCrawlRobot = TracedAgent.GetComponent<CrawlRobot>();
        tracedCrawlRobotSensors = TracedAgent.GetComponent<CrawlRobotSensors>();
    }

    // Update is called once per frame
    void Update()
    {
        int l1 = tracedCrawlRobotSensors.GroundContact[0] ? 1 : 0, l2 = tracedCrawlRobotSensors.GroundContact[1] ? 1 : 0;
        int l3 = tracedCrawlRobotSensors.GroundContact[2] ? 1 : 0, l4 = tracedCrawlRobotSensors.GroundContact[3] ? 1 : 0;
        transform.GetComponent<Text>().text =
            "Worst ray: " +
            (tracedCrawlRobot.WorstGroundRay).ToString("0.00") +
            ", error: " + Mathf.Abs(tracedCrawlRobot.WorstGroundRay - tracedCrawlRobot.targetBodyHeight).ToString("0.000") + 
            "\nReward: " + tracedCrawlRobot.currentReward.ToString("0.000") +
            "\nContact array: " + l1*1 + ", " + l2*1 + ", " + l3*1 + ", " + l4*1;
    }
}