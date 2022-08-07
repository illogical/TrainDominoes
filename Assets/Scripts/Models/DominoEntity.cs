using Assets.Scripts.Interfaces;
using Mirror;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class DominoEntity : NetworkBehaviour
    {
        public int ID;
        [SyncVar(hook = nameof(OnTopScoreChanged))]
        public int TopScore;
        [SyncVar(hook = nameof(OnBottomScoreChanged))]
        public int BottomScore;
        [SyncVar(hook = nameof(OnFlippedChanged))]
        public bool Flipped;

        [SerializeField] private TextMeshPro topText;
        [SerializeField] private TextMeshPro bottomText;

        public void OnValidate()
        {
            // when initialized to 0, the hook does not fire if the value is set to 0
            TopScore = -1;
            BottomScore = -1;
        }

        public void OnTopScoreChanged(int oldValue, int newValue)
        {
            //if (!hasAuthority) { return; }
            if (Flipped)
            {
                bottomText.SetText(newValue.ToString());
                return;
            }

            topText.SetText(newValue.ToString());
        }

        public void OnBottomScoreChanged(int oldValue, int newValue)
        {
            //if (!hasAuthority) { return; }

            if (Flipped)
            {
                topText.SetText(newValue.ToString());
                return;
            }

            bottomText.SetText(newValue.ToString());
        }

        public void OnFlippedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                topText.SetText(BottomScore.ToString());
                bottomText.SetText(TopScore.ToString());
                return;
            }

            topText.SetText(TopScore.ToString());
            bottomText.SetText(BottomScore.ToString());
        }

        public void SwapAuthority()
        {

        }
    }
}