using System.Collections;
using System.Collections.Generic;
using UnityEngine;      //using unity Engine random
using System;

public class DNA<T> : MonoBehaviour {




    public T[] Genes { get; private set; }
    public float Fitness { get; private set; }
    System.Random random;
    private Func<T> getRandomGene;
    private Func<float, int> fitnessFunction;
    public void Start()
    {
        
    }
    public DNA(int size, System.Random random, Func<T> getRandomGene, Func<float, int> fitnessFunction, bool initGenes = true)
    {
        Genes = new T[size];
        this.random = random;
        this.getRandomGene = getRandomGene;
        this.fitnessFunction = fitnessFunction;
        
        if (initGenes)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = getRandomGene();
            }
        }

    }

    public float CalculateFitness(int index)
    {
        Fitness = fitnessFunction(index);
        return Fitness;
    }

    public DNA<T> Crossover(DNA<T> otherParent)
    {
        DNA<T> child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, initGenes:false);
        
        //no random nimbers here creating them one after the other will make them the same too 
        for (int i = 0; i < Genes.Length; i++)
        {
            if (random.NextDouble() < 0.5)
            child.Genes[i] = Genes[i];
            else
            child.Genes[i] = otherParent.Genes[i];
        }
        return child;
    }

    public void Mutate(float mutationRate)
    {
      for (int i = 0; i < Genes.Length; i++)
        {
            if (random.NextDouble() < mutationRate)
                Genes[i] = getRandomGene();
        }
    }
}
