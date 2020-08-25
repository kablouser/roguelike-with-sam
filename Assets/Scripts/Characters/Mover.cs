using UnityEngine;
using System;

public class Mover : MonoBehaviour
{
    private static readonly float sqrt2 = Mathf.Sqrt(2);
    public Vector3Int GetPosition => currentPosition;

    public CharacterComponents character;
    public float moveTime = 0.7f;
    [SerializeField]
    [ContextMenuItem("Set To Transform Position","SetCurrentToTransform")]
    private Vector3Int currentPosition;
    private Vector3Int previousPosition;

    private WorldTilemap world;
    private Tweener moveTweener;
    /// <summary>
    /// saves the position on undo, if another move is issued on the same frame then its resumed from this position instead of currentPosition
    /// it creates a smoother transition from orthogonal to diagonal
    /// </summary>
    private Vector3 undoSavePosition;
    private float undoFrame;    

    private void Awake()
    {
        SetCurrentToTransform();

        world = WorldTilemap.Current;
        world.AddForeground(currentPosition, character);
        moveTweener = new Tweener(this);
        undoFrame = -1;
    }

    private void OnDisable()
    {
        world.RemoveForeground(currentPosition, character);
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
        if(1 < Mathf.Abs(direction.x) || 1 < Mathf.Abs(direction.y))
        {
            Debug.LogWarning("Direction is too large! Stop trying to teleport.", this);
            return false;
        }
        if(direction == Vector3Int.zero)
        {
            Debug.LogWarning("You shouldn't be moving in zero direction.", this);
            return false;
        }

        float useMoveTime = moveTime;

        if (world.IsWalkable(currentPosition + direction) == false)
        {
            //wall gliding
            if (direction.x != 0 && direction.y != 0)
            {
                //direction is diagonal
                if (world.IsWalkable(currentPosition + new Vector3Int(direction.x, 0, 0)) == false)
                    //there is a wall/obstacle to glid across
                    return Move(new Vector3Int(0, direction.y, 0), onEnd);
                else if (world.IsWalkable(currentPosition + new Vector3Int(0, direction.y, 0)) == false)
                    //there is a wall/obstacle to glid across
                    return Move(new Vector3Int(direction.x, 0, 0), onEnd);
            }

            return false;
        }
        else if(direction.x != 0 && direction.y != 0)
        {
            //direction is diagonal
            //make sure the move is not impossible
            if (world.IsWalkable(currentPosition + new Vector3Int(direction.x, 0, 0)) == false &&
                world.IsWalkable(currentPosition + new Vector3Int(0, direction.y, 0)) == false)
                return false;
            //if diagonal direction is ok, slow down the animation
            else
                useMoveTime = moveTime * sqrt2;
        }

        Action onMoveEnd;

        previousPosition = currentPosition;
        onMoveEnd = () =>
        {
            world.RemoveForeground(previousPosition, character);
            onEnd?.Invoke();
        };

        currentPosition += direction;
        world.AddForeground(currentPosition, character);

        Vector3 useStartPosition = undoFrame == Time.time ? undoSavePosition : previousPosition;
        moveTweener.SetEnumerator(moveTweener.MoveRoutine(transform, useStartPosition, currentPosition, useMoveTime));
        moveTweener.SetOnEnd(onMoveEnd);
        Tweener.EnqueueMain(moveTweener);

        return true;
    }

    public bool UndoMove()
    {
        if (moveTweener.IsEnded == false)
        {
            moveTweener.End(true);
            
            world.RemoveForeground(currentPosition, character);
            undoSavePosition = transform.position;
            transform.position = currentPosition = previousPosition;

            undoFrame = Time.time;
            //if tweener is playing, then foreground exists on the previous tile already
            return true;
        }
        else return false;
    }
}
