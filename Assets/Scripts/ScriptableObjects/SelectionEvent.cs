using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Selection Event", fileName = "NewSelectionEvent")]
public class SelectionEvent : ScriptableObject
{
    public UnityEvent<int> OnEventRaised;

    public void RaiseEvent(int id)
    {
        OnEventRaised?.Invoke(id);
    }
}

