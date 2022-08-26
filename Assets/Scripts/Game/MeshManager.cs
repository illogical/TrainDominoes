// TODO: MeshManager eventually needs to track the existing meshes along with the ability to make a mesh for each domino
using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class MeshManager : MonoBehaviour
    {
        public GameObject PlayerDominoPrefab = null;
        public GameObject TableDominoPrefab = null;
        
        private Dictionary<int, GameObject> dominoObjects = new Dictionary<int, GameObject>();   // TODO: now both clients know about each other's dominoes. Feels unsure.
        private Quaternion dominoRotation = Quaternion.Euler(new Vector3(-90, 0, 180));

        private GameObject engineDomino;

        public GameObject GetDominoMeshById(int id)
        {
            return dominoObjects[id];
        }

        public GameObject GetPlayerDomino(GameObject prefab, DominoInfo info, Vector3 position)
        {
            engineDomino = CreateDominoFromInfo(prefab, info, position, PurposeType.Player);
            return engineDomino;
        }

        public GameObject GetEngineDomino() => engineDomino;

        public GameObject CreateEngineDomino(GameObject prefab, DominoInfo info, Vector3 position)
        {
            engineDomino = CreateDominoFromInfo(prefab, info, position, PurposeType.Engine);
            return engineDomino;
        }

        private GameObject CreateDominoFromInfo(GameObject prefab, DominoInfo info, Vector3 position, PurposeType purpose) 
        {
            var newDomino = Instantiate(prefab, position, dominoRotation);
            newDomino.name = info.ID.ToString();

            var dom = newDomino.GetComponent<DominoEntity>();
            dom.ID = info.ID;
            dom.TopScore = info.TopScore;
            dom.BottomScore = info.BottomScore;
            dom.Purpose = purpose;

            dominoObjects.Add(info.ID, newDomino);

            return newDomino;
        }
    }
}