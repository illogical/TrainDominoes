using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;

    // TODO: use PositionHelper to move dominoes to the correct positions

    // TODO: pass in the location instead of the index
    public IEnumerator DealDomino(Vector3 destination)
    {
        yield return StartCoroutine(MoveOverSeconds(destination, 0.5f, 0));

        //var objectSize = PositionHelper.GetObjectDimensions(this.gameObject);

        //var positions = PositionHelper.GetLayoutAcrossScreen(objectSize, camera, totalDominoesInRow, sideMargin);

        //PositionHelper.LayoutAcrossAndUnderScreen(gameObjects, mainCamera, sideMargin);

        //GetCenterPositionByIndex(index, distanceFromEachOther, centerOffSet);

        //StartCoroutine(SlideStaggeredToYPosition(bottomGroup.PlayerObjects.Objects, bottomGroup.Empty.transform.position.y, () => bottomGroup.ParentGroupToEmpty()));
    }

    public IEnumerator SlideStaggeredToYPosition(List<GameObject> gameObjects, float destinationYPosition, AnimationCurve curve, Action afterComplete = null)
    {
        float animationDuration = 0.8f;
        float delayBeforeAnimation = 0.5f;
        float delayStagger = 0.04f;
        float totalAnimationTime = gameObjects.Count * delayStagger + delayBeforeAnimation + animationDuration;

        for (int i = 0; i < gameObjects.Count; i++)
        {
            var currentObj = gameObjects[i];
            Vector3 pos = new Vector3(currentObj.transform.position.x, destinationYPosition, 0);
            StartCoroutine(AnimationHelper.MoveOverSeconds(currentObj.transform, pos, animationDuration, i * delayStagger + delayBeforeAnimation, curve));
        }

        yield return new WaitForSeconds(totalAnimationTime);

        if (afterComplete != null)
        {
            afterComplete();
        }
    }

    public IEnumerator MoveOverSeconds(Vector3 endPos, float seconds, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0;
        var startPos = transform.position;
        while (elapsedTime < seconds)
        {
            transform.position = Vector3.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPos;
    }

    public IEnumerator RotateOverSeconds(Quaternion rotationAmount, float seconds, float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsedTime = 0;
        var startRotation = transform.rotation;
        while (elapsedTime < seconds)
        {
            transform.rotation = Quaternion.Lerp(startRotation, rotationAmount, animationCurve.Evaluate(elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = rotationAmount;
    }
}
