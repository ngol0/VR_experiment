using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionManager : MonoBehaviour
{
    [SerializeField] FlowManager flowManager;
    [SerializeField] List<SliderHandler> sliders;

    const int NUM_OF_QUESTION = 3;
    Dictionary<int, float> sliderDict;
    public Action OnQuestionDone;

    void OnEnable()
    {
        sliderDict = new Dictionary<int, float>();
        for (int i = 0; i < NUM_OF_QUESTION; i++)
        {
            sliderDict.Add(i, 0.0f); // Initialize sliders to default value
            if (sliders.Count > i)
            {
                sliders[i].sliderId = i; // Assign slider ID
            }
            else
            {
                Debug.LogWarning($"SliderHandler for question {i} not found in the list.");
            }
        }
    }

    public void OnSliderValueChanged(int sliderId, float value)
    {
        //Debug.Log("Slider ID: " + sliderId + ", Value: " + value);
        if (sliderDict.ContainsKey(sliderId))
        {
            sliderDict[sliderId] = value; // Update the value for the slider
        }
        else
        {
            Debug.LogWarning($"Slider ID {sliderId} not found in dictionary.");
        }

        // Check the continue button
        for (int i = 0; i < NUM_OF_QUESTION; i++)
        {
            if (sliderDict[i] < 1.0f)
            {
                return; // Not all sliders are filled, do not allow continue
            }
        }
        OnQuestionDone?.Invoke(); // activate continue button
    }

    public void OnReset()
    {
        // Reset all sliders to default values
        for (int i = 0; i < NUM_OF_QUESTION; i++)
        {
            sliderDict[i] = 0.0f;
            if (sliders.Count > i)
            {
                sliders[i].OnReset(); // Reset the slider UI
            }
            else
            {
                Debug.LogWarning($"SliderHandler for question {i} not found in the list.");
            }
        }
    }

    public void OnSaveData()
    {
        // todo: make this dynamic and also add safety here
        flowManager.SaveData(sliderDict[0], sliderDict[1], sliderDict[2]);
    }
}
