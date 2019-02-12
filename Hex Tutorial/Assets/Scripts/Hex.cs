using UnityEngine;
using System.Collections;

public class Hex : MonoBehaviour {

    // Our coordinates in the map array
    public bool odd = false;
    public int rowNumber;
	public int x;
	public int y;

    GameObject left, right, upperLeft, upperRight, lowerLeft, lowerRight;

    private void Start()
    {
        if (x % 2 == 1)     //x represents the row number in this situation
            odd = true;

        else
            odd = false;
        
    }

    public Hex[] GetNeighbours() {

        ///Can use neigbors to determin the order the track should be built in?
        ///if last hex is adjacent connect the hexes else current hex connects to other adjacent hex
		left = GameObject.Find("Hex_" + (x-1) + "_" + y);

		right = GameObject.Find("Hex_" + (x+1) + "_" + y);


        if (odd)
        {
            //odd = true;
            upperLeft = GameObject.Find("Hex_" + (x) + "_" + (y + 1));
            upperRight = GameObject.Find("Hex_" + (x + 1) + "_" + (y + 1));
            lowerLeft = GameObject.Find("Hex_" + (x) + "_" + (y - 1));
            lowerRight = GameObject.Find("Hex_" + (x + 1) + "_" + (y - 1));
        }
        else
        {
            //odd = false;
            upperLeft = GameObject.Find("Hex_" + (x - 1) + "_" + (y + 1));
            upperRight = GameObject.Find("Hex_" + (x) + "_" + (y + 1));
            lowerLeft = GameObject.Find("Hex_" + (x - 1) + "_" + (y - 1));
            lowerRight = GameObject.Find("Hex_" + (x) + "_" + (y - 1));
        }





        return null;
	}

}
