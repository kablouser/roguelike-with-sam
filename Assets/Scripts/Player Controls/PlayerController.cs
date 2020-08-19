using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public PlayerComponents character;
    [SerializeField]
    private Vector2Int moveInput;
    private TurnManager turnManager;

    private bool isLocked;

    private void Awake()
    {
        turnManager = TurnManager.Current;
        TurnManager.Current.RegisterPlayer(character);
    }

    private void Update()
    {
        Vector2Int previousInput = moveInput;
        moveInput.x = Mathf.RoundToInt(Input.GetAxisRaw("Horizontal"));
        moveInput.y = Mathf.RoundToInt(Input.GetAxisRaw("Vertical"));

        if (isLocked) return;

        if (previousInput.x != moveInput.x && moveInput.x != 0)
        {
            character.mover.Move(new Vector3Int(moveInput.x, 0, 0), turnManager.EndPlayerTurn);
        }
        else if(previousInput.y != moveInput.y && moveInput.y != 0)
        {
            character.mover.Move(new Vector3Int(0, moveInput.y, 0), turnManager.EndPlayerTurn);
        }
    }

    public void UnlockControl()
    {
        if(character.characterSheet.IsAlive)
            isLocked = false;
    }

    public void LockControl()
    {
        isLocked = true;
    }
}
