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

    private PlayerDominoes playerDominoes = new PlayerDominoes();
    private GameObject selectedDomino = null;

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

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner) { return; }

        ((MyNetworkManager)NetworkManager.singleton).StartGame();
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

    #endregion


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
    public void CmdSelectDomino(int dominoId, int netId)
    {
        NetworkDebugger.OutputAuthority(this, $"DominoPlayer.{nameof(CmdSelectDomino)}", true);

        var gameSession = FindObjectOfType<GameSession>();
        var dominoObject = gameSession.GetDominoById(dominoId);

        gameSession.RpcMoveSelectedDomino(connectionToClient, dominoObject, selectedDomino);
        selectedDomino = dominoObject;
    }

    /// <summary>
    /// Adds multiple dominoes to a hand for starting the game.
    /// </summary>
    /// <param name="dominoCount"></param>
    [Command]
    public void CmdAddPlayerDominoes(int dominoCount)
    {
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
            playerDominoes.AddDominoes((int)connectionToClient.identity.netId, dominoIds);
        }

        // TODO: can this only run on the the local player? isLocalPlayer causes client to not line up any dominoes for themselves
        //RpcSetPlayerDominoPositions(connectionToClient, newDominoes);
        RpcSetPlayerDominoPositions(newDominoes);
    }

    [ClientRpc]
    public void RpcSetPlayerDominoPositions(List<GameObject> dominoes)
    {
        NetworkDebugger.OutputAuthority(this, nameof(RpcSetPlayerDominoPositions));

        var gameSession = FindObjectOfType<GameSession>();
        gameSession.MovePlayerDominoes(dominoes, hasAuthority);

    }

    #endregion

}
