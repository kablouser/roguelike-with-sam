using UnityEngine;
using System.Collections;

/// <summary>
/// when the character dies, this controls the animation and destruction of the object
/// </summary>
public class DeathController : MonoBehaviour
{
    public GameObject characterSprite;
    public GameObject deathEffect;
    public AnimationCurve shrinkSpriteScale;

    private GameObject effectInstance;

    public void OnDeath()
    {
        StartCoroutine(ShrinkSprite());
        effectInstance = Instantiate(deathEffect, characterSprite.transform.position, Quaternion.identity);
    }

    public void OnCleanup()
    {
        Destroy(gameObject);
        if (effectInstance != null)
            Destroy(effectInstance);
    }

    [ContextMenu("Shrink Animation")]
    private void DoAnimation()
    {
        StartCoroutine(ShrinkSprite(() =>
        {
            characterSprite.SetActive(true);
            characterSprite.transform.localScale = Vector3.one;
        }
        ));
    }

    private IEnumerator ShrinkSprite(System.Action onFinished = null)
    {
        float time = 0;
        float readValue;
        while(true)
        {
            readValue = shrinkSpriteScale.Evaluate(time);
            if (readValue <= 0.01f)
                break;
            characterSprite.transform.localScale = Vector3.one * readValue;

            yield return null;
            time += Time.deltaTime;
        }

        characterSprite.SetActive(false);
        onFinished?.Invoke();
    }
}
