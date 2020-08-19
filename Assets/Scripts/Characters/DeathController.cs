using UnityEngine;

/// <summary>
/// when the character dies, this controls the animation and destruction of the object
/// </summary>
public class DeathController : MonoBehaviour
{
    public GameObject characterSprite;
    public GameObject deathEffect;    

    public void OnDeath()
    {
        characterSprite.SetActive(false);
        Instantiate(deathEffect, characterSprite.transform.position, Quaternion.identity);
    }

    public void OnCleanup()
    {
        Destroy(gameObject);
    }
}
