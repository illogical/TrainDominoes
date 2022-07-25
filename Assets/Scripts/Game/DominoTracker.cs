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
        private Dictionary<int, DominoEntity> dominoData = new Dictionary<int, DominoEntity>();
        private List<int> availableDominoes = new List<int>();

        private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

        private Vector3 playerTopCenter = new Vector3(0, 1.07f, -9.87f);
        private Vector3 playerBottomCenter = new Vector3(0, 0.93f, -9.87f);

        public Quaternion GetDominoRotation() => dominoRotation;

        public void CreateFakeDominoes()
        {
            for (int i = 0; i < 10; i++)
            {
                //GameObject dominoInstance = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
                //dominoInstance.GetComponent<DominoEntity>();        
                var dominoEntity = new DominoEntity()
                {
                    ID = i + 1,
                    TopScore = i + 1,
                    BottomScore = i + 1
                };

                dominoData.Add(i, dominoEntity);
                availableDominoes.Add(i);
            }
        }

        public DominoEntity GetNextDomino()
        {
            if (availableDominoes.Count == 0)
            {
                Debug.LogError("Server is out of dominoes");
            }
            //int nextIndex = currentDominoIndex;
            //currentDominoIndex = Mathf.Clamp(currentDominoIndex + 1, 0, dominoData.Count - 1);

            var nextDominoEntity = dominoData[availableDominoes[0]];
            availableDominoes.RemoveAt(0);

            Debug.Log("GetNextDomino() executed");

            //GameObject dominoInstance = Instantiate(dominoPrefab, Vector3.zero, dominoRotation);
            //var dom = dominoInstance.GetComponent<DominoEntity>();
            //dom.ID = nextDominoEntity.ID;
            //dom.TopScore = nextDominoEntity.ID;
            //dom.BottomScore = nextDominoEntity.ID;



            return nextDominoEntity;
        }
    }
}
