using UnityEngine;
using System;

public class Mover : MonoBehaviour
{
    public float moveTime = 0.7f;
    [SerializeField]
    [ContextMenuItem("Set To Transform Position","SetCurrentToTransform")]
    private Vector3Int currentPosition;

    private WorldTilemap world;
    private Tweener moveTweener;

    private void Awake()
    {
        world = WorldTilemap.Current;
        world.AddForeground(currentPosition, true);
        moveTweener = new Tweener(this);
    }

    private void SetCurrentToTransform()
    {
        Vector3 transformPosition = transform.position;
        currentPosition.x = Mathf.RoundToInt(transformPosition.x);
        currentPosition.y = Mathf.RoundToInt(transformPosition.y);
        transform.position = currentPosition;

        //UnityEditor namespace is not used when building the game
#if UNITY_EDITOR
        //if this script is on a prefab, the changes made here are ignored
        //so we need to record modifications on this
        UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
#endif
    }

    public bool Move(Vector3Int direction, Action onEnd = null)
    {
        if (world.IsWalkable(currentPosition + direction) == false)
            return false;

        Action onMoveEnd;
        Vector3Int previousPosition;

        previousPosition = currentPosition;
        onMoveEnd = () =>
        {
            world.RemoveForeground(previousPosition, true);
            onEnd?.Invoke();
        };

        currentPosition += direction;
        world.AddForeground(currentPosition, true);

        moveTweener.Start(
            moveTweener.MoveRoutine(transform, previousPosition, currentPosition, moveTime),
            onMoveEnd);

        return true;
    }
}
