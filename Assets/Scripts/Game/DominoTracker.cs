using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class DominoTracker
    {
        public int? SelectedDomino { get; private set; }
        public Dictionary<int, DominoInfo> AllDominoes = new Dictionary<int, DominoInfo>();
        // TODO: begin tracking tracks
        public Station Station { get; private set; }

        private PlayerDominoes playerDominoes = new PlayerDominoes();
        private List<int> availableDominoes = new List<int>();
        private List<int> engineIndices = new List<int>();
        private int engineIndex = -1;

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

        public DominoInfo GetDominoByID(int dominoId) => AllDominoes[dominoId];
        public int GetEngineDominoID() => engineIndices[engineIndex];
        public bool IsPlayerDomino(int netId, int dominoId) => playerDominoes.Dominoes[netId].Contains(dominoId);   // TODO: this doesn't work on the client-only
        public bool IsEngine(int dominoId) => engineIndices[engineIndex] == dominoId;


        public void SetSelectedDomino(int dominoId)
        {
            SelectedDomino = dominoId;
        }

        public void AddPlayerDomino(int netId, int dominoId)
        {
            playerDominoes.AddDomino(netId, dominoId);
        }

        public void AddPlayerDominoes(int netId, List<int> dominoId)
        {
            playerDominoes.AddDominoes(netId, dominoId);
        }

        public DominoInfo GetDominoFromBonePile()
        {
            int randomDominoIndex = UnityEngine.Random.Range(0, availableDominoes.Count); // TODO: could do a single shuffle instead
            int dominoID = availableDominoes[randomDominoIndex];
            var domino = AllDominoes[dominoID];

            availableDominoes.RemoveAt(randomDominoIndex);
            return domino;
        }

        public DominoInfo GetNextEngineAndCreateStation()
        {
            var engine = AllDominoes[engineIndices[++engineIndex]];
            availableDominoes.Remove(engine.ID);    // no longer available to pick up

            Station = new Station(engine);

            return engine;
        }

        //public Track AddToNewTrack(int dominoId) => station.AddTrack(dominoId);

        //public Track AddToTrack(int dominoId, int trackIndex) => station.AddDominoToTrack(dominoId, trackIndex);


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
