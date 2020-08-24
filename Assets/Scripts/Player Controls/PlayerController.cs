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

    private TurnManager turnManager;
    private bool isLocked;
    private Vector2Int moveInput;
    private float nextMoveRepeat;
    private bool diagonalMoveToggle;

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

        if (ProcessMove(previousInput, out Vector2Int processed))
            MoveOrAttack(processed);
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
    private bool ProcessMove(Vector2Int previous, out Vector2Int processed)
    {
        int result = ProcessInput(moveInput, previous, ref nextMoveRepeat);

        processed = moveInput;

        if (result == 1)
        {
            //detect if moving diagonal
            if (moveInput.x != 0 && moveInput.y != 0)
            {
                //alternate between zeroing x and y
                if (diagonalMoveToggle)
                    processed = new Vector2Int(moveInput.x, 0);
                else
                    processed = new Vector2Int(0, moveInput.y);
            }
        }
        else if (result == 2)
        {
            diagonalMoveToggle = false;
            if (moveInput.x != 0)
                processed = new Vector2Int(moveInput.x, 0);
            else
                processed = new Vector2Int(0, moveInput.y);
        }

        return 0 < result;
    }

    private void MoveOrAttack(Vector2Int direction)
    {
        if (isLocked) return;

        diagonalMoveToggle = !diagonalMoveToggle;
        if (character.mover.Move(new Vector3Int(direction.x, direction.y, 0), turnManager.EndPlayerTurn))
        {
            isLocked = true;
        }
        else if (character.combatant.Attack(direction, turnManager.EndPlayerTurn))
        {
            isLocked = true;
        }
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
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            //input down
            nextRepeat = Time.time + repeatDelay;
            return 2;
        }
    }
}
