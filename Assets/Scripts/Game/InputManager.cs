using Assets.Scripts.Models;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Camera MainCamera;
    public SelectionEvent BottomObjectClicked;
    //public SelectionEvent TrackAdded;
    //public SelectionEvent TrackSelected;

    // TODO: how to prevent clicks while other things are happening? Wait for an event to fire that allows selections to continue?

    // Update is called once per frame
    void Update()
    {
        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            var dominoEntity = hit.transform.gameObject.GetComponent<DominoEntity>();
            if(dominoEntity == null)
            {
                return;
            }

            MouseClickedObject(dominoEntity.ID, dominoEntity.Purpose);
        }
        
    }

    void MouseClickedObject(int id, PurposeType purpose)
    {
        // TODO: might need access to the parent object to know if this is a player or table domino
        print($"Domino {id} ({purpose}) was clicked!");
        BottomObjectClicked.RaiseEvent(id);
    }
}
