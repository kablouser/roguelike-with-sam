using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float delay;
    private void Awake()
    {
        Destroy(gameObject, delay);
    }
}
