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

    public bool IsGameInProgress = false;
    public List<DominoPlayer> Players = new List<DominoPlayer>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    public void SetTransport(Transport t) => transport = t;

    // TODO: move this to GameManager when GameManager is ready to be used
    private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

    private List<GameObject> allDominoes = new List<GameObject>();
    private int currentDominoIndex = 0;

    public List<GameObject> GetDominoes() => allDominoes;
    public GameObject GetNextDomino()
    {
        int nextIndex = currentDominoIndex;
        currentDominoIndex = Mathf.Clamp(currentDominoIndex + 1, 0, allDominoes.Count - 1);

        return allDominoes[nextIndex];
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

            CreateDominoes();

            foreach (DominoPlayer player in Players)
            {
                // TODO: Need to loop through players before starting? Start an event?
            }
        }
    }

    [Server]
    public void CreateDominoes()    // TODO: move CreateDominoes to GameSession
    {
        for (int i = 0; i < 2; i++)
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
