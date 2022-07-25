using Assets.Scripts.Game;
using Assets.Scripts.Models;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private GameObject dominoPrefab = null;
    [SerializeField] private GameOverHandler gameOverHandler = null;
    //[SerializeField] private GameSession gameSessionPrefab = null;            // TODO: does this need to be a SyncVar??

    public bool IsGameInProgress = false;
    public List<DominoPlayer> Players = new List<DominoPlayer>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public void SetTransport(Transport t) => transport = t;

    // TODO: move this to GameManager when GameManager is ready to be used
    private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

    // TODO: move this to GameManager when GameManager is ready to be used
    private List<GameObject> allDominoes = new List<GameObject>();
    //private int currentDominoIndex = 0;

    private DominoTracker dominoTracker = new DominoTracker();


    public List<GameObject> GetDominoes() => allDominoes;
    public GameObject GetNextDomino()
    {
        var nextDominoEntity = dominoTracker.GetNextDomino();
        
        // TODO: move the mesh creation to a MeshManager. How can the mesh manager decide where to position this?
        GameObject dominoInstance = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
        var dom = dominoInstance.GetComponent<DominoEntity>();
        dom.ID = nextDominoEntity.ID;
        dom.TopScore = nextDominoEntity.ID;
        dom.BottomScore = nextDominoEntity.ID;

        return dominoInstance;

        //int nextIndex = currentDominoIndex;
        //currentDominoIndex = Mathf.Clamp(currentDominoIndex + 1, 0, allDominoes.Count - 1);

        //return allDominoes[nextIndex];
    }


    #region Server

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        DominoPlayer player = conn.identity.GetComponent<DominoPlayer>();
        Players.Add(player);

        player.SetDisplayName($"Player {Players.Count}");
        player.SetPartyOwner(Players.Count == 1);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (!IsGameInProgress)
        {
            return;
        }

        conn.Disconnect();
    }

    public void StartGame()
    {
        if (Players.Count < 2)
        {
            return;
        }

        IsGameInProgress = true;

        ServerChangeScene("Main_Scene");
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Main_Scene"))
        {
            GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandler);
            NetworkServer.Spawn(gameOverHandlerInstance.gameObject);

            // TODO: STUCK HERE 7/22/22. How to make gameSession persisent across scenes? maybe just don't make it here?
            // TODO: how will I access the server's gameSessionInstance vs the clients'?
            //gameSessionInstance = Instantiate(gameSessionPrefab);

            // create a game session on the server
            //NetworkServer.Spawn(gameSessionInstance.gameObject);  // TODO: not sure if this is needed but the client doesn't know how run the command on it
            //gameSessionInstance.StartGame();

            dominoTracker.CreateFakeDominoes();

            // TODO: how to allow clients to see this?
            // create a server Domino in the center


            foreach (DominoPlayer player in Players)
            {
                // create a game session on each client
                //NetworkServer.Spawn(gameSessionInstance.gameObject, player.connectionToClient);

                //var entity = domino.GetComponent<DominoEntity>();
                //entity.ID = domData.ID;
                //entity.TopScore = domData.TopScore;
                //entity.BottomScore = domData.BottomScore;                
            }
        }
    }

    [Server]
    public void CreateDominoes()    // TODO: move CreateDominoes to GameSession
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject dominoInstance = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
            var dominoEntity = dominoInstance.GetComponent<DominoEntity>();
            dominoEntity.TopScore = i + 1;
            dominoEntity.BottomScore = i + 1;
            allDominoes.Add(dominoInstance);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        DominoPlayer player = conn.identity.GetComponent<DominoPlayer>();

        Players.Remove(player);

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        Players.Clear();

        IsGameInProgress = false;
    }

    #endregion


    #region Client

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    //[ClientRpc]
    //public void RpcCreateDomino(int id, int top, int bottom, DominoPlayer player)
    //{
    //    GameObject dominoInstance = Instantiate(dominoPrefab, GetStartPosition().position, Quaternion.identity);

    //    NetworkServer.Spawn(dominoInstance, player.connectionToClient);
    //}

    #endregion
}
