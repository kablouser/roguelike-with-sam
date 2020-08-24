using UnityEngine;
using System.Collections;

/// <summary>
/// when the character dies, this controls the animation and destruction of the object
/// </summary>
public class DeathController : MonoBehaviour
{
    public GameObject characterSprite;
    public GameObject deathEffect;
    public CurveScriptable scaleAnimation;

    public void OnDeath()
    {
        Tweener.EnqueueMain(new Tweener(this, DeathAnimation(), OnCleanup));
    }

    public void OnCleanup()
    {
        Destroy(gameObject);
    }

    [ContextMenu("Shrink Animation")]
    private void InspectorAnimation()
    {
        Tweener.EnqueueMain(new Tweener(this, DeathAnimation(), () =>
        {
            characterSprite.SetActive(true);
            characterSprite.transform.localScale = Vector3.one;
        }));
    }

    private IEnumerator DeathAnimation()
    {
        Instantiate(deathEffect, characterSprite.transform.position, Quaternion.identity);

        float time = 0;
        float readValue;
        while(true)
        {
            readValue = scaleAnimation.curve.Evaluate(time);
            if (readValue <= 0.01f)
                break;
            characterSprite.transform.localScale = Vector3.one * readValue;

            yield return null;
            time += Time.deltaTime;
        }

        characterSprite.SetActive(false);
    }
}
