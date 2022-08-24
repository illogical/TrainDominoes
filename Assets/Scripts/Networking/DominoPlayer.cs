using Assets.Scripts.Models;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public void CmdSelectPlayerDomino(int dominoId, int? lastSelectedId)
    {
        var gameSession = FindObjectOfType<GameSession>();
        // TODO: check if this is on the table and if so then ignore it. After the state machine is implemented then it would take different action if a domino is selected.
        var dominoObject = gameSession.GameplayManager.MeshManager.GetDominoMeshById(dominoId);
        GameObject lastSelectedDomino = lastSelectedId.HasValue 
            ? gameSession.GameplayManager.MeshManager.GetDominoMeshById(lastSelectedId.Value) 
            : null;

        gameSession.RpcMoveSelectedDomino(connectionToClient, dominoObject, lastSelectedDomino);
    }

    [Command]
    public void CmdAddDominoToNewTrack(int selectedDominoId, int engineDominoId)
    {
        var gameSession = FindObjectOfType<GameSession>();

        Debug.Log("Pretend a new track was added.");

        // TODO: add logic to check if these dominoes match

    }

    [Command]
    public void CmdEngineClicked(int selectedDominoId)
    {
        RpcRaiseCreateNewTrack(connectionToClient, selectedDominoId);
    }

    [Command]
    public void CmdDominoClicked(int dominoId)
    {
        var gameSession = FindObjectOfType<GameSession>();
        int netId = (int)connectionToClient.identity.netId;
        if (!gameSession.GameplayManager.TurnManager.IsPlayerTurn(netId))
        {
            Debug.Log("It is not your turn");
            return;
        }

        if(!gameSession.GameplayManager.DominoTracker.IsPlayerDomino(netId, dominoId))
        {
            // TODO: check if this is an engine or table domino and fire events for each

            RpcRaiseSelectEngineDomino(connectionToClient, dominoId);
            return;
        }

        RpcRaisePlayerSelectedDomino(connectionToClient, dominoId);
    }

    /// <summary>
    /// Adds multiple dominoes to a hand for starting the game.
    /// </summary>
    /// <param name="dominoCount"></param>
    [Command]
    public void CmdDealDominoes(int dominoCount)
    {
        var gameSession = FindObjectOfType<GameSession>();
        var newDominoes = gameSession.DealNewDominoes(dominoCount);

        if(connectionToClient != null)
        {
            // store IDs of the dominoes that this player has in their hand
            int netId = (int)connectionToClient.identity.netId;
            gameSession.GameplayManager.DominoTracker.AddPlayerDominoes(
                netId, 
                newDominoes.Select(d => d.GetComponent<DominoEntity>().ID).ToList());

            RpcSetPlayerTurnText(netId, false);
        }

        RpcSetPlayerDominoPositions(newDominoes);   
    }

    [Command]
    public void CmdEndTurn(int netId)
    {
        var gameSession = FindObjectOfType<GameSession>();
        gameSession.GameplayManager.EndTurn(netId);
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

    [TargetRpc]
    public void RpcRaisePlayerSelectedDomino(NetworkConnection conn, int dominoId)
    {
        var gameSession = FindObjectOfType<GameSession>();
        gameSession.GameplayManager.PlayerDominoSelected?.RaiseEvent(dominoId);
    }

    [TargetRpc]
    public void RpcRaiseSelectEngineDomino(NetworkConnection conn, int dominoId)
    {
        var gameSession = FindObjectOfType<GameSession>();
        gameSession.GameplayManager.EngineDominoSelected?.RaiseEvent(dominoId);
    }

    [TargetRpc]
    public void RpcRaiseCreateNewTrack(NetworkConnection conn, int dominoId)
    {
        var gameSession = FindObjectOfType<GameSession>();
        gameSession.GameplayManager.CreateTrackWithDomino?.RaiseEvent(dominoId);
    }

    #endregion Client

}
