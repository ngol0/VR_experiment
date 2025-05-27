using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    int numberOfQuestion = 3;
    Dictionary<int, float> sliderDict;

    public Action OnContinueAfterQuestion;

    void OnEnable()
    {
        sliderDict = new Dictionary<int, float>();
        for (int i = 0; i < numberOfQuestion; i++)
        {
            sliderDict.Add(i, 0.0f); // Initialize sliders to default value
        }
    }

    public void OnSliderValueChanged(int sliderId, float value)
    {
        Debug.Log("Slider ID: " + sliderId + ", Value: " + value);
        if (sliderDict.ContainsKey(sliderId))
        {
            sliderDict[sliderId] = value; // Update the value for the slider
        }
        else
        {
            Debug.LogWarning($"Slider ID {sliderId} not found in dictionary.");
        }

        // Check the continue button
        for (int i = 0; i < numberOfQuestion; i++)
        {
            if (sliderDict[i] == 1.0f)
            {
                //Debug.Log("Not all sliders are filled.");
                return; // Not all sliders are filled, do not allow continue
            }
            OnContinueAfterQuestion?.Invoke(); // All sliders are filled, allow continue
        }
    }
}
