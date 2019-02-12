using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour {

	Unit selectedUnit;
    GameObject startingHex = null;
    GameObject lastHex = null;

	// Use this for initialization
	void Start () {
        GameObject lastHex = startingHex;
    }
	
	// Update is called once per frame
	void Update () {

		// Is the mouse over a Unity UI Element?
		if(EventSystem.current.IsPointerOverGameObject()) {


			return;
		}


		Ray ray = Camera.main.ScreenPointToRay( Input.mousePosition );

		RaycastHit hitInfo;

		if( Physics.Raycast(ray, out hitInfo) ) {
			GameObject ourHitObject = hitInfo.collider.transform.parent.gameObject;

			//Debug.Log("Clicked On: " + ourHitObject.name);

			// So...what kind of object are we over?
			if(ourHitObject.GetComponent<Hex>() != null) {
				// Ah! We are over a hex!
				MouseOver_Hex(ourHitObject);
			}
			else if (ourHitObject.GetComponent<Unit>() != null) {
				// We are over a unit!
				MouseOver_Unit(ourHitObject);

			}


		}



	}

    void MouseOver_Hex(GameObject ourHitObject)
    {
        Debug.Log("Raycast hit: " + ourHitObject.name);
        MeshRenderer mr = ourHitObject.GetComponentInChildren<MeshRenderer>();
        
        //tool tip

        if (Input.GetMouseButtonDown(0))
        { 
            
            if (mr.material.color == Color.white)
            {
                mr.material.color = Color.green;
                //create road
            }
            else if (mr.material.color == Color.green)
            {
                mr.material.color = Color.blue;
                //create high road 
            }
            else
            {
                mr.material.color = Color.white;
                //delete road
            }

            // If we have a unit selected, let's move it to this tile!

            if (selectedUnit != null)
            {
                selectedUnit.destination = ourHitObject.transform.position;
            }


        }

        if (Input.GetMouseButtonDown(1))
        {
            mr.material.color = Color.white;
        }
    }

	void MouseOver_Unit(GameObject ourHitObject) {
		Debug.Log("Raycast hit: " + ourHitObject.name );

		if(Input.GetMouseButtonDown(0)) {
			// We have click on the unit
			selectedUnit = ourHitObject.GetComponent<Unit>();
		}

	}
}
