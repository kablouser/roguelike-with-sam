using UnityEngine;

public abstract class ControlWindow : MonoBehaviour
{
    public abstract KeyCode GetActivationKey { get; }
    public abstract void MoveInput(Vector2Int direction);
}
