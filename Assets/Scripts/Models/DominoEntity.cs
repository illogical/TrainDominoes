using Mirror;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class DominoEntity : NetworkBehaviour
    {
        public int ID;
        [SyncVar]
        public int TopScore;
        [SyncVar]
        public int BottomScore;
        [SyncVar]
        public bool Flipped;

        [ClientCallback]
        public void UpdateDominoLabels()
        {
            var labels = gameObject.GetComponentsInChildren<TextMeshPro>();

            if (Flipped)
            {
                labels[1].SetText(BottomScore.ToString());
                labels[2].SetText(TopScore.ToString());
                return;
            }

            labels[1].SetText(TopScore.ToString());
            labels[2].SetText(BottomScore.ToString());
        }
    }
}