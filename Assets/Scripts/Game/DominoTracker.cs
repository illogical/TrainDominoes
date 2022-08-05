using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class DominoTracker
    {
        public Dictionary<int, DominoInfo> AllDominoes = new Dictionary<int, DominoInfo>();
        private List<int> availableDominoes = new List<int>();

        private List<int> engineIndices = new List<int>();

        private const int maxDots = 12;

        /// <summary>
        /// Creates 91 dominoes based upon 12 point max train domino set
        /// </summary>
        public void CreateDominoSet()
        {
            var index = 0;
            for (int i = 0; i < maxDots + 1; i++)
            {
                for (int j = i; j < maxDots + 1; j++)
                {
                    AllDominoes.Add(index, createDomino(i, j, index));
                    availableDominoes.Add(index);

                    if (i == j)
                    {
                        // track index for each double
                        engineIndices.Add(index);
                    }

                    index++;
                }
            }

            engineIndices.Reverse();
        }

        //public DominoInfo GetNextDomino()
        //{
        //    if (availableDominoes.Count == 0)
        //    {
        //        Debug.LogError("Server is out of dominoes");
        //    }
        //    var nextDominoEntity = AllDominoes[availableDominoes[0]];
        //    availableDominoes.RemoveAt(0);


        //    return nextDominoEntity;
        //}

        public DominoInfo GetDominoFromBonePile()
        {
            int randomDominoIndex = UnityEngine.Random.Range(0, availableDominoes.Count);
            int dominoID = availableDominoes[randomDominoIndex];
            var domino = AllDominoes[dominoID];

            availableDominoes.RemoveAt(randomDominoIndex);
            return domino;
        }

        private DominoInfo createDomino(int topScore, int bottomScore, int index)
        {
            return new DominoInfo()
            {
                TopScore = topScore,
                BottomScore = bottomScore,
                ID = index
            };
        }
    }
}
