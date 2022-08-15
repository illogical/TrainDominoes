using Assets.Models;
using Assets.Scripts.Helpers;
using Assets.Scripts.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{    
    public class LayoutManager : MonoBehaviour
    {
        [Range(0, 2)]
        public float BottomYOffset = 0.01f;
        public float BottomSideMargin = 0.01f;
        public float SelectionRaiseAmount = 0.02f;
        [Space]
        public AnimationCurve SelectionEase;
        public float SelectionDuration = 0.03f;
        public float FlyInStaggerDelay = 0.02f;
        [Space]
        public Camera MainCamera;
        public GameObject DominoPrefab;

        private float playerYPosition = 0;

        public void PlacePlayerDominoes(List<GameObject> playerDominoes)
        {
            PositionHelper.LayoutAcrossAndUnderScreen(playerDominoes, MainCamera, BottomSideMargin);  //place them outside of the camera's view to allow them to slide in

            var objectSize = PositionHelper.GetObjectDimensions(playerDominoes[0]);
            var positions = PositionHelper.GetLayoutAcrossScreen(objectSize, MainCamera, playerDominoes.Count, BottomSideMargin);

            playerYPosition = positions[0].y;

            for (int i = 0; i < playerDominoes.Count; i++)
            {
                var domino = playerDominoes[i];
                var mover = domino.GetComponent<Mover>();

                var staggerDelay = FlyInStaggerDelay * i;

                StartCoroutine(mover.MoveOverSeconds(positions[i], 0.5f, staggerDelay));
            }
        }

        public Vector3 GetPlayerDominoLinePosition()
        {
            var screenSize = PositionHelper.GetScreenSize(MainCamera);
            var bottomCenter = new Vector3(screenSize.x, BottomYOffset, 0);

            return bottomCenter;
        }

        public void PlaceEngine(GameObject engine, Action afterComplete = null)
        {
            var destination = GetEnginePosition(engine);

            var mover = engine.GetComponent<Mover>();
            StartCoroutine(mover.MoveOverSeconds(destination, 0.5f, 0, afterComplete));
        }

        public Vector3 GetEnginePosition(GameObject engine)
        {
            var objectSize = PositionHelper.GetObjectDimensions(engine);
            return PositionHelper.GetScreenLeftCenterPositionForObject(objectSize, MainCamera, 0);
        }

        public void SelectDomino(GameObject domino)
        {
            var destination = new Vector3(domino.transform.position.x, playerYPosition + SelectionRaiseAmount, domino.transform.position.z);

            var mover = domino.GetComponent<Mover>();
            StartCoroutine(mover.MoveOverSeconds(destination, SelectionDuration, 0));
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
