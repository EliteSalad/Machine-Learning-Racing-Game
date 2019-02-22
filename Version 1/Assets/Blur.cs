using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Blur : MonoBehaviour {

    public Blur blurEffect;

    void Update()
    {
        // Toggles blur effect
        if (Input.GetKeyDown(KeyCode.Space))
            blurEffect.enabled = !blurEffect.enabled;
    }
}
