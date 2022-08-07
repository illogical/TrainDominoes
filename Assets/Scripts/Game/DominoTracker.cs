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
        private int engineIndex = 0;

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
        }

        public DominoInfo GetDominoByID(int id)
        {
            return AllDominoes[id];
        }

        public DominoInfo GetDominoFromBonePile()
        {
            int randomDominoIndex = UnityEngine.Random.Range(0, availableDominoes.Count);
            int dominoID = availableDominoes[randomDominoIndex];
            var domino = AllDominoes[dominoID];

            availableDominoes.RemoveAt(randomDominoIndex);
            return domino;
        }

        public DominoInfo GetNextEngine()
        {
            var engine = AllDominoes[engineIndices[engineIndex++]];
            availableDominoes.Remove(engine.ID);    // no longer available to pick up

            return engine;
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
