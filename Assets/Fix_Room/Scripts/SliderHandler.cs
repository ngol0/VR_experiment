using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text valueText;
    public int sliderId = 0;

    [Header("Events")]
    [SerializeField] ValueChangeEventSO valueChangeEvent;

    float currentValue;
    void Start()
    {
        slider.value = slider.minValue;
        currentValue = slider.value;

        slider.onValueChanged.AddListener(UpdateHandleValue);
        valueText.text = "?";
    }

    void UpdateHandleValue(float value)
    {
        currentValue = value;
        valueText.text = currentValue.ToString("F1");

        valueChangeEvent.Raise(sliderId, currentValue); // Notify listeners of the value change
    }

    public void OnReset()
    {
        // Temporarily remove listener
        slider.onValueChanged.RemoveListener(UpdateHandleValue);

        // Reset value
        slider.value = slider.minValue;
        currentValue = slider.value;
        valueText.text = "?";

        // Reattach listener
        slider.onValueChanged.AddListener(UpdateHandleValue);
    }
}
