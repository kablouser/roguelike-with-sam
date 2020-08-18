using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Current { get; private set; }

    public virtual void Awake()
    {
        if(Current == null)
            Current = this as T;
        else
            OnCurrentExists();
    }

    protected virtual void OnCurrentExists()
    {
        Debug.LogWarning("Singleton has multiple instances, destroying this.", this);
        Destroy(this);
    }
}
