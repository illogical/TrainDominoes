using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class DrawDominoes : NetworkBehaviour
{
    //private DominoPlayer dominoPlayer;
    //[SerializeField] private GameSession gameSession = null;

    public void OnClick()
    {
        // this had worked before trying to run this same command from a GameSession object
        NetworkIdentity identity = NetworkClient.connection.identity;
        var dominoPlayer = identity.GetComponent<DominoPlayer>();
        //dominoPlayer.CmdAddPlayerDomino();

        // TODO: how to instead call GameSession? Keep in mind that the clients do not have authority over GameSession (it exists in the sceen prior to game start)
        dominoPlayer.CmdAddPlayerDominoes(12);

        // the button is owned by the server. Does that have anything to do with my challenges?

        // this barely worked on one client (seemingly the server)
        //var session = FindObjectOfType<GameSession>();
        //session.CmdDealDomino();

        // this works only when each player has a session. That also means there is not a single source of truth for the full deck.
        //NetworkIdentity identity = NetworkClient.connection.identity;
        //var dominoPlayer = identity.GetComponent<DominoPlayer>();
        //dominoPlayer.Session.CmdDealDomino();

        //if (!isServer)
        //{
        //    // TODO: how to get the client's game session

        //    // TODO: WHY IS GAMESESSION ALWAYS NULL HERE FOR THE CLIENT?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!?!
        //    //NetworkIdentity identity = NetworkClient.connection.identity;
        //    //var session = identity.GetComponent<GameSession>();
        //    //session.CmdDealDomino();


        //    //dominoPlayer = identity.GetComponent<DominoPlayer>();
        //    //GameSession gameSession = dominoPlayer.GetGamesSession();
        //    //var gameSession = conn.identity.GetComponent<DominoPlayer>().GetGameSession();
        //    //dominoPlayer.GetGamesSession().CmdDealDomino();
        //    //gameSession.CmdDealDomino();

        //    NetworkIdentity identity = NetworkClient.connection.identity;
        //    var dominoPlayer = identity.GetComponent<DominoPlayer>();
        //    var session = dominoPlayer.GetGamesSession();
        //    session.CmdDealDomino();
        //}
        //else
        //{
        //    // this worked!
        //    GameSession gameSession = ((MyNetworkManager)NetworkManager.singleton).GetGameSession();
        //    gameSession.Test();
        //}


    }
}
