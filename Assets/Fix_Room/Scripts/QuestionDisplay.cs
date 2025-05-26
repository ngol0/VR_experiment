using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionDisplay : MonoBehaviour
{
    int numberOfQuestion = 3;
    Dictionary<int, float> sliderDict;

    void OnEnable()
    {
        sliderDict = new Dictionary<int, float>();
        for (int i = 0; i < numberOfQuestion; i++)
        {
            sliderDict.Add(i, 1.0f); // Initialize sliders to default value
        }
    }

    
}
