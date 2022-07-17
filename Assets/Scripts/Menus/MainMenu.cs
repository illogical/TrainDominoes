using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        //if (useSteam)
        //{
        //    SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, maxPlayers);
        //}

        NetworkManager.singleton.StartHost();
    }
}
