using UnityEngine;

public class ControlWindow : MonoBehaviour
{
    public bool isDefaultWindow;

    private static ControlWindow currentWindow;
    private static ControlWindow defaultWindow;

    public virtual void Awake()
    {
        if (isDefaultWindow)
            defaultWindow = this;
    }

    public virtual void OnEnable()
    {
        if (currentWindow == this)
            return;

        if (currentWindow != null)
            currentWindow.gameObject.SetActive(false);

        currentWindow = this;
    }

    public virtual void OnDisable()
    {
        if (this == defaultWindow || defaultWindow == null)
            return;

        if (defaultWindow.gameObject.activeSelf == false)
            defaultWindow.gameObject.SetActive(true);
    }

    public virtual KeyCode GetActivationKey => KeyCode.None;
    public virtual void MoveInput(Vector2Int direction) { }
}
