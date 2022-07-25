using Assets.Scripts.Models;
using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class GameSession : NetworkBehaviour
{
    [SerializeField] private GameObject dominoPrefab = null;

    private DominoPlayer dominoPlayer;  // TODO: reference to current local player might be convenient
    private SyncDictionary<int, DominoEntity> dominoData = new SyncDictionary<int, DominoEntity>();
    private SyncList<int> availableDominoes = new SyncList<int>();  // TODO: ensure the clients don't have this list

    private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

    private Vector3 playerTopCenter = new Vector3(0, 1.07f, -9.87f);
    private Vector3 playerBottomCenter = new Vector3(0, 0.93f, -9.87f);




    private void Start()
    {
        // TODO: this almost works. Major flaw though. The client-only loses track of the domino that the server/client has.
        //NetworkIdentity identity = NetworkClient.connection.identity;

        //dominoPlayer = identity.GetComponent<DominoPlayer>();
        //dominoPlayer.CmdDealDomino();

        //if(hasAuthority)
        //{
        //    // TODO: run command to create Domino. Later I want this to create a Round and GameManager can create the dominoes.
        //}
    }


    public void StartGame()
    {
        CreateFakeDominoes();


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

        StartGame();
    }

    [Server]
    private void CreateFakeDominoes()
    {
        for (int i = 0; i < 10; i++)
        {
            //GameObject dominoInstance = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
            //dominoInstance.GetComponent<DominoEntity>();        
            var dominoEntity = new DominoEntity()
            { 
                ID = i + 1,
                TopScore = i + 1,
                BottomScore = i + 1
            }; 
               
            dominoData.Add(i, dominoEntity);
            availableDominoes.Add(i);
        }
    }

    [Server]
    public GameObject GetNextDomino()
    {
        if(availableDominoes.Count == 0)
        {
            Debug.LogError("Server is out of dominoes");
        }
        //int nextIndex = currentDominoIndex;
        //currentDominoIndex = Mathf.Clamp(currentDominoIndex + 1, 0, dominoData.Count - 1);

        var nextDominoEntity = dominoData[availableDominoes[0]];

        GameObject dominoInstance = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
        var dom = dominoInstance.GetComponent<DominoEntity>();
        dom.ID = nextDominoEntity.ID;
        dom.TopScore = nextDominoEntity.ID;
        dom.BottomScore = nextDominoEntity.ID;

        availableDominoes.RemoveAt(0);

        return dominoInstance;
    }

    [Server]
    public void Test(NetworkConnectionToClient conn)      // THIS WORKED!!!!!!! Only for the client who is also the server :(
    {
        // TODO: execute CmdDealDomino() when the turn begins for this player
        var newDomino = GetNextDomino();
        NetworkServer.Spawn(newDomino, conn);
        //AddPlayerDomino(newDomino);

        RpcShowDominoes(newDomino);
    }

    #endregion Server


    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);
    }

    // TODO: maybe instead make this event based? could invoke an event on the player

    //[Command(requiresAuthority = false)]
    [Command(requiresAuthority = false)]
    public void CmdDealDomino()
    {
        if (!isServer) { return; }
        // TODO: execute CmdDealDomino() when the turn begins for this player
        var newDomino = GetNextDomino(); // ((MyNetworkManager)NetworkManager.singleton).GetNextDomino(); // TODO: this has to happen on the server
        NetworkServer.Spawn(newDomino, connectionToClient);
        //AddPlayerDomino(newDomino);

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
            // TODO: no longer render the other player's dominoes
            domino.transform.position = playerTopCenter;
        }
    }

    [ClientRpc]
    public void RpcShowTableDominoes(GameObject domino)
    {
        var dominoEntity = domino.GetComponent<DominoEntity>();
        dominoEntity.UpdateDominoLabels();

        var mover = domino.GetComponent<Mover>();
        domino.transform.position = Vector3.zero;// new Vector3(1, 1, 1);
        //StartCoroutine(mover.MoveOverSeconds(Vector3.zero, 0.3f, 0));
    }

    #endregion Client
}
