using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class DrawDominoes : NetworkBehaviour
{
    private DominoPlayer dominoPlayer;

    public void OnClick()
    {
        NetworkIdentity identity = NetworkClient.connection.identity;
        dominoPlayer = identity.GetComponent<DominoPlayer>();
        dominoPlayer.CmdDealDomino();
    }
}
