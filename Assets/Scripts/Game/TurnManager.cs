using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TurnManager : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnTurnChanged))]
    [SerializeField] private int turnIndex = 0;  // TODO: will this need to be a syncvar or can the server update/use this similar to the domino set?
    [SerializeField] private List<int> playerIds = new List<int>(); // TODO: subscribe to an event [on MyNetworkManager] to capture these? Mirror Basic example stores references to all Player objects in each Player (not ideal) 

    public static SelectionEvent turnChanged = null;

    public void Start()
    {
        foreach(var player in ((MyNetworkManager)NetworkManager.singleton).Players)
        {
            playerIds.Add(player.ID);
        }
    }

    public void NextTurn() => turnIndex = (turnIndex + 1) % playerIds.Count;

    public int GetCurrentPlayerId() => playerIds[turnIndex];

    public bool IsPlayerTurn(int netId) => playerIds[turnIndex] == netId;

    private void OnTurnChanged(int oldTurnIndex, int newTurnIndex)
    {
        turnChanged?.RaiseEvent(playerIds[newTurnIndex]);
    }
}
