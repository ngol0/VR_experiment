using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderHandler : MonoBehaviour
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text valueText;
    [SerializeField] int sliderId = 0;

    float currentValue;
    void Start()
    {
        slider.value = 1.0f;
        slider.onValueChanged.AddListener(UpdateHandleValue);
        UpdateHandleValue(slider.value);
    }

    void UpdateHandleValue(float value)
    {
        currentValue = value;
        valueText.text = value.ToString("F1");
    }

    public void OnContinue()
    {
        // save the value here:
        //manager.ContinueAfterQuestion(currentValue);

        // reset values
        slider.value = 1.0f;
    }
}
