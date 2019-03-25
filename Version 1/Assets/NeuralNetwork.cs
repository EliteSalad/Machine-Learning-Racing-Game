using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour {

    int[] layer;
    Layer[] layers;

    public NeuralNetwork(int[] layer)
    {
        this.layer = new int[layer.Length];
        for (int i = 0; i < layer.Length; i++)
            this.layer[i] = layer[i];
        //layers = new Layer[Layer.]
    }
public class Layer
    {

    }

}
