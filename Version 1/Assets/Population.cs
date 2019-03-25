using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour {

    Transform startLocation;
    GameObject car;

    float timeAlive = 0;
    float mutationRate;
    public int popMax = 5 , pop = 0;
    public GameObject[] population;
    public List<int> matingPool;
    int generations;
    bool finished = false;
    
    
	// Use this for initialization
	void Start ()
    {
        do
        {
            Instantiate(car, startLocation);
        } while (pop < popMax);
	//instationte all drivers needed	
	}
	
	// Update is called once per frame
	void Update () {
        
	}
    private void FixedUpdate()
    {
        timeAlive += Time.deltaTime;
    }
}
