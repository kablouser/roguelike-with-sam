using System.Collections;
using UnityEngine;
using System;

public class Tweener
{
    private static readonly AnimationCurve easeInOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private MonoBehaviour caller;        
    private bool isEnded;
    private Coroutine coroutine;
    private Action coroutineOnEnd;
    private Action additionalOnEnd;

    public Tweener(MonoBehaviour caller)
    {
        this.caller = caller;

        isEnded = true;
        coroutine = null;
        coroutineOnEnd = additionalOnEnd = null;
    }

    public void Start(IEnumerator startCoroutine, Action onEnd = null)
    {
        if(isEnded == false)
            End();

        additionalOnEnd = onEnd;
        coroutine = caller.StartCoroutine(startCoroutine);
    }

    public void End()
    {
        if (isEnded == false)
        {
            isEnded = true;
            caller.StopCoroutine(coroutine);
            coroutineOnEnd?.Invoke();
            additionalOnEnd?.Invoke();
        }
    }

    public IEnumerator MoveRoutine(Transform moveTarget, Vector3 startPosition, Vector3 endPosition, float duration)
    {
        isEnded = false;
        coroutineOnEnd = () => moveTarget.position = endPosition;

        Vector3 travelDirection = endPosition - startPosition;
        float progress = 0;

        do
        {
            progress += Time.deltaTime / duration;
            moveTarget.position = startPosition + travelDirection * easeInOutCurve.Evaluate(progress);
            yield return null;
        }
        while (progress < 1);

        End();
    }
}
