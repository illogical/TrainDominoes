using Assets.Scripts.Models;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DominoPlayer : NetworkBehaviour
{
    [SerializeField] private Transform cameraTransform = null;
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;
    [SyncVar(hook = nameof(ClientHandleDisplayNameUpdated))]
    private string displayName;
    [SerializeField] private List<GameObject> myDominoes = new List<GameObject>(); // TODO: Not sure if DominoEntity needs to be a NetworkBehviour

    public static event Action ClientOnInfoUpdated;
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;
    public event Action<int> ClientOnResourcesUpdated;

    public bool GetIsPartyOwner() => isPartyOwner;
    public string GetDisplayName() => displayName;

    // TODO: Move this logic elsewhere
    private Vector3 playerTopCenter = new Vector3(0, 1.07f, -9.87f);
    private Vector3 playerBottomCenter = new Vector3(0, 0.93f, -9.87f);

    public void AddPlayerDomino(GameObject domino)
    {
        myDominoes.Add(domino);
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
    public void CmdDealDomino()
    {
        // TODO: execute CmdDealDomino() when the turn begins for this player
        var newDomino = ((MyNetworkManager)NetworkManager.singleton).GetNextDomino();        
        NetworkServer.Spawn(newDomino, connectionToClient);

        RpcShowDominoes(newDomino);
    }

    [ClientRpc]
    public void RpcShowDominoes(GameObject domino)
    {
        var dominoEntity = domino.GetComponent<DominoEntity>();
        dominoEntity.UpdateDominoLabels();

        if (hasAuthority)
        {
            var mover = domino.GetComponent<Mover>();
            domino.transform.position = new Vector3(0, 0, 0);

            // animate the movement for the current player
            StartCoroutine(mover.MoveOverSeconds(playerBottomCenter, 0.5f, 0));
        }
        else
        {
            domino.transform.position = playerTopCenter;
        }
    }

    #endregion

}
