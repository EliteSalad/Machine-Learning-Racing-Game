using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

	public GameObject hexPrefab;

    //Size of hex hex map (not world space)

	public int width = 64;
	public int height = 40;

	float xOffset = 0.882f;
	float zOffset = 0.764f;

    public bool create = false;
    public bool delete = false;



	// Use this for initialization
	void Start () {

        CreateMap();

	}
	
	// Update is called once per frame
	void Update ()
    {
	if (create)
        {
            DeleteMap();
            CreateMap();
            create = false;
        }
    if (delete)
        {
            DeleteMap();
            delete = false;
        }
	}

    void CreateMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                float xPos = x * xOffset;

                // Are we on an odd row?
                if (y % 2 == 1)
                {
                    xPos += xOffset / 2f;
                }

                GameObject hex_go = (GameObject)Instantiate(hexPrefab, new Vector3(xPos, 0, y * zOffset), Quaternion.identity);

                // Name the gameobject something sensible.
                hex_go.name = "Hex_" + x + "_" + y;

                // Make sure the hex is aware of its place on the map
                hex_go.GetComponent<Hex>().x = x;
                hex_go.GetComponent<Hex>().y = y;
                

                // For a cleaner hierachy, parent this hex to the map
                hex_go.transform.SetParent(this.transform);

                //optimisation
                hex_go.isStatic = true;

            }
        }
    }

    void DeleteMap()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        
        //delete all children of Map
    }
}
