using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField]
    [ContextMenuItem("Set To Transform Position","SetCurrentToTransform")]
    private Vector3Int currentPosition;
    private WorldTilemap world;

    public float tweenTime = 0.7f;

    private void Awake()
    {
        world = WorldTilemap.Current;
        world.AddForeground(currentPosition);
    }

    private void OnEnable()
    {
        
    }

    private void SetCurrentToTransform()
    {
        Vector3 transformPosition = transform.position;
        currentPosition.x = Mathf.RoundToInt(transformPosition.x);
        currentPosition.y = Mathf.RoundToInt(transformPosition.y);
        transform.position = currentPosition;
    }

    //move to new position
    public virtual void Move(Vector3Int direction, bool ignorePrevious = false)
    {
        if (ignorePrevious == false)
            world.RemoveForeground(currentPosition);

        currentPosition += direction;
        world.AddForeground(currentPosition);

        transform.position += direction;
    }

    [ContextMenu("MOVE")]
    private void InspectorMove() => Move(new Vector3Int(1, 0, 0));
}
