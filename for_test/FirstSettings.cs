using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSettings : MonoBehaviour
{
    // 캐릭터와 몬스터 시스템
    public CharacterState m_character;
    public MonsterState m_monster;

    void Start()
    {
        m_character = new CharacterState();
        m_monster = new MonsterState();

        // 공격력 세팅
        m_character.LV_Attack_Fire = 20;
        m_character.LV_Attack_Water = 10;
        m_character.LV_Attack_Wood = 10;
        m_character.LV_Attack_Metal = 10;
        m_character.LV_Attack_Earth = 10;

        m_monster.Power_Attack = 10;

        Debug.Log("first player setting: fire(" + m_character.LV_Attack_Fire +
            ") water(" + m_character.LV_Attack_Water +
            ") wood(" + m_character.LV_Attack_Wood +
            ") metal(" + m_character.LV_Attack_Metal +
            ") earth(" + m_character.LV_Attack_Earth + ")");
        Debug.Log("first monster setting: attack(" + m_monster.Power_Attack + ")");


        // 방어력 없음
    }

    void Update()
    {
        
    }
}
