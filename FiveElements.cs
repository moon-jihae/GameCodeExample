using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiveElements : MonoBehaviour
{
    // 캐릭터가 몬스터한테 주는 데미지 계산 
    public float CharacterToMonsterDamage(CharacterState m_character, MonsterState m_monster)
    {
        // 캐릭터 데미지 정의
        float damage = 0.0f;

        // 캐릭터 공격 무기 능력 정의 #################################### 나중에 다시 할당 
        int CharacterLVAttackWeaponFire = 2;
        int CharacterLVAttackWeaponWater = 0;
        int CharacterLVAttackWeaponWood = 0;
        int CharacterLVAttackWeaponMetal = 0;
        int CharacterLVAttackWeaponEarth = 0;

        // 캐릭터 공격력 
        int CharacterLVAttackFire = m_character.LV_Attack_Fire;
        int CharacterLVAttackWater = m_character.LV_Attack_Water;
        int CharacterLVAttackWood = m_character.LV_Attack_Wood;
        int CharacterLVAttackMetal = m_character.LV_Attack_Metal;
        int CharacterLVAttackEarth = m_character.LV_Attack_Earth;

        // 파티 여부
        bool CharacterParty = m_character.m_party.HavingParty;

        // 파티를 맺고 있을 경우
        if (CharacterParty)
        {
            // 파티원 공격력
            int PartyMemberLVAttackFire = m_character.m_party.LV_Attack_Fire;
            int PartyMemberLVAttackWater = m_character.m_party.LV_Attack_Water;
            int PartyMemberLVAttackWood = m_character.m_party.LV_Attack_Wood;
            int PartyMemberLVAttackMetal = m_character.m_party.LV_Attack_Metal;
            int PartyMemberLVAttackEarth = m_character.m_party.LV_Attack_Earth;

            // 파티를 맺은 파티원과 상생 관계를 통해 속성이 높아짐
            // 화 <- 목
            if (CharacterLVAttackFire < PartyMemberLVAttackWood)
                CharacterLVAttackFire = PartyMemberLVAttackWood;
            // 수 <- 금
            if (CharacterLVAttackWater < PartyMemberLVAttackMetal)
                CharacterLVAttackWater = PartyMemberLVAttackMetal;
            // 목 <- 수
            if (CharacterLVAttackWood < PartyMemberLVAttackWater)
                CharacterLVAttackWood = PartyMemberLVAttackWater;
            // 금 <- 토
            if (CharacterLVAttackMetal < PartyMemberLVAttackEarth)
                CharacterLVAttackMetal = PartyMemberLVAttackEarth;
            // 토 <- 화
            if (CharacterLVAttackEarth < PartyMemberLVAttackFire)
                CharacterLVAttackEarth = PartyMemberLVAttackFire;
        }

        // 오행별 계산
        float finalAttackDamageFire = (float)CharacterLVAttackFire / 10.0f * CharacterLVAttackWeaponFire;
        float finalAttackDamageWater = (float)CharacterLVAttackWater / 10.0f * CharacterLVAttackWeaponWater;
        float finalAttackDamageWood = (float)CharacterLVAttackWood / 10.0f * CharacterLVAttackWeaponWood;
        float finalAttackDamageMetal = (float)CharacterLVAttackMetal / 10.0f * CharacterLVAttackWeaponMetal;
        float finalAttackDamageEarth = (float)CharacterLVAttackEarth / 10.0f * CharacterLVAttackWeaponEarth;

        // 몬스터 속성
        int MonsterElement = (int)m_monster.m_elem;

        // 몬스터가 화 속성일 경우 
        if (MonsterElement == 0)
            finalAttackDamageWater = finalAttackDamageWater * 2.0f;
        // 몬스터가 수 속성일 경우 
        if (MonsterElement == 1)
            finalAttackDamageEarth = finalAttackDamageEarth * 2.0f;
        // 몬스터가 목 속성일 경우 
        if (MonsterElement == 2)
            finalAttackDamageMetal = finalAttackDamageMetal * 2.0f;
        // 몬스터가 금 속성일 경우 
        if (MonsterElement == 3)
            finalAttackDamageFire = finalAttackDamageFire * 2.0f;
        // 몬스터가 토 속성일 경우 
        if (MonsterElement == 4)
            finalAttackDamageWood = finalAttackDamageWood * 2.0f;

        damage = finalAttackDamageFire + finalAttackDamageWater + finalAttackDamageWood + finalAttackDamageMetal + finalAttackDamageEarth;

        return damage;
    }

    // 몬스터가 캐릭터한테 주는 데미지 계산 
    public float MonsterToCharacterDamage(CharacterState m_character, MonsterState m_monster)
    {
        // 캐릭터 데미지 정의
        float damage = m_monster.Power_Attack;

        // 캐릭터 방어 무기 능력 정의 #################################### 나중에 다시 할당 
        int CharacterLVDefenseWeaponFire = 2;
        int CharacterLVDefenseWeaponWater = 0;
        int CharacterLVDefenseWeaponWood = 0;
        int CharacterLVDefenseWeaponMetal = 0;
        int CharacterLVDefenseWeaponEarth = 0;

        // 캐릭터 방어력
        int CharacterLVDefenseFire = m_character.LV_Defense_Fire;
        int CharacterLVDefenseWater = m_character.LV_Defense_Water;
        int CharacterLVDefenseWood = m_character.LV_Defense_Wood;
        int CharacterLVDefenseMetal = m_character.LV_Defense_Metal;
        int CharacterLVDefenseEarth = m_character.LV_Defense_Earth;

        // 파티 여부
        bool CharacterParty = m_character.m_party.HavingParty;

        // 파티를 맺고 있을 경우
        if (CharacterParty)
        {
            // 파티원 방어력
            int PartyMemberLVDefenseFire = m_character.m_party.LV_Defense_Fire;
            int PartyMemberLVDefenseWater = m_character.m_party.LV_Defense_Water;
            int PartyMemberLVDefenseWood = m_character.m_party.LV_Defense_Wood;
            int PartyMemberLVDefenseMetal = m_character.m_party.LV_Defense_Metal;
            int PartyMemberLVDefenseEarth = m_character.m_party.LV_Defense_Earth;

            // 파티를 맺은 파티원과 상생 관계를 통해 속성이 높아짐
            // 화 <- 목
            if (CharacterLVDefenseFire < PartyMemberLVDefenseWood)
                CharacterLVDefenseFire = PartyMemberLVDefenseWood;
            // 수 <- 금
            if (CharacterLVDefenseWater < PartyMemberLVDefenseMetal)
                CharacterLVDefenseWater = PartyMemberLVDefenseMetal;
            // 목 <- 수
            if (CharacterLVDefenseWood < PartyMemberLVDefenseWater)
                CharacterLVDefenseWood = PartyMemberLVDefenseWater;
            // 금 <- 토
            if (CharacterLVDefenseMetal < PartyMemberLVDefenseEarth)
                CharacterLVDefenseMetal = PartyMemberLVDefenseEarth;
            // 토 <- 화
            if (CharacterLVDefenseEarth < PartyMemberLVDefenseFire)
                CharacterLVDefenseEarth = PartyMemberLVDefenseFire;
        }
        // 몬스터 속성
        int MonsterElement = (int)m_monster.m_elem;
        
        // 오행별 계산
        float finalDefenseDamageFire = (float)CharacterLVDefenseFire / 10.0f * CharacterLVDefenseWeaponFire;
        float finalDefenseDamageWater = (float)CharacterLVDefenseWater / 10.0f * CharacterLVDefenseWeaponWater;
        float finalDefenseDamageWood = (float)CharacterLVDefenseWood / 10.0f * CharacterLVDefenseWeaponWood;
        float finalDefenseDamageMetal = (float)CharacterLVDefenseMetal / 10.0f * CharacterLVDefenseWeaponMetal;
        float finalDefenseDamageEarth = (float)CharacterLVDefenseEarth / 10.0f * CharacterLVDefenseWeaponEarth;

        // 몬스터가 화 속성일 경우 
        if (MonsterElement == 0)
            finalDefenseDamageMetal = finalDefenseDamageMetal * 0.5f;
        // 몬스터가 수 속성일 경우 
        if (MonsterElement == 1)
            finalDefenseDamageFire = finalDefenseDamageFire * 0.5f;
        // 몬스터가 목 속성일 경우 
        if (MonsterElement == 2)
            finalDefenseDamageEarth = finalDefenseDamageEarth * 0.5f;
        // 몬스터가 금 속성일 경우 
        if (MonsterElement == 3)
            finalDefenseDamageWood = finalDefenseDamageWood * 0.5f;
        // 몬스터가 토 속성일 경우 
        if (MonsterElement == 4)
            finalDefenseDamageWater = finalDefenseDamageWater * 0.5f;

        // 캐릭터 최종 방어력
        float finalDefense = finalDefenseDamageFire + finalDefenseDamageWater + finalDefenseDamageWood + finalDefenseDamageMetal + finalDefenseDamageEarth;

        damage = damage - finalDefense;

        if (damage < 0)
            damage = 0.0f;

        return damage;
    }
}
