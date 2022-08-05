using Assets.Scripts.Interfaces;
using Mirror;
using TMPro;

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

        public void OnTopScoreChanged(int oldValue, int newValue)
        {
            //if (!hasAuthority) { return; }

            var labels = gameObject.GetComponentsInChildren<TextMeshPro>();
            if (Flipped)
            {
                labels[2].SetText(newValue.ToString());
                return;
            }

            labels[1].SetText(newValue.ToString());
        }

        public void OnBottomScoreChanged(int oldValue, int newValue)
        {
            //if (!hasAuthority) { return; }

            var labels = gameObject.GetComponentsInChildren<TextMeshPro>();
            if (Flipped)
            {
                labels[1].SetText(newValue.ToString());
                return;
            }

            labels[2].SetText(newValue.ToString());
        }

        public void OnFlippedChanged(bool oldValue, bool newValue)
        {
            var labels = gameObject.GetComponentsInChildren<TextMeshPro>();

            if (newValue)
            {
                labels[1].SetText(BottomScore.ToString());
                labels[2].SetText(TopScore.ToString());
                return;
            }

            labels[1].SetText(TopScore.ToString());
            labels[2].SetText(BottomScore.ToString());
        }

        public void SwapAuthority()
        {

        }
    }
}