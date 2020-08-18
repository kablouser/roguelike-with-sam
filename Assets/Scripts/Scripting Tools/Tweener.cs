using System.Collections;
using UnityEngine;

public static class Tweener
{
    private static readonly AnimationCurve easeInOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    /// <summary>
    /// IS BROKEN
    /// </summary>
    public static IEnumerator Move(Transform moveTarget, Vector3 endPosition, float duration)
    {
        Vector3 startPosition = moveTarget.position;
        Vector3 travelDirection = endPosition - startPosition;
        float progress = 0;

        do
        {
            progress += Time.deltaTime / duration;//easeInOutCurve.Evaluate((Time.time - startTime) / duration);
            Debug.Log(Time.deltaTime);
            moveTarget.position = startPosition + travelDirection * progress;
            //WHY IS THIS WAITING FOR 0.3 SECONDS ON THE SECOND YIELD?!
            //WHY !?!?!?!?!?!?!!?!?!?!?
            yield return null;
        }
        while (progress < 1);

        moveTarget.position = endPosition;
    }
}
