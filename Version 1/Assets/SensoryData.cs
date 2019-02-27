using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensoryData : MonoBehaviour {

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    void Feelers()
    {
        /// array of inputs to map the up coming roads 
        Vector3[] feeler = new Vector3[] 
        {

        };
    }
    void CurrentState()
    {
        //Speed
        
        //Downforce
        
        //Slip Limit
        

    }
    void FixedAttributes()
    {
        //Torque
        //Steering angle 
        //Brake Torque

    }
}
