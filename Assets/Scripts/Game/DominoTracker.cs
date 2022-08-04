using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class DominoTracker
    {
        private Dictionary<int, DominoInfo> dominoData = new Dictionary<int, DominoInfo>();
        private List<int> availableDominoes = new List<int>();

        private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

        private Vector3 playerTopCenter = new Vector3(0, 1.07f, -9.87f);
        private Vector3 playerBottomCenter = new Vector3(0, 0.93f, -9.87f);

        public Quaternion GetDominoRotation() => dominoRotation;

        public void CreateFakeDominoes()
        {
            for (int i = 0; i < 10; i++)
            {      
                var dominoEntity = new DominoInfo()
                {
                    ID = i + 1,
                    TopScore = i + 1,
                    BottomScore = i + 1
                };

                dominoData.Add(i, dominoEntity);
                availableDominoes.Add(i);
            }
        }

        public DominoInfo GetNextDomino()
        {
            if (availableDominoes.Count == 0)
            {
                Debug.LogError("Server is out of dominoes");
            }
            var nextDominoEntity = dominoData[availableDominoes[0]];
            availableDominoes.RemoveAt(0);


            return nextDominoEntity;
        }
    }
}
