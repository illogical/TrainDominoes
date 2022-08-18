using Assets.Scripts.Game;
using Assets.Scripts.Models;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: make this a singleton for convenience?
public class GameSession : NetworkBehaviour
{
    [SerializeField] private GameObject playerDominoPrefab = null;
    [SerializeField] private GameObject tableDominoPrefab = null;

    //private int dominoCount = 12;
    private Dictionary<int, DominoInfo> dominoData = new Dictionary<int, DominoInfo>();
    private List<int> availableDominoes = new List<int>();
    private Dictionary<int, GameObject> dominoObjects = new Dictionary<int, GameObject>();   // TODO: now both clients know about each other's dominoes. Feels unsure.

    private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

    private Vector3 playerTopCenter = new Vector3(0, 0.08f, 0);
    private Vector3 playerBottomCenter = new Vector3(0, -0.08f, 0);
    private Vector3 tablePosition = new Vector3(0, 0, 0);

    private GameObject tableDomino;

    private bool gameStarted = false;

    public LayoutManager Layout = null;
    public TurnManager TurnManager = null;

    public SelectionEvent PlayerDominoSelected;

    [HideInInspector]
    public DominoTracker DominoTracker = new DominoTracker();


    private void Start()
    {
        //NetworkIdentity identity = NetworkClient.connection.identity;
        //dominoPlayer = identity.GetComponent<DominoPlayer>();
        //dominoPlayer.CmdDealDomino();

        //PlayerDominoSelected?.OnEventRaised.AddListener(HandlePlayerDominoClicked);

        StartGame();
    }

    private void Update()
    {
        // TODO: this is all sloppy. CreateTableDomino
        if(!gameStarted)
        {
            gameStarted = true;
            
            if(isServer)
            {
                CreateAndPlaceNextEngine();
            }
        }
    }

    private void OnDestroy()
    {
        //PlayerDominoSelected?.OnEventRaised.RemoveListener(HandlePlayerDominoClicked);
    }

    public void StartGame()
    {
        NetworkDebugger.OutputAuthority(this, $"GameSession.{nameof(StartGame)}", true);

        if (isServer)
        {
            DominoTracker.CreateDominoSet();
        }
    }

    // TODO: introduce state machine soon because different states will subscribe to relevant events
    public void HandlePlayerDominoClicked(int id)
    {
        // TODO: notice that this never has authority on client or server
        NetworkDebugger.OutputAuthority(this, $"GameSession.{nameof(HandlePlayerDominoClicked)}", true);

        //RpcMoveSelectedDomino(dominoObject);    // TODO: why is connectionToClient always null here? (running on server only but how to allow clients to call this? BAH it should be a command but can an even call a command? It was a Server function prior)
        NetworkIdentity identity = NetworkClient.connection.identity;
        var dominoPlayer = identity.GetComponent<DominoPlayer>();
        dominoPlayer.CmdSelectDomino(id, (int)NetworkClient.connection.identity.netId);

    }

    public GameObject GetDominoById(int id)
    {
        return dominoObjects[id];
    }

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        DontDestroyOnLoad(gameObject);
    }

    [Server]
    public DominoInfo GetNextDomino()
    {
        if(availableDominoes.Count == 0)
        {
            Debug.LogError("Server is out of dominoes. Game over?");
        }

        var nextDominoEntity = dominoData[availableDominoes[0]];

        availableDominoes.RemoveAt(0);

        return nextDominoEntity;
    }

    [Server]
    public List<GameObject> DealNewDominoes(int dominoCount)
    {
        var newDominoes = new List<GameObject>();

        for (int i = 0; i < dominoCount; i++)
        {
            var freshDomino = GetNewPlayerDomino();
            var dominoInfo = freshDomino.GetComponent<DominoEntity>();

            // TODO: only do this if isLocalPlayer otherwise would spawn in all players' games??
            NetworkServer.Spawn(freshDomino, connectionToClient);

            newDominoes.Add(freshDomino);
        }

        return newDominoes;
    }

    [Server]
    public GameObject GetNewPlayerDomino()
    {
        var dominoInfo = DominoTracker.GetDominoFromBonePile();
        return CreateDominoFromInfo(playerDominoPrefab, dominoInfo, playerBottomCenter, PurposeType.Player);
    }

    [Server]
    public GameObject GetNewEngineDomino()
    {
        var dominoInfo = DominoTracker.GetNextEngine();
        return CreateDominoFromInfo(tableDominoPrefab, dominoInfo, Vector3.zero, PurposeType.Engine);
    }

    [Server]
    public GameObject CreateDominoFromInfo(GameObject prefab, DominoInfo info, Vector3 position, PurposeType purpose)         // TODO: move to MeshManager
    {
        var newDomino = Instantiate(prefab, position, dominoRotation);
        newDomino.name = info.ID.ToString();    // TODO: this only sets the name on the server

        var dom = newDomino.GetComponent<DominoEntity>();
        dom.ID = info.ID;
        dom.TopScore = info.TopScore;
        dom.BottomScore = info.BottomScore;
        dom.Purpose = purpose;

        dominoObjects.Add(info.ID, newDomino);

        return newDomino;
    }

    [Server]
    public void CreateAndPlaceNextEngine()
    {
        tableDomino = GetNewEngineDomino();
        NetworkServer.Spawn(tableDomino);

        tableDomino.transform.position = Vector3.zero;
        Layout.PlaceEngine(tableDomino);
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
                domino.transform.position = playerTopCenter;
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
