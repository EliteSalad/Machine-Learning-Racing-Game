using UnityEngine;

public class SensoryData : MonoBehaviour {

    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public float[] inp;
    Collider col;
    public bool paused = false;
    public GameObject colliders;
    private int lap = 0;

    // Use this for initialization
    void Start ()
    {
        colliders = gameObject.transform.GetChild(0).gameObject;
        col = GetComponent<Collider>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Feelers();
	}
    
    void Feelers()
    {
        RaycastHit hit;

        Vector3[] feeler = new Vector3[]
        {
            transform.TransformDirection(Vector3.left),
            transform.TransformDirection(Vector3.left+Vector3.forward),
            transform.TransformDirection(Vector3.forward),
            transform.TransformDirection(Vector3.right + Vector3.forward),
            transform.TransformDirection(Vector3.right),
            transform.TransformDirection(Vector3.left+Vector3.back),
            transform.TransformDirection(Vector3.back),
            transform.TransformDirection(Vector3.right + Vector3.back),

        };

        inp = new float[feeler.Length];

        for (int i = 0; i < feeler.Length; i++)
        {
            if (Physics.Raycast(transform.position, feeler[i], out hit))
            {
                if (hit.collider != null && hit.collider != col)
                    inp[i] = hit.distance;
            }
            // Draw the feelers in the Scene mode
            Debug.DrawRay(transform.position, feeler[i] * 10, Color.red);
        }
    }
  

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "FinishLine")
        {
            lap++;
                //increase lap
            //
        }
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Wall")
            Pause();

    }

    void Pause()
    {
        paused = true;
        //setcollider to false
        colliders.SetActive(false);
        //Stop movement
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        //stop nn

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
