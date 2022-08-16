using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour    // TODO: TurnManager is only a MonoBehaviour for debugging purposes (so I can swap whose turn it is)
{
    [SerializeField] private int turnIndex = 0;  // TODO: will this need to be a syncvar or can the server update/use this similar to the domino set?
    [SerializeField] private List<uint> playerIds = new List<uint>(); // TODO: subscribe to an event [on MyNetworkManager] to capture these? Mirror Basic example stores references to all Player objects in each Player (not ideal) 
    
    public TurnManager()
    {
        
    }

    public void NextTurn() => turnIndex = turnIndex++ % playerIds.Count;

    public uint GetCurrentPlayerId() => playerIds[turnIndex];

    public bool IsPlayerTurn(uint netId) => playerIds[turnIndex] == netId;

}
