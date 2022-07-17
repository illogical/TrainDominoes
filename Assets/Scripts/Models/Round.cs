using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class Round
    {
        public Dictionary<int, int> Scores { get; set; }  // playerId to score

        public Round(Dictionary<int, int> scores)
        {
            Scores = scores;
        }
    }
}