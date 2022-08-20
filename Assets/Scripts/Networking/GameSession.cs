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
    public LayoutManager Layout = null;
    public TurnManager TurnManager = null;
    public MeshManager MeshManager = null;
    public GameplayManager GameplayManager = null;

    private bool gameStarted = false;

    private Dictionary<int, DominoInfo> dominoData = new Dictionary<int, DominoInfo>();
    private List<int> availableDominoes = new List<int>();

    private GameStateContext gameState;


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
            var freshDomino = GetNewPlayerDomino();

            // TODO: only do this if isLocalPlayer otherwise would spawn in all players' games??
            NetworkServer.Spawn(freshDomino, connectionToClient);

            newDominoes.Add(freshDomino);
        }

        return newDominoes;
    }

    [Server]
    public GameObject GetNewPlayerDomino()
    {
        var dominoInfo = GameplayManager.DominoTracker.GetDominoFromBonePile();
        return MeshManager.GetPlayerDomino(MeshManager.PlayerDominoPrefab, dominoInfo, Layout.PlayerBottomCenter);
    }

    [Server]
    public GameObject GetNewEngineDomino()
    {
        var dominoInfo = GameplayManager.DominoTracker.GetNextEngine();
        return MeshManager.GetEngineDomino(MeshManager.TableDominoPrefab, dominoInfo, Vector3.zero);
    }

    [Server]
    public void CreateAndPlaceNextEngine()
    {
        var engineMesh = GetNewEngineDomino();
        NetworkServer.Spawn(engineMesh);

        engineMesh.transform.position = Vector3.zero;
        Layout.PlaceEngine(engineMesh);
    }

    [Server]
    public void EndTurn(int callerNetId)
    {
        if (!TurnManager.IsPlayerTurn(callerNetId)) { return; }

        TurnManager.NextTurn();

        Debug.Log($"It is Player {TurnManager.GetCurrentPlayerId()}'s turn. It was {callerNetId}'s turn.");
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
            Layout.PlacePlayerDominoes(dominoes);
        }
        else
        {
            // TODO: remove this. Currently displays other player's dominoes for debugging purposes only
            foreach (var domino in dominoes)
            {
                domino.transform.position = Layout.PlayerTopCenter + new Vector3(0, 10, 0); // TODO: out of sight out of mind [is dangerous]. I think clients should be creating their own, non-networked, dominoes
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
            if (TurnManager.IsPlayerTurn(callerNetId))
            {
                Layout.SetHeaderText($"It is your turn");
            }
            else
            {
                Layout.SetHeaderText($"It is NOT your turn");
            }
        }

        if (updateAll)
        {
            if (!isLocalTurn)        // TODO: this works for 2 player but would not for more. How do we know who this client is? Instead maybe subscribe to an event in DominoPlayer so this runs for each player?
            {
                Layout.SetHeaderText($"It is your turn");
            }
            else
            {
                Layout.SetHeaderText($"It is NOT your turn");
            }
        }
    }

    [TargetRpc]
    public void RpcMoveSelectedDomino(NetworkConnection conn, GameObject selectedDomino, GameObject previouslySelectedDomino)
    {
        NetworkDebugger.OutputAuthority(this, nameof(RpcMoveSelectedDomino), true);

        Layout.SelectDomino(selectedDomino);

        if (previouslySelectedDomino != null)
        {
            Layout.DeselectDomino(previouslySelectedDomino);
        }
    }

    #endregion Client

}
