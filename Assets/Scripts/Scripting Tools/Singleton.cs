using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Current
    {
        get
        {
            if (current == null)
            {
                current = FindObjectOfType<T>();
                if (current == null)
                    Debug.LogError("Couldn't find singleton of type " + typeof(T));
            }
            return current;
        }
    }
    private static T current;

    public virtual void Awake()
    {
        if(current == null)
            current = this as T;
        else if(current != this)
            OnCurrentExists();
    }

    protected virtual void OnCurrentExists()
    {
        Debug.LogWarning("Singleton ("+GetType()+") has multiple instances, destroying this.", this);
        Destroy(this);
    }
}
