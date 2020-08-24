using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tweener
{
    public static Tweener GetCurrentMain => mainQueue.Count == 0 ? null : mainQueue.Peek();

    private static readonly Queue<Tweener> mainQueue = new Queue<Tweener>();
    private static readonly AnimationCurve easeInOutCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private MonoBehaviour caller;
    private IEnumerator enumerator;
    private Coroutine coroutine;

    private Action coroutineOnEnd;
    private Action additionalOnEnd;

    private bool isEnded;

    public static void EnqueueMain(Tweener tweener)
    {
        mainQueue.Enqueue(tweener);
        if (mainQueue.Count == 1)
            //start execution chain
            tweener.Start();
    }

    public Tweener(MonoBehaviour caller)
    {
        this.caller = caller;
        enumerator = null;
        coroutine = null;
        coroutineOnEnd = additionalOnEnd = null;

        isEnded = true;
    }

    public Tweener(MonoBehaviour caller, IEnumerator enumerator, Action onEnd = null)
    {
        this.caller = caller;
        this.enumerator = enumerator;
        coroutine = null;
        coroutineOnEnd = null;
        additionalOnEnd = onEnd;

        isEnded = true;
    }

    public void SetEnumerator(IEnumerator enumerator) => this.enumerator = enumerator;
    public void SetOnEnd(Action onEnd) => additionalOnEnd = onEnd;

    public void Start()
    {
        if(isEnded == false)
            End();

        coroutineOnEnd = null;
        coroutine = caller.StartCoroutine(RoutineWrapper(enumerator));
    }

    public void End()
    {
        if (isEnded == false)
        {
            isEnded = true;

            if (GetCurrentMain == this)
            {
                //execute next in queue
                mainQueue.Dequeue();
                if (0 < mainQueue.Count)
                    mainQueue.Peek().Start();
            }
            
            if (coroutine != null)
                caller.StopCoroutine(coroutine);
            coroutineOnEnd?.Invoke();
            additionalOnEnd?.Invoke();
        }
    }

    public IEnumerator RoutineWrapper(IEnumerator actualRoutine)
    {
        isEnded = false;
        yield return actualRoutine;
        End();
    }

    public IEnumerator MoveRoutine(Transform moveTarget, Vector3 startPosition, Vector3 endPosition, float duration)
    {
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
    }
}
