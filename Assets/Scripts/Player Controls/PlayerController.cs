using System;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public const string
        Horizontal = "Horizontal",
        Vertical = "Vertical",
        Jump = "Jump";

    public PlayerComponents character;
    public float repeatDelay = 0.2f;
    [Range(10, 30)]
    public float inputsPerSecond = 10f;

    public float speedupTime = 1.5f;

    public float diagonalSwitchWait = 0.2f;

    private TurnManager turnManager;
    private bool isLocked;
    private Vector2Int moveInput;
    private float nextMoveRepeat;

    private float diagonalSwitchLimit;
    private Vector2Int orthogonalInput;

    public override void Awake()
    {
        base.Awake();
        if (this == Current)
        {
            turnManager = TurnManager.Current;
            TurnManager.Current.RegisterPlayer(character);
        }
    }

    private void Update()
    {
        ProcessSpeedup(Input.GetButton(Jump));

        Vector2Int previousInput = moveInput;
        moveInput = new Vector2Int(
            Mathf.RoundToInt(Input.GetAxisRaw(Horizontal)),
            Mathf.RoundToInt(Input.GetAxisRaw(Vertical)));

        if (ProcessMove(previousInput))
            MoveOrAttack(moveInput);
    }

    public void UnlockControl()
    {
        if(character.characterSheet.IsAlive)
            isLocked = false;
    }

    /// <summary>
    /// Handles hold down time mechanic.
    /// Will only return true if the button has just been pressed,
    /// or has been held down for long enough.
    /// And the rate of inputs allowed each second is capped.
    /// </summary>
    private bool ProcessMove(Vector2Int previous)
    {
        int result = ProcessInput(moveInput, previous, ref nextMoveRepeat);
        if (result == 0)
        {
            //don't allow diagonal switch
            diagonalSwitchLimit = 0;
        }
        else if(result == 3)
        {
            //input down

            if (moveInput.x == 0 || moveInput.y == 0)
            {
                //moveInput is orthogonal
                if (isLocked == false)
                {
                    //strictly non-diagonal move
                    diagonalSwitchLimit = Time.time + diagonalSwitchWait;
                    orthogonalInput = moveInput;
                }
            }
            //else moveInput is diagonal
            //make sure moveInput equals orthogonalInput in 1 axes
            else if (moveInput.x == orthogonalInput.x || moveInput.y == orthogonalInput.y)
            {
                //check time limit
                if ((Time.time < diagonalSwitchLimit))
                    //undo current move
                    if (character.mover.UndoMove() || character.combatant.UndoAttack())
                        //but controls will still be locked
                        UnlockControl();
            }
            //else diagonal moveInput is going in a different direction to orthogonalInput
            else
                //don't allow diagonal switch
                diagonalSwitchLimit = 0;
        }

        return 1 < result;
    }

    private void MoveOrAttack(Vector2Int direction)
    {
        if (isLocked) return;

        if (character.mover.Move(new Vector3Int(direction.x, direction.y, 0), turnManager.EndPlayerTurn) ||
            character.combatant.Attack(direction, turnManager.EndPlayerTurn))
            isLocked = true;
    }

    private void ProcessSpeedup(bool speedupButton)
    {
        if (speedupButton) Time.timeScale = speedupTime;
        else Time.timeScale = 1.0f;
    }

    private int ProcessInput<T>(T current, T previous, ref float nextRepeat)
    {
        if (current.Equals(default(T)))
        {
            //no input
            return 0;
        }
        else if (current.Equals(previous))
        {
            //held input
            if (nextRepeat < Time.time)
            {
                nextRepeat = Time.time + 1 / inputsPerSecond;
                return 2;
            }
            else
            {
                return 1;
            }
        }
        else
        {
            //input down
            nextRepeat = Time.time + repeatDelay;
            return 3;
        }
    }
}
