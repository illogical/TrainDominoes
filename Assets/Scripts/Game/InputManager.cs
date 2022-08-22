using Assets.Scripts.Models;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    public Camera MainCamera;
    public SelectionEvent DominoClicked;
    //public SelectionEvent TrackAdded;
    //public SelectionEvent TrackSelected;

    // TODO: how to prevent clicks while other things are happening? Wait for an event to fire that allows selections to continue?

    // Update is called once per frame
    [ClientCallback]
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

            if(isClient)
            {
                MouseClickedObject(dominoEntity.ID, dominoEntity.Purpose);
            }
        }
        
    }

    void MouseClickedObject(int id, PurposeType purpose)
    {
        Debug.Log($"Domino {id} ({purpose}) was clicked via InputManager");

        NetworkIdentity identity = NetworkClient.connection.identity;
        var dominoPlayer = identity.GetComponent<DominoPlayer>();

        dominoPlayer.CmdDominoClicked(id);    // let this determine if a client should fire off the PlayerDominoSelected event for the GameStateContext
    }
}
