using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject Domino;
    [SerializeField] private Player Player;
    [SerializeField] private Camera MainCamera;
    [Range(0.1f, 0.5f)]
    [SerializeField] private float DistanceFromCamera;

    private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));
    private Camera playerCamera;
    private Vector3 playerDominoCenter;
    private List<DominoEntity> allDominoes;

    void Start()
    {
        // create domino
        // TODO: create Domino for each player

        playerCamera = MainCamera;
        playerDominoCenter = GetPlayerCenter();

        // TODO: create 3 dominoes- one for each player and a third that only the host knows about
        Instantiate(Domino, playerDominoCenter, playerCamera.transform.rotation * dominoRotation);
    }

    void Update()
    {

    }

    Vector3 GetPlayerCenter()
    {
        // TODO: will this direction differ per player if all players are looking at one another? Might need a switch statement that returns a Vector3 pointing in the direction the player is pointed
        return playerCamera.transform.position + new Vector3(0, 0, DistanceFromCamera);
    }

    // TODO: bare basic POC:
    /*
     * *Create 3 cards. Give each of 2 players 1 cards. Check that all cards are unique after spwaning into the game.
     * *Use MultiplayerRTS as examples of how to use Mirror. Server, Client, SyncVar attributes. Checking for authority vs client vs host.
     * * * Maybe steal the lobby GUI I created and scripts
     */
}
