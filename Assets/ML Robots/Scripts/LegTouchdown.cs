using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTouchdown : MonoBehaviour
{
    public Material ContactMaterial;
    public Material DefaultMaterial;

    [HideInInspector] public bool hasContact;
    // Start is called before the first frame update
    void Start()
    {
        hasContact = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("ground"))
        {
            transform.GetComponent<MeshRenderer>().material = ContactMaterial;
            hasContact = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.transform.CompareTag("ground"))
        {
            transform.GetComponent<MeshRenderer>().material = DefaultMaterial;
            hasContact = false;
        }
    }
}
