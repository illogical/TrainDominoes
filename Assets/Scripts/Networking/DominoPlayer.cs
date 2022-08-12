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
    [SerializeField] private List<GameObject> myDominoes = new List<GameObject>(); 

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    //public GameSession Session;       // TODO: Use GetComponent<GameSession>() to get this somehow. Need each player (or an object owned by the player) to request dominoes (GameSession to own the domino set)

    public bool GetIsPartyOwner() => isPartyOwner;
    public string GetDisplayName() => displayName;

    public void AddPlayerDomino(GameObject domino)
    {
        myDominoes.Add(domino);
    }


    public void Start()
    {
        NetworkDebugger.OutputAuthority(this, $"DominoPlayer.Start() ({displayName})", true);
    }

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

    [Server]
    public void AddPlayerDominoes(List<GameObject> playerDominoes)
    {
        foreach (var domino in playerDominoes)
        {
            NetworkServer.Spawn(domino, connectionToClient);

            if (isLocalPlayer)
            {
                AddPlayerDomino(domino);
            }
        }

        RpcShowDominoes(playerDominoes);
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

    /// <summary>
    /// Adds a single domino to a hand.
    /// </summary>
    [Command]
    public void CmdAddPlayerDomino()
    {
        var gameSession = FindObjectOfType<GameSession>();
        var freshDomino = gameSession.GetNewPlayerDomino();

        NetworkServer.Spawn(freshDomino, connectionToClient);    // TODO: will connectionToClient be null if this is sent from GameSession?
        AddPlayerDomino(freshDomino);
        RpcShowDominoes(new List<GameObject>() { freshDomino });
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
        
        if (isLocalPlayer)
        {
            myDominoes.AddRange(newDominoes);
        }

        // TODO: can this only run on the the local player? isLocalPlayer causes client to not line up any dominoes for themselves
        RpcShowDominoes(newDominoes);
    }

    [ClientRpc]
    public void RpcShowDominoes(List<GameObject> dominoes)
    {
        NetworkDebugger.OutputAuthority(this, nameof(RpcShowDominoes));

        var gameSession = FindObjectOfType<GameSession>();
        gameSession.MovePlayerDominoes(dominoes, hasAuthority);
    }

    [TargetRpc]
    public void RpcShowDominoes(NetworkConnection conn, List<GameObject> dominoes)
    {
        NetworkDebugger.OutputAuthority(this, nameof(RpcShowDominoes));

        var gameSession = FindObjectOfType<GameSession>();
        gameSession.MovePlayerDominoes(dominoes, hasAuthority);
    }


    #endregion

}
