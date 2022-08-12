using Assets.Scripts.Game;
using Assets.Scripts.Helpers;
using Assets.Scripts.Models;
using Mirror;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// TODO: make this a singleton for convenience?
public class GameSession : NetworkBehaviour
{
    [SerializeField] private GameObject playerDominoPrefab = null;
    [SerializeField] private GameObject tableDominoPrefab = null;

    private int dominoCount = 12;
    private Dictionary<int, DominoInfo> dominoData = new Dictionary<int, DominoInfo>();
    private List<int> availableDominoes = new List<int>();  // TODO: ensure the clients don't have this list
    private List<GameObject> playerDominoes = new List<GameObject>();

    private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

    private Vector3 playerTopCenter = new Vector3(0, 0.08f, 0);
    private Vector3 playerBottomCenter = new Vector3(0, -0.08f, 0);
    private Vector3 tablePosition = new Vector3(0, 0, 0);

    private GameObject tableDomino;

    private bool gameStarted = false;

    public LayoutManager LayoutManager = null;
    [HideInInspector]
    public DominoTracker DominoTracker = new DominoTracker();

    public SelectionEvent PlayerDominoSelected;


    private void Start()
    {
        //NetworkIdentity identity = NetworkClient.connection.identity;
        //dominoPlayer = identity.GetComponent<DominoPlayer>();
        //dominoPlayer.CmdDealDomino();

        PlayerDominoSelected?.OnEventRaised.AddListener(HandlePlayerDominoClicked);

        StartGame();
    }

    private void Update()
    {
        if(!gameStarted)
        {
            gameStarted = true;
            
            if(isServer)
            {
                CreateTableDomino();
            }
        }
    }

    private void OnDestroy()
    {
        PlayerDominoSelected?.OnEventRaised.RemoveListener(HandlePlayerDominoClicked);
    }

    public void StartGame()
    {
        NetworkDebugger.OutputAuthority(this, nameof(StartGame), true);

        if (isServer)
        {
            DominoTracker.CreateDominoSet();

            //AddPlayerDominoes(dominoCount);
        }
    }

    // TODO: introduce state machine soon because different states will subscribe to relevant events
    public void HandlePlayerDominoClicked(int id)
    {
        // TODO: notice that this never has authority on client or server. Wtf?
        NetworkDebugger.OutputAuthority(this, $"GameSession.{nameof(HandlePlayerDominoClicked)}", true);
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
            Debug.LogError("Server is out of dominoes");
        }

        //int nextIndex = currentDominoIndex;
        //currentDominoIndex = Mathf.Clamp(currentDominoIndex + 1, 0, dominoData.Count - 1);

        var nextDominoEntity = dominoData[availableDominoes[0]];       

        availableDominoes.RemoveAt(0);

        return nextDominoEntity;
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

        return newDomino;
    }

    [Server]
    public void CreateTableDomino()
    {
        tableDomino = GetNewEngineDomino();
        NetworkServer.Spawn(tableDomino);

        // only happens on the server
        //tableDomino.transform.position = tablePosition;
        //LayoutManager.PlaceEngine(tableDomino);

        tableDomino.transform.position = Vector3.zero;
        LayoutManager.PlaceEngine(tableDomino);

        //RpcShowTableDominoes(tableDomino);
    }

    //[Command(requiresAuthority = false)]
    [Command] // reminder: runs on server but is called by client
    public void CmdDealDominoes()
    {
        var dominoInfo = GetNextDomino();

        var newDomino = GetNewPlayerDomino();
        var dom = tableDomino.GetComponent<DominoEntity>();
        // TODO: move this to when the server creates the instance
        dom.ID = dominoInfo.ID;
        dom.TopScore = dominoInfo.TopScore;
        dom.BottomScore = dominoInfo.BottomScore;

        NetworkServer.Spawn(newDomino, connectionToClient);
        //AddPlayerDomino(newDomino);

        RpcShowDominoes(newDomino);
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


    [ClientRpc]
    public void RpcShowDominoes(GameObject domino)
    {
        if (hasAuthority)
        {
            var mover = domino.GetComponent<Mover>();
            domino.transform.position = new Vector3(0, 0, 0);

            // animate the movement for the current player
            StartCoroutine(mover.MoveOverSeconds(playerBottomCenter, 0.5f, 0));
        }
        else
        {
            // TODO: no longer render the other player's dominoes. May want to use TargetRpc instead and check for isLocalPlayer before executing it and passing connectionToClient to it.
            domino.transform.position = playerTopCenter;
        }
    }

    [ClientRpc]
    public void RpcShowDominoes(List<GameObject> dominoes)
    {
        NetworkDebugger.OutputAuthority(this, nameof(RpcShowDominoes));

        LayoutManager.PlacePlayerDominoes(dominoes);

        for (int i = 0; i < dominoes.Count; i++)
        {
            MovePlayerDomino(dominoes[i], i, hasAuthority);
        }
    }

    /// <summary>
    /// Adds multiple dominoes to a hand for starting the game.
    /// </summary>
    /// <param name="dominoCount"></param>
    [Server]
    public void AddPlayerDominoes(int dominoCount)
    {
        var newDominoes = new List<GameObject>();

        for (int i = 0; i < dominoCount; i++)
        {
            var freshDomino = GetNewPlayerDomino();
            NetworkServer.Spawn(freshDomino, connectionToClient);    // TODO: will connectionToClient be null if this is sent from GameSession?
            
            // TODO: need to add domino to DominoPlayer for localPlayer
            if(isLocalPlayer)
            {
                //AddPlayerDomino(freshDomino);
            }

            newDominoes.Add(freshDomino);
        }

        // TODO: get current player and add dominoes
        //DominoPlayer player = connectionToClient.identity.GetComponent<DominoPlayer>(); // TODO: will connectionToClient be null if this is sent from GameSession?
        //player.AddPlayerDominoes(newDominoes);

        //RpcShowDominoes(newDominoes);
    }

    [ClientRpc]
    public void RpcShowTableDominoes(GameObject domino) // TODO: kill this now that this is rightfully done on the server
    {
        NetworkDebugger.OutputAuthority(this, nameof(RpcShowTableDominoes), true);

        domino.transform.position = Vector3.zero;
        LayoutManager.PlaceEngine(domino);
    }

    #endregion Client


    public void MovePlayerDomino(GameObject domino, int index, bool hasAuthority)
    {
        if (hasAuthority)
        {
            var mover = domino.GetComponent<Mover>();
            domino.transform.position = new Vector3(0, 0, 0);   // TODO: get next position based upon index in ObjectGroup

            // animate the movement for the current player
            StartCoroutine(mover.MoveOverSeconds(playerBottomCenter, 0.5f, 0));
        }
        else
        {
            // TODO: no longer render the other player's dominoes
            domino.transform.position = playerTopCenter;
        }
    }

    public void MovePlayerDominoes(List<GameObject> dominoes, bool hasAuthority)
    {
        if(hasAuthority)
        {
            LayoutManager.PlacePlayerDominoes(dominoes);
        }
        else
        {
            // TODO: remove this. Currently displays other player's dominoes for debugging purposes only

            foreach(var domino in dominoes)
            {
                domino.transform.position = playerTopCenter;
            }
        }
    }
}
