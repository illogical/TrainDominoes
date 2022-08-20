using Assets.Scripts.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private DominoEntity Domino;
    [Range(0.1f, 0.5f)]
    [SerializeField] private float DistanceFromCamera;
    [Range(0f, 0.1f)]
    [SerializeField] private float DistanceFromCenter;





    // TODO: bare basic POC:
    /*
     * *Create 3 cards. Give each of 2 players 1 cards. Check that all cards are unique after spwaning into the game.
     * *Use MultiplayerRTS as examples of how to use Mirror. Server, Client, SyncVar attributes. Checking for authority vs client vs host.
     * * * Maybe steal the lobby GUI I created and scripts
     */
}
