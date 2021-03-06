using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blend : Enemy
{
    protected override IEnumerator TauntCoroutine()
    {
        checkOnce = true;
        canTakeDamage = false;
        ChangeAnimationState(AnimationStates.TAUNT);
        yield return new WaitForSeconds(2);
        canTakeDamage = true;
        SetState(EnemyStates.Idle);
        checkOnce = false;
    }
    public override void MakeDecision()
    {
        if (currState == EnemyStates.BlockingH || currState == EnemyStates.BlockingL || currState == EnemyStates.Idle)
            CalculateAttackChances();
    }

    void CalculateAttackChances()
    {
        int attackPicker;
        //pick attack
        attackPicker = Random.Range(0, 101);
        if (attackPicker < 40)
            SetState(EnemyStates.AttackingL);
        else if (attackPicker >= 40 && attackPicker < 80)
            SetState(EnemyStates.AttackingR);
        else if (attackPicker >= 80 && attackPicker <= 100)
            SetState(EnemyStates.Taunting);
    }
}
