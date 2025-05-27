using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CustomizedEvents/ValueChangeEventSO")]
public class ValueChangeEventSO : ScriptableObject
{
    private List<ValueChangeListener> listeners =
        new List<ValueChangeListener>();

    public void Raise(int id, float value)
    {
        //Debug.Log(":::Raise?");
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventRaised(id, value);
    }

    public void RegisterListener(ValueChangeListener listener)
    { listeners.Add(listener); }

    public void UnregisterListener(ValueChangeListener listener)
    { listeners.Remove(listener); }
}
