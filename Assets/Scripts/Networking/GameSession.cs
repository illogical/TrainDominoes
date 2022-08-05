using Assets.Scripts.Game;
using Assets.Scripts.Models;
using Mirror;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// TODO: make this a singleton for convenience?
public class GameSession : NetworkBehaviour
{
    [SerializeField] private GameObject dominoPrefab = null;

    private DominoPlayer dominoPlayer;  // TODO: reference to current local player might be convenient
    private Dictionary<int, DominoInfo> dominoData = new Dictionary<int, DominoInfo>();
    private List<int> availableDominoes = new List<int>();  // TODO: ensure the clients don't have this list

    private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

    private Vector3 playerTopCenter = new Vector3(0, 1.07f, -9.87f);
    private Vector3 playerBottomCenter = new Vector3(0, 0.93f, -9.87f);
    private Vector3 tablePosition = new Vector3(0, 1, -9.8f);

    private GameObject tableDomino;

    private bool gameStarted = false;

    public DominoTracker Dominoes = new DominoTracker();


    private void Start()
    {
        //NetworkIdentity identity = NetworkClient.connection.identity;
        //dominoPlayer = identity.GetComponent<DominoPlayer>();
        //dominoPlayer.CmdDealDomino();

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

    public void StartGame()
    {
        NetworkDebugger.OutputAuthority(this, nameof(StartGame), true);

        if (isServer)
        {
            //CreateFakeDominoes();
            Dominoes.CreateDominoSet();
        }


        // TODO: how to create a table domino?
        //var firstDomino = GetNextDomino();
        //NetworkServer.Spawn(firstDomino);
        //RpcShowTableDominoes(firstDomino);
    }


    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();

        DontDestroyOnLoad(gameObject);
    }

    [Server]
    private void CreateFakeDominoes()
    {
        for (int i = 0; i < 10; i++)
        {
            //GameObject dominoInstance = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
            //dominoInstance.GetComponent<DominoEntity>();        
            //var dominoEntity = new DominoEntity()
            //{ 
            //    ID = i + 1,
            //    TopScore = i + 1,
            //    BottomScore = i + 1
            //};

            //var dominoInstance = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
            //var dominoEntity = dominoInstance.GetComponent<DominoEntity>();
            var score = i + 1;
            var dominoInfo = new DominoInfo()
            {
                ID = score,
                TopScore = score,
                BottomScore = score,
            };
            
            //dominoEntity.ID = score;
            //dominoEntity.TopScore = score;
            //dominoEntity.BottomScore = score;

            dominoData.Add(i, dominoInfo);
            availableDominoes.Add(i);
        }
    }


    // TODO: use these instead of using DominoPlayer
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
    public GameObject GetNewDomino()   // TODO: pass int to determine how many dominoes to create. Need this for StartGame()
    {
        var dominoInfo = Dominoes.GetDominoFromBonePile();

        var newDomino = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
        var dom = newDomino.GetComponent<DominoEntity>();
        dom.ID = dominoInfo.ID;
        dom.TopScore = dominoInfo.TopScore;
        dom.BottomScore = dominoInfo.BottomScore;

        return newDomino;
    }

    [Server]
    public void CreateTableDomino()
    {
        tableDomino = GetNewDomino();
        NetworkServer.Spawn(tableDomino);

        RpcShowTableDominoes(tableDomino);
    }

    //[Command(requiresAuthority = false)]
    [Command] // runs on server but is called by client
    public void CmdDealDomino()
    {
        var dominoInfo = GetNextDomino();

        var newDomino = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
        var dom = tableDomino.GetComponent<DominoEntity>();
        // TODO: move this to when the server creates the instance
        dom.ID = dominoInfo.ID;
        dom.TopScore = dominoInfo.ID;
        dom.BottomScore = dominoInfo.ID;

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
            // TODO: no longer render the other player's dominoes
            domino.transform.position = playerTopCenter;
        }
    }

    [ClientRpc]
    public void RpcShowTableDominoes(GameObject domino)
    {
        NetworkDebugger.OutputAuthority(this, nameof(RpcShowTableDominoes), true);
        var dominoEntity = domino.GetComponent<DominoEntity>();
        dominoEntity.UpdateDominoLabels();

        var mover = domino.GetComponent<Mover>();
        domino.transform.position = tablePosition;//Vector3.zero;
        // TODO: why does this animation only work on the client that is the server? Probably because it has authority.
        StartCoroutine(mover.MoveOverSeconds(tablePosition, 0.5f, 0));       //domino.transform.position += new Vector3(0, 1, -9.8f);
    }

    #endregion Client
}
