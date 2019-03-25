using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gates : MonoBehaviour {
    public bool isLastGate = false;
    public bool isMiddleGate = false;
    public bool isActive = true;
    public Gates nextGate, previouseGate;
    public UIManager uiManager;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    public void OnTriggerEnter()
    {
        //make this one not active
        //make next one active
        if (isMiddleGate == true && previouseGate.isActive == true && nextGate.isActive == false)
        { previouseGate.isActive = false;
            //minus 1 from the lap counter
        }

        nextGate.isActive = true;
        if (isLastGate == true && isActive == true)
        { Debug.Log("Do the thing");
            isActive = false;
            uiManager.newLap = true;
        }
        else
            isActive = false;
    }
}
