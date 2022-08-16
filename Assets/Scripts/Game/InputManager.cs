using Assets.Scripts.Models;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    public Camera MainCamera;
    public SelectionEvent BottomObjectClicked;
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
        // TODO: how do we know which player clicked the mouse? This seems to only run on the local client
        // TODO: might need access to the parent object to know if this is a player or table domino
        Debug.Log($"Domino {id} ({purpose}) was clicked via InputManager");

        //BottomObjectClicked.RaiseEvent(id);
        NetworkIdentity identity = NetworkClient.connection.identity;
        var dominoPlayer = identity.GetComponent<DominoPlayer>();

        int netId = (int)NetworkClient.connection.identity.netId;

        dominoPlayer.CmdSelectDomino(id, netId);


        // TODO: figure out how events work

    }
}
