using UnityEngine;
using UnityEngine.Events;

public class ValueChangeListener : MonoBehaviour
{
    public ValueChangeEventSO Event;
    public ValueChangeEvent UnitEventResponse;

    public void OnEventRaised(int id, float value)
    {
        UnitEventResponse?.Invoke(id, value);
    }

    private void OnEnable()
    { Event.RegisterListener(this); }

    private void OnDisable()
    { Event.UnregisterListener(this); }
}

[System.Serializable]
public class ValueChangeEvent : UnityEvent<int, float>
{
}
