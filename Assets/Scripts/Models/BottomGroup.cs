using Assets.Models;
using UnityEngine;

namespace Assets.Scripts.Models
{
    public class BottomGroup
    {
        public GameObject Empty { get; private set; }       // TODO: might need to destroy this if game resets
        public ObjectGroup<GameObject> PlayerObjects { get; set; }

        public BottomGroup(GameObject bottomEmpty)
        {
            Empty = bottomEmpty;
            bottomEmpty.name = "bottom";
            PlayerObjects = new ObjectGroup<GameObject>();
        }

        public void ParentGroupToEmpty()
        {
            foreach (var obj in PlayerObjects.Objects)
            {
                obj.transform.SetParent(Empty.transform);
            }
        }
    }
}
