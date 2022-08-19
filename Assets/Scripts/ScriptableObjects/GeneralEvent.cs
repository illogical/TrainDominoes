using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Selection Event", fileName = "New General Event")]
public class GeneralEvent : ScriptableObject
{
    public UnityEvent OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}

