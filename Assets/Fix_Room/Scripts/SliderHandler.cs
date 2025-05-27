using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text valueText;
    [SerializeField] int sliderId = 0;

    [Header("Events")]
    [SerializeField] ValueChangeEventSO valueChangeEvent;

    float currentValue;
    void Start()
    {
        slider.value = slider.minValue;
        slider.onValueChanged.AddListener(UpdateHandleValue);
        UpdateHandleValue(slider.value);
    }

    void UpdateHandleValue(float value)
    {
        currentValue = value;
        valueText.text = value.ToString("F1");

        valueChangeEvent.Raise(sliderId, currentValue); // Notify listeners of the value change
    }

    public void OnContinue()
    {
        // ???

        // reset values
        slider.value = slider.minValue;
    }
}
