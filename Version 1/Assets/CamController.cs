using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour {

    public enum CameraMode { POV, Follow, Top, Locked, Free, Track }
    public CameraMode camToggler ;
    public Transform targetCar = null;

    // Use this for initialization
    void Start () {
        camToggler = CameraMode.Track;
	}
	
	// Update is called once per frame
	void Update () {

        if (targetCar == null)
            targetCar = GameObject.Find("/Racers/Car").transform;
        //if we don't have a curent car to follow find one 
        //when we find car make it the target
        switch (camToggler)

        {
            case CameraMode.Follow:
                //Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, targetCar.transform.localRotation, Time.deltaTime * 10);   //camera "smoothly" turns matches rotation of car
                if (Vector3.Distance(Camera.main.transform.position, targetCar.position) > 15)
                {
                    Camera.main.transform.Translate(Vector3.forward * .15f);
                }

                Camera.main.transform.LookAt(targetCar.position + (Vector3.up * .5f));
                break;
            case CameraMode.Free:
                Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x + Input.GetAxis("Horizontal"), -100, 100), Mathf.Clamp(Camera.main.transform.position.y + (Input.GetAxis("Mouse ScrollWheel") * -10), 5, 100), Mathf.Clamp(Camera.main.transform.position.z + Input.GetAxis("Vertical"), -100, 100));
                break;
            case CameraMode.Locked:
                Camera.main.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
                Camera.main.transform.position = new Vector3(targetCar.transform.position.x, 50, targetCar.transform.position.z);
                break;
            case CameraMode.POV:
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, targetCar.transform.localRotation, Time.deltaTime * 10);   //camera "smoothly" turns matches rotation of car
                Camera.main.transform.position = targetCar.transform.position + new Vector3(0,2,0) ;    //car position + 2 vertically 
                break;
            case CameraMode.Top:
                Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Euler(new Vector3(90, targetCar.transform.eulerAngles.y, 0)), Time.deltaTime * 10);
                Camera.main.transform.position = new Vector3(targetCar.transform.position.x, 25, targetCar.transform.position.z);
                break;
            case CameraMode.Track:
                Camera.main.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Mouse ScrollWheel") * 10, Input.GetAxis("Vertical")));
                Camera.main.transform.position = new Vector3(Mathf.Clamp(Camera.main.transform.position.x, -100, 100), Mathf.Clamp(Camera.main.transform.position.y, 2, 100), Mathf.Clamp(Camera.main.transform.position.z, -100, 100));
                Camera.main.transform.LookAt(targetCar.transform);
                break;

        }
    }

    public void TogglerCamera()
    {


    }

}
