using Assets.Models;
using Assets.Scripts.Helpers;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    // TODO: might want to pass in existing GameObject empties in the scene and use math to place them. Engine to the side center. Player's are x distance from the bottom. Train tracks stay centered on the left.
    // TODO: GameObject empties for player positions maybe need to be NetworkBehaviour objects so that each player can move their's independently
            // unless it plans to stay centered on the X position which I think is how it currently works
    
    public class LayoutManager : MonoBehaviour
    {
        [Range(0, 2)]
        public float BottomYOffset;
        public float BottomSideMargin = 0.01f;
        [Space]
        public AnimationCurve SelectionEase;
        public float SelectionDuration = 0.3f;
        [Space]
        public Camera MainCamera;
        public GameObject DominoPrefab;

        private BottomGroup bottomGroup;

        public void PlacePlayerDominoes(List<GameObject> playerDominoes)
        {
            PositionHelper.LayoutAcrossAndUnderScreen(playerDominoes, MainCamera, BottomSideMargin);  //place them outside of the camera's view to allow them to slide in

            var objectSize = PositionHelper.GetObjectDimensions(playerDominoes[0]);
            var positions = PositionHelper.GetLayoutAcrossScreen(objectSize, MainCamera, playerDominoes.Count, BottomSideMargin);

            for (int i = 0; i < playerDominoes.Count; i++)
            {
                var domino = playerDominoes[i];
                var mover = domino.GetComponent<Mover>();

                //mover.DealDomino(positions[i], LayoutManager.MainCamera, LayoutManager.BottomSideMargin);
                //mover.DealDomino(positions[i]);
                var staggerDelay = 0.02f * i;

                StartCoroutine(mover.MoveOverSeconds(positions[i], 0.5f, staggerDelay));
            }
        }

        /// <summary>
        /// Resets the test scene. Destroys then recreates all of the objects.
        /// </summary>
        /// <param name="original">Object to be duplicated</param>
        /// <param name="count">Number of duplicates to create</param>
        /// <param name="sideMargin">Distance from edge of screen</param>
        public void Reset(List<GameObject> playerDominoes, float sideMargin = 0f)
        {
            bool bottomGroupExisted = bottomGroup.PlayerObjects.Objects.Count > 0;

            if (bottomGroupExisted)
            {
                DestroyGroup(bottomGroup.PlayerObjects);
            }

            CreateBottomGroup(playerDominoes);

            if (!bottomGroupExisted)
            {
                PositionHelper.LayoutAcrossAndUnderScreen(bottomGroup.PlayerObjects.Objects, MainCamera, sideMargin);
                StartCoroutine(SlideStaggeredToYPosition(bottomGroup.PlayerObjects.Objects, bottomGroup.Empty.transform.position.y, () => bottomGroup.ParentGroupToEmpty()));
            }
            else
            {
                // TODO: make these slide to new positions and ParentGroupToEmpty when complete. Currently a bug [when moving in real-time in the AnimationEasing project]. 
                PositionHelper.LayoutAcrossScreen(bottomGroup.PlayerObjects.Objects, MainCamera, bottomGroup.Empty.transform.position.y, sideMargin);
            }
        }

        ObjectGroup<GameObject> CreateBottomGroup(List<GameObject> playerDominoes)
        {
            var newGroup = new ObjectGroup<GameObject>();

            foreach (var domino in playerDominoes)
            {
                newGroup.Add(domino.name, domino);
            }

            return newGroup;
        }

        /// <summary>
        /// Destroys all of the objects in the group and removes them from the group
        /// </summary>
        /// <param name="group">Group to clear</param>
        void DestroyGroup(ObjectGroup<GameObject> group)
        {
            var keys = group.GetKeys();
            for (int i = group.Count - 1; i >= 0; i--)
            {
                Destroy(group.GetObjectByKey(keys[i]));
                group.Remove(keys[i]);
            }
        }

        IEnumerator SlideStaggeredToYPosition(List<GameObject> gameObjects, float destinationYPosition, Action afterComplete = null)
        {
            float animationDuration = 0.8f;
            float delayBeforeAnimation = 0.5f;
            float delayStagger = 0.04f;
            float totalAnimationTime = gameObjects.Count * delayStagger + delayBeforeAnimation + animationDuration;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                var currentObj = gameObjects[i];
                Vector3 pos = new Vector3(currentObj.transform.position.x, destinationYPosition, 0);
                StartCoroutine(AnimationHelper.MoveOverSeconds(currentObj.transform, pos, animationDuration, i * delayStagger + delayBeforeAnimation, SelectionEase));
            }

            yield return new WaitForSeconds(totalAnimationTime);

            if (afterComplete != null)
            {
                afterComplete();
            }
        }
    }
}
