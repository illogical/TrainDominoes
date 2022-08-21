using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/General Event", fileName = "NewGeneralEvent")]
public class GeneralEvent : ScriptableObject
{
    public UnityEvent OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}

