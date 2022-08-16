using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyState
{
    // 캐릭터가 파티를 맺고 있는지 
    public bool HavingParty = false;

    // 파티 요청이 들어왔는지
    public bool RequestParty = false;

    // 파티를 요청한 플레이어의 ID
    public string RequestPlayerID = string.Empty;

    // 파티 요청에 대한 결과가 왔을 경우-true, 결과를 확인한 후-false
    public bool ResultPartyWithPlayer = false;

    // 파티 요청에 대한 결과
    public bool isPartyAccept = false;

    // 파티원 ID
    public string PartyPlayerID = string.Empty;

    // 파티원 닉네임
    public string PartyPlayerNickName = string.Empty;

    // 요청을 보낸 플레이어의 레벨(1~100)
    public int Request_LV_Attack_Fire = 0;
    public int Request_LV_Attack_Water = 0;
    public int Request_LV_Attack_Wood = 0;
    public int Request_LV_Attack_Metal = 0;
    public int Request_LV_Attack_Earth = 0;

    public int Request_LV_Defense_Fire = 0;
    public int Request_LV_Defense_Water = 0;
    public int Request_LV_Defense_Wood = 0;
    public int Request_LV_Defense_Metal = 0;
    public int Request_LV_Defense_Earth = 0;

    // 파티원 레벨(1~100)
    public int LV_Attack_Fire = 0;
    public int LV_Attack_Water = 0;
    public int LV_Attack_Wood = 0;
    public int LV_Attack_Metal = 0;
    public int LV_Attack_Earth = 0;

    public int LV_Defense_Fire = 0;
    public int LV_Defense_Water = 0;
    public int LV_Defense_Wood = 0;
    public int LV_Defense_Metal = 0;
    public int LV_Defense_Earth = 0;
}
