using Assets.Scripts.Models;
using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DominoPlayer : NetworkBehaviour
{
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;

    [SyncVar]
    [HideInInspector] 
    public int ID;

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    //public GameSession Session;       // TODO: Use GetComponent<GameSession>() to get this somehow. Need each player (or an object owned by the player) to request dominoes (GameSession to own the domino set)

    public bool GetIsPartyOwner() => isPartyOwner;
    public string GetDisplayName() => displayName;


    #region Server

    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public void SetPartyOwner(bool state)
    {
        isPartyOwner = state;
    }

    [Server]
    public void SetDisplayName(string name)
    {
        displayName = name;
    }

    #endregion Server


    #region Client

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);

        ((MyNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) { return; }

        ((MyNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) { return; }
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    private void ClientHandleDisplayNameUpdated(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) { return; }

        ((MyNetworkManager)NetworkManager.singleton).StartGame();
    }

    [Command]
    public void CmdSelectDomino(int dominoId, int? lastSelectedId)
    {
        var gameSession = FindObjectOfType<GameSession>();
        // TODO: check if this is on the table and if so then ignore it. After the state machine is implemented then it would take different action if a domino is selected.
        var dominoObject = gameSession.MeshManager.GetDominoMeshById(dominoId);
        GameObject lastSelectedDomino = lastSelectedId.HasValue ? gameSession.MeshManager.GetDominoMeshById(lastSelectedId.Value) : null;

        gameSession.RpcMoveSelectedDomino(connectionToClient, dominoObject, lastSelectedDomino);
    }

    /// <summary>
    /// Adds multiple dominoes to a hand for starting the game.
    /// </summary>
    /// <param name="dominoCount"></param>
    [Command]
    public void CmdDealDominoes(int dominoCount)
    {
        // TODO: move this logic into MeshManager (except for the Rpc call)
        var gameSession = FindObjectOfType<GameSession>();
        var newDominoes = gameSession.DealNewDominoes(dominoCount);

        if(connectionToClient != null)
        {
            var dominoIds = new List<int>();
            foreach(var dom in newDominoes)
            {
                dominoIds.Add(dom.GetComponent<DominoEntity>().ID);
            }

            // store IDs of the dominoes that this player has in their hand
            int netId = (int)connectionToClient.identity.netId;
            gameSession.GameplayManager.DominoTracker.AddPlayerDominoes(netId, dominoIds);
            RpcSetPlayerTurnText(netId, false);
        }

        // TODO: can this only run on the the local player? isLocalPlayer causes client to not line up any dominoes for themselves
        RpcSetPlayerDominoPositions(newDominoes);   
    }

    [Command]
    public void CmdEndTurn(int netId)
    {
        var gameSession = FindObjectOfType<GameSession>();
        gameSession.EndTurn(netId);
        RpcSetPlayerTurnText((int)connectionToClient.identity.netId, true);
    }

    [ClientRpc]
    public void RpcSetPlayerDominoPositions(List<GameObject> dominoes)
    {
        var gameSession = FindObjectOfType<GameSession>();
        gameSession.MovePlayerDominoes(dominoes, hasAuthority);
    }

    [ClientRpc]
    public void RpcSetPlayerTurnText(int netId, bool updateAll)
    {
        var gameSession = FindObjectOfType<GameSession>();
        gameSession.DisplayPlayersTurn(netId, isLocalPlayer, updateAll);
    }

    #endregion Client

}
