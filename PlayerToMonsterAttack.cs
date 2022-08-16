using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProjectDoD;

public class PlayerToMonsterAttack : MonoBehaviour
{
    private MonsterState monsterState;
    private AIMonster AImonster;
    FiveElements m_fiveElements = null;

    bool doAttack = false;

    void Start()
    {
        monsterState = GetComponent<MonsterState>();
        AImonster = GetComponent<AIMonster>();
        m_fiveElements = new FiveElements();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag(Tags.PlayerSwordCollider))
        {
            CharacterState m_characterState = col.gameObject.GetComponentInParent<CharacterState>(true);
            GameObject m_character = col.gameObject.GetComponentInParent<CharacterState>(true).gameObject;

            if (m_character.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            {
                if (monsterState != null)
                {
                    StartCoroutine(WaitForAttackTrigger(1f, col, m_characterState, m_character));
                }
            }
        }
    }

    IEnumerator WaitForAttackTrigger(float delay, Collider col, CharacterState m_characterState, GameObject m_character)
    {
        DoAttackTrigger(col, m_characterState, m_character);
        yield return new WaitForSeconds(delay);
        doAttack = false;
    }

    void DoAttackTrigger(Collider col, CharacterState m_characterState, GameObject m_character)
    {
        if (doAttack == false)
        {
            doAttack = true;

            float damage = m_fiveElements.CharacterToMonsterDamage(m_characterState, monsterState);

            OutputMsg.Instance.SendMonsterDamage(damage, true, monsterState.m_ID);

            monsterState.SetHP_Damage(damage);
            AImonster.UpdateHPBar(monsterState.HP / monsterState.m_maxHP);
            AImonster.GetDamage();
        }
    }

}
