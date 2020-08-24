using UnityEngine;
using System.Collections;

/// <summary>
/// uses character sheet to do combat with
/// </summary>
public class Combatant : MonoBehaviour
{
    public CharacterComponents character;
    public CurveScriptable attackAnimation;

    private WorldTilemap world;
    private Tweener tweener;

    private Vector3 attackStart;

    private void Awake()
    {
        world = WorldTilemap.Current;
        tweener = new Tweener(this);
    }

    public bool Attack(Vector2Int inDirection, System.Action onEnd = null)
    {
        if(inDirection == Vector2Int.zero)
        {
            Debug.LogWarning("You shouldn't be attacking in zero direction");
            return false;
        }

        //we can attack in 1 tile range
        if (1 < inDirection.sqrMagnitude) return false;

        Vector3Int attackPosition = character.mover.GetPosition + (Vector3Int) inDirection;
        var overlays = world.GetOverlays(attackPosition);
        if (overlays == null) return false;

        foreach (var overlay in overlays)
            if(overlay is CharacterComponents other && other.characterSheet.IsAlive)
            {
                tweener.SetEnumerator(AttackAnimation(other.mover.GetPosition - character.mover.GetPosition));
                tweener.SetOnEnd(() =>
                {
                    //decrease other health
                    other.characterSheet.health.Decrease(
                        //with my attack
                        character.characterSheet.attack.GetTotal);
                    onEnd?.Invoke();
                    transform.position = attackStart;
                });

                Tweener.EnqueueMain(tweener);
                return true;
            }

        return false;
    }

    private IEnumerator AttackAnimation(Vector3 direction)
    {
        attackStart = transform.position;

        var getCurve = attackAnimation.curve;
        float finalTime = getCurve.keys[getCurve.length - 1].time;
        float currentTime = 0;

        do
        {
            transform.position = attackStart + getCurve.Evaluate(currentTime) * direction;

            yield return null;
            currentTime += Time.deltaTime;
        }
        while (currentTime < finalTime);
    }
}
