using Assets.Scripts.Game;
using Assets.Scripts.Game.States;
using Assets.Scripts.Models;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: make this a singleton for convenience?
public class GameSession : NetworkBehaviour
{
    public GameplayManager GameplayManager = null;

    private GameStateContext gameState;
    private bool gameStarted = false;

    private void Start()
    {
        if (isServer)
        {
            GameplayManager.DominoTracker.CreateDominoSet();
            CreateAndPlaceNextEngine();
        }
    }

    private void Update()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            gameState = new GameStateContext(
                NetworkClient.connection.identity.GetComponent<DominoPlayer>(),
                GameplayManager);
        }

        gameState.Update();
    }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public List<GameObject> DealNewDominoes(int dominoCount)
    {
        var newDominoes = new List<GameObject>();

        for (int i = 0; i < dominoCount; i++)
        {
            var freshDomino = GameplayManager.GetNewPlayerDomino();

            // TODO: only do this if isLocalPlayer otherwise would spawn in all players' games??
            NetworkServer.Spawn(freshDomino, connectionToClient);

            newDominoes.Add(freshDomino);
        }

        return newDominoes;
    }

    [Server]
    public void CreateAndPlaceNextEngine()
    {
        var engineMesh = GameplayManager.GetNewEngineDomino();
        NetworkServer.Spawn(engineMesh);

        engineMesh.transform.position = Vector3.zero;
        GameplayManager.LayoutManager.PlaceEngine(engineMesh);
    }


    #endregion Server


    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();

        NetworkDebugger.OutputAuthority(this, nameof(OnStartLocalPlayer));

        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);
    }

    [Client]
    public void MovePlayerDominoes(List<GameObject> dominoes, bool hasAuthority)
    {
        if (hasAuthority)
        {
            GameplayManager.LayoutManager.PlacePlayerDominoes(dominoes);
        }
        else
        {
            // TODO: remove this. Currently displays other player's dominoes for debugging purposes only
            foreach (var domino in dominoes)
            {
                domino.transform.position = GameplayManager.LayoutManager.PlayerTopCenter + new Vector3(0, 10, 0); // TODO: out of sight out of mind [is dangerous]. I think clients should be creating their own, non-networked, dominoes
            }
        }
    }

    [Client]
    public void DisplayPlayersTurn(int callerNetId, bool isLocal, bool updateAll)
    {
        var isLocalTurn = false;

        if (isLocal)
        {
            isLocalTurn = true;
            if (GameplayManager.TurnManager.IsPlayerTurn(callerNetId))
            {
                GameplayManager.LayoutManager.SetHeaderText($"It is your turn");
            }
            else
            {
                GameplayManager.LayoutManager.SetHeaderText($"It is NOT your turn");
            }
        }

        if (updateAll)
        {
            if (!isLocalTurn)        // TODO: this works for 2 player but would not for more. How do we know who this client is? Instead maybe subscribe to an event in DominoPlayer so this runs for each player?
            {
                GameplayManager.LayoutManager.SetHeaderText($"It is your turn");
            }
            else
            {
                GameplayManager.LayoutManager.SetHeaderText($"It is NOT your turn");
            }
        }
    }

    [TargetRpc]
    public void RpcMoveSelectedDomino(NetworkConnection conn, GameObject selectedDomino, GameObject previouslySelectedDomino)
    {
        NetworkDebugger.OutputAuthority(this, nameof(RpcMoveSelectedDomino), true);

        GameplayManager.LayoutManager.SelectDomino(selectedDomino);

        if (previouslySelectedDomino != null)
        {
            GameplayManager.LayoutManager.DeselectDomino(previouslySelectedDomino);
        }
    }

    #endregion Client

}
