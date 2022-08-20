using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class PlayerDominoes
    {
        public Dictionary<int, List<int>> Dominoes { get; set; }    // List of domino IDs for each player's NetId

        public PlayerDominoes()
        {
            Dominoes = new Dictionary<int, List<int>>();
        }

        public void AddDomino(int netId, int dominoId)
        {
            if(!Dominoes.ContainsKey(netId))
            {
                Dominoes.Add(netId, new List<int>());
            }

            Dominoes[netId].Add(dominoId);
        }

        public void AddDominoes(int netId, List<int> dominoIds)
        {
            if (!Dominoes.ContainsKey(netId))
            {
                Dominoes.Add(netId, new List<int>());
            }

            Dominoes[netId].AddRange(dominoIds);
        }

    }
}
