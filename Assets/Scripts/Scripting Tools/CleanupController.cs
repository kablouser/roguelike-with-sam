using UnityEngine;

/// <summary>
/// Prevents errors when the scene is unloaded/destroyed
/// </summary>
public class CleanupController : Singleton<CleanupController>
{
    public void OnUnloadScene() =>
        OnApplicationQuit();

    private void OnApplicationQuit()
    {
        MonoBehaviour[] scripts = FindObjectsOfType<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
            script.enabled = false;
    }
}
