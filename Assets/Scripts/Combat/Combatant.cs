using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// uses character sheet to do combat with
/// </summary>
public class Combatant : MonoBehaviour
{
    public CharacterComponents character;

    public GameObject characterSprite;
    public GameObject slashSprite;
    public GameObject deathEffect;

    public CurveScriptable attackAnimation;
    public CurveScriptable onHitAnimation;
    public CurveScriptable slashAnimation;
    public CurveScriptable deathAnimation;    

    private WorldTilemap world;
    private Tweener mainTweener;
    private Tweener slashTweener;

    private Vector3 attackStart;
    private bool damageDealt;

    private void Awake()
    {
        world = WorldTilemap.Current;
        mainTweener = new Tweener(this);
        slashTweener = new Tweener(this);
    }

    public bool Attack(Vector2Int direction, System.Action onEnd = null)
    {
        if (1 < Mathf.Abs(direction.x) || 1 < Mathf.Abs(direction.y))
        {
            Debug.LogWarning("Direction is too large! Stop trying to cheat.", this);
            return false;
        }
        if (direction == Vector2Int.zero)
        {
            Debug.LogWarning("You shouldn't be attacking in zero direction.", this);
            return false;
        }

        Vector3Int attackPosition = character.mover.GetPosition + (Vector3Int) direction;
        var overlays = world.GetOverlays(attackPosition);
        if (overlays == null) return false;

        foreach (var overlay in overlays)
            if(overlay is CharacterComponents other && other.characterSheet.IsAlive)
            {
                mainTweener.SetEnumerator(AttackAnimation(other.mover.GetPosition - character.mover.GetPosition, other));
                mainTweener.SetOnEnd(() =>
                {
                    //damage is dealt during the animation
                    onEnd?.Invoke();
                    transform.position = attackStart;
                });

                Tweener.EnqueueMain(mainTweener);
                return true;
            }

        return false;
    }

    public bool UndoAttack()
    {
        if (mainTweener.IsEnded == false && damageDealt == false)
        {
            mainTweener.End(true);
            transform.position = character.mover.GetPosition;
            return true;
        }
        else return false;
    }

    private void PlayHitAnimation(out Tweener hitTweener)
    {
        slashSprite.SetActive(true);
        slashTweener.SetEnumerator(slashTweener.ScaleWithCurve(slashSprite.transform, slashAnimation.curve));
        slashTweener.SetOnEnd(() => slashSprite.SetActive(false));
        slashTweener.Start();

        if (character.characterSheet.IsAlive)
        {
            //play normal on hit animation
            mainTweener.SetEnumerator(mainTweener.ScaleWithCurve(characterSprite.transform, onHitAnimation.curve));
            mainTweener.SetOnEnd(() => slashTweener.End());
        }
        else
        {
            //play death animation
            Instantiate(deathEffect, characterSprite.transform.position, Quaternion.identity);
            mainTweener.SetEnumerator(mainTweener.ScaleWithCurve(characterSprite.transform, deathAnimation.curve));
            mainTweener.SetOnEnd(() =>
            {
                slashTweener.End();
                Destroy(gameObject);
            });
        }
        mainTweener.Start();
        hitTweener = mainTweener;
    }

    private IEnumerator AttackAnimation(Vector3 direction, CharacterComponents other)
    {
        Tweener hitTweener = null;

        attackStart = transform.position;

        var getCurve = attackAnimation.curve;
        float finalTime = getCurve.keys[getCurve.length - 1].time;
        float currentTime = 0;
        
        bool impacted = false;
        damageDealt = false;
        do
        {
            float curveValue = getCurve.Evaluate(currentTime);
            transform.position = attackStart + curveValue * direction;

            if(impacted == false && .8f <= curveValue)
            {
                impacted = true;

                //decrease other health
                other.characterSheet.health.Decrease(
                    //with my attack
                    character.characterSheet.attributes.attack.GetTotal);
                damageDealt = true;
                //play onhit animation
                other.combatant.PlayHitAnimation(out hitTweener);
            }

            yield return null;
            currentTime += Time.deltaTime;
        }
        while (currentTime < finalTime);

        yield return new WaitUntil(() => hitTweener.IsEnded);
    }
}
