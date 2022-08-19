using Assets.Scripts.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private AnimationCurve animationCurve;

    public IEnumerator MoveOverSeconds(Vector3 endPos, float seconds, float delay, Action afterComplete = null)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay); // TODO: store in dictionary for performance's sake
        }

        float elapsedTime = 0;
        var startPos = transform.position;
        while (elapsedTime < seconds)
        {
            transform.position = Vector3.Lerp(startPos, endPos, animationCurve.Evaluate(elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        transform.position = endPos;

        if (afterComplete != null)
        {
            afterComplete();
        }
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

    public IEnumerator SlideToPosition(Transform objectTransform, Vector3 endPos, float duration, float delay, AnimationCurve curve)
    {
        if (objectTransform.position == endPos)
        {
            // skip objects that are already in place
            yield break;
        }

        yield return StartCoroutine(AnimationHelper.MoveOverSeconds(objectTransform, endPos, duration, delay, curve));
    }

    public IEnumerator SlideToPositionByIndex(int index, Vector3 destination, float animationDuration, float delayBeforeStart, float delayStagger, AnimationCurve curve)
    {
        yield return StartCoroutine(AnimationHelper.MoveOverSeconds(transform, destination, animationDuration, index * delayStagger + delayBeforeStart, curve));
    }
}
