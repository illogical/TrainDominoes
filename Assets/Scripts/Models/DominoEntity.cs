using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class DominoEntity
    {
        public GameObject Mesh { get; set; }
        public int ID { get; set; }
        public int TopScore { get; set; }
        public int BottomScore { get; set; }
        public bool IsFlipped { get; set; }

    }
}