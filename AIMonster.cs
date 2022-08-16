using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class AIMonster : MonoBehaviour
{
    /// <summary>
    /// 레이어 마스크(땅, 플레이어 캐릭터, 장애물)
    /// </summary>
    public LayerMask GroundMask, CharacterPlayerMask, ObstacleMask;

    /// <summary>
    /// 몬스터 HP 바
    /// </summary>
    public GameObject MonsterHPBar;

    /// <summary>
    /// 몬스터 HP 바 인스턴스
    /// </summary>
    public GameObject HPBar;

    /// <summary>
    /// 몬스터 LV (공격력) 텍스트 인스턴스
    /// </summary>
    public Text LVText;

    /// <summary>
    /// 몬스터 HP 텍스트 인스턴스
    /// </summary>
    public Text HPText;

    /// <summary>
    /// 몬스터 눈 위치
    /// </summary>
    public GameObject MonsterEye;

    /// <summary>
    /// MonsterState 스크립트
    /// </summary>
    public MonsterState monsterState;

    /// <summary>
    /// 몬스터 HP 바 위치
    /// </summary>
    private Vector3 HPBarOffset = new Vector3(0, 1.5f, 0);

    /// <summary>
    /// 몬스터가 죽었는지 체크
    /// </summary>
    private bool isplaydie = false;

    /// <summary>
    ///  Patroling 상태 시 다음 이동 지점
    /// </summary>
    public Vector3 walkPoint;

    /// <summary>
    /// Patriling 상태 시 다음 이동 지점이 정해졌는지 확인
    /// </summary>
    public bool walkPointSet;

    /// <summary>
    /// 몬스터와 캐릭터 사이의 공격 거리
    /// </summary>
    private float attackRange;

    /// <summary>
    /// Attacking 상태 시 공격 애니메이션 사이의 간격
    /// </summary>
    private float timeBetweenAttacks;

    /// <summary>
    /// Attacking 상태 시 공격 애니메이션을 수행했는지 체크
    /// </summary>
    private bool alreadyAttacked;

    /// <summary>
    /// 한번 공격하고 캐릭터가 멀어질 때 일정 간격을 두고 멀어지기 위해 공격했는지 체크
    /// </summary>
    private bool attacked;

    /// <summary>
    /// 캐릭터가 시야에 있는지
    /// </summary>
    public bool playerInSightRange;

    /// <summary>
    /// 캐릭터가 공격 범위 내에 있는지
    /// </summary>
    public bool playerInAttackRange;

    /// <summary>
    /// 데미지를 받았는지
    /// </summary>
    private bool getdamage = false;

    /// <summary>
    /// NavmeshAgent AI 몬스터
    /// </summary>
    private NavMeshAgent Monster;

    /// <summary>
    /// 몬스터 애니메이터 
    /// </summary>
    private Animator MonsterAnimator;

    /// <summary>
    /// 몬스터 HP 바 캔버스 UI 
    /// </summary>
    private Canvas HBBarUICanvas;

    /// <summary>
    /// 몬스터 HP 바 이미지
    /// </summary>
    private Image HPBarImage;

    private Rigidbody MonsterRigidbody;

    private Collider slthit = null;

    void init()
    {
        attackRange = 2f;
        timeBetweenAttacks = 3f;

        monsterState = GetComponent<MonsterState>();
        Monster = GetComponent<NavMeshAgent>();
        Monster.stoppingDistance = attackRange * 0.3f;
        MonsterAnimator = GetComponent<Animator>();
        MonsterRigidbody = GetComponent<Rigidbody>();

        playerInSightRange = false;
        playerInAttackRange = false;
        walkPointSet = false;
        alreadyAttacked = false;
        attacked = false;

        HBBarUICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        HPBar = Instantiate<GameObject>(MonsterHPBar, HBBarUICanvas.transform);
        HPBarImage = HPBar.GetComponentsInChildren<Image>()[1]; //자식, 0번은 자기자신
        LVText = HPBar.GetComponentsInChildren<Text>()[0];
        HPText = HPBar.GetComponentsInChildren<Text>()[1];
    }

    void Start()
    {
        init();
        SetHPBar();
    }

    void Update()
    {
        MonsterRigidbody.velocity = Vector3.zero;
        MonsterRigidbody.angularVelocity = Vector3.zero;

        if (monsterState.HP <= 0f)
        {
            StartCoroutine(WaitForDie(2f));
        }
        
        StartCoroutine(StartMonster());     
    }

    IEnumerator StartMonster()
    {
        if (monsterState.HP >= 0f && !isplaydie)
        {
            if (!playerInSightRange && !playerInAttackRange)
            {
                Patroling();
            }
            else if (slthit != null)
            {
                if (playerInSightRange && !playerInAttackRange)
                {
                    if (attacked)
                    {
                        StartCoroutine(WaitForChase(2f, slthit));
                    }
                    else
                    {
                        Monster.enabled = true;
                        if (getdamage == false)
                            ChasePlayer(slthit);
                    }
                }
                else if (playerInSightRange && playerInAttackRange)
                {
                    if (getdamage == false)
                    {
                        StartCoroutine(WaitForAttack(1f, slthit));
                        CharacterState characterState = slthit.GetComponent<CharacterState>();

                        if (characterState.HP <= 0f)
                        {
                            MonsterAnimator.Play("Roar");
                            playerInSightRange = false;
                            playerInAttackRange = false;
                        }
                    }
                }
            }
        }
        yield return new WaitForEndOfFrame();
    }
    /// <summary>
    /// 적이 캐릭터 플레이어를 따라갈 때 일정 간격(delay) 설정
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator WaitForChase(float delay, Collider hit)
    {
        Locomotion(0);
        Turning(0);
        yield return new WaitForSeconds(delay);
        ChasePlayer(hit);
        attacked = false;
    }

    IEnumerator WaitForAttack(float delay, Collider hit)
    {
        Locomotion(0);
        Turning(0);
        yield return new WaitForSeconds(delay);
        AttackPlayer(hit);
        attacked = true;
    }

    IEnumerator WaitForDamage(float delay)
    {
        Monster.enabled = false;
        playerInSightRange = false;
        PlayDamage();
        Locomotion(0);
        Turning(0);
        yield return new WaitForSeconds(delay);
        Monster.enabled = true;
        getdamage = false;
        playerInSightRange = true;
    }

    /// <summary>
    /// 죽을 때 일정 간격(delay) 설정
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator WaitForDie(float delay)
    {
        Monster.enabled = false;
        MonsterRigidbody.velocity = Vector3.zero;
        MonsterRigidbody.angularVelocity = Vector3.zero;
        PlayDeath();

        yield return new WaitForSeconds(delay);
        DestroyMonster();
    }

    IEnumerator ResetAttacked(float timeBetweenAttacks)
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        alreadyAttacked = false;
    }

    /// <summary>
    /// 적의 시야에 캐릭터 플레이어가 없을 경우 
    /// 적은 순찰 수행
    /// </summary>
    void Patroling()
    {
        MonsterAnimator.speed = 2f;
        Locomotion(1);

        if(!walkPointSet)
        {
            SearchWalkPoint();
        }

        if (walkPointSet)
        {
            if (Vector3.Angle(Monster.transform.forward, walkPoint) < 10f)
            {
                Turning(0);
            }
        }
    }

    /// <summary>
    /// 적의 순찰 할 곳의 위치를 지정
    /// </summary>
    void SearchWalkPoint()
    {
        MonsterAnimator.speed = 2f;
        Locomotion(0);

        if (Physics.Raycast(walkPoint, -transform.up, 10f, GroundMask))
        {
            Vector3 RelativePos = Monster.transform.InverseTransformPoint(walkPoint);

            if (RelativePos.x > 0f)
            {
                Turning(0.5f);
            }
            else if (RelativePos.x < 0f)
            {
                Turning(-0.5f);
            }
        }
    }

    /// <summary>
    /// 적의 시야에 캐릭터 플레이어가 있을 경우 
    /// 캐릭터 플레이어를 쫒아감
    /// </summary>
    void ChasePlayer(Collider CharacterPlayer)
    {
        Locomotion(1);
        MonsterAnimator.speed = 3f;
        Monster.speed = 0.5f;

        Vector3 RelativePos = Monster.transform.InverseTransformPoint(CharacterPlayer.transform.position);

        if (Vector3.Angle(Monster.transform.forward, (CharacterPlayer.transform.position - Monster.transform.position).normalized) < 10)
        {
            Turning(0);
        }
        else if (RelativePos.x > 0f)
        {
            Turning(0.5f);
        }
        else if (RelativePos.x < 0f)
        {
            Turning(-0.5f);
        }

        if (Physics.Raycast(MonsterEye.transform.position, (CharacterPlayer.transform.position - MonsterEye.transform.position).normalized, 10f, CharacterPlayerMask))
        {
            Monster.enabled = true;
        }
    }

    /// <summary>
    /// 일정 거리 이하일 경우 캐릭터 플레이어 공격
    /// </summary>
    void AttackPlayer(Collider CharacterPlayer)
    {
        MonsterAnimator.speed = 1f;
        Monster.velocity = new Vector3();

        transform.LookAt(CharacterPlayer.transform);

        Vector3 rot = transform.rotation.eulerAngles;
        rot.x = 0;
        rot.z = 0;

        transform.rotation = Quaternion.Euler(rot);

        if (!alreadyAttacked && !isplaydie)
        {
            //공격하는 애니메이션 수행 && 공격 수행
            int randomnum = Random.Range(0, 2);

            if (randomnum == 0)
                TriggerAnimation("Attack1");
            else if (randomnum == 1)
                TriggerAnimation("Attack2");

            alreadyAttacked = true;

            StartCoroutine(ResetAttacked(timeBetweenAttacks));
        }
    }

    /// <summary>
    /// 다음 공격까지의 애니메이션 수행 시간 -> 수정하기 애니메이션 끝난 시간으로
    /// </summary>
    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    /// <summary>
    /// 몬스터가 공격을 받았을 경우
    /// </summary>
    public void GetDamage()
    {
        if (!isplaydie)
        {
            getdamage = true;
            StartCoroutine(WaitForDamage(2f));
        }
    }

    void PlayDamage()
    {
        if (!isplaydie)
        {
            MonsterAnimator.speed = 1f;
            MonsterAnimator.Play("GotHit");
        }
    }

    void PlayDeath()
    {
        if(!isplaydie)
        {
            isplaydie = true;
            MonsterAnimator.speed = 1f;
            TriggerAnimation("Death");
        }
    }

    /// <summary>
    /// 몬스터의 HP가 없을 경우 
    /// </summary>
    void DestroyMonster()
    {
        Monster.enabled = false;
        Destroy(gameObject, 2f);
        Destroy(HPBar, 2f);
        Destroy(LVText, 2f);
        Destroy(HPText, 2f);
    }

    /// <summary>
    /// 몬스터 애니메이션 Idle walk back
    /// </summary>
    /// <param name="value"></param>
    public void Locomotion(float value)
    {
        MonsterAnimator.SetFloat("Locomotion", value);
    }

    /// <summary>
    /// 몬스터 애니메이션 right left 
    /// </summary>
    /// <param name="value"></param>
    public void Turning(float value)
    {
        MonsterAnimator.SetFloat("Turning", value);
    }

    /// <summary>
    /// 몬스터 애니메이션 공격, 죽음
    /// </summary>
    /// <param name="value"></param>
    public void TriggerAnimation(string value)
    {
        MonsterAnimator.SetTrigger(value);
    }

    /// <summary>
    /// 몬스터 HP 설정
    /// </summary>
    void SetHPBar()
    {
        var _hpbar = HPBar.GetComponent<AIMonsterHPBar>();
        _hpbar.MonsterTr = Monster.transform;
        _hpbar.offset = HPBarOffset;

        LVText.text = "LV. " + monsterState.Power_Attack.ToString();
        int m_HP = 0; 
        if (monsterState.HP > 0f && monsterState.HP < 1f )
            m_HP = 1;

        HPText.text = m_HP.ToString() + " / " + monsterState.m_maxHP.ToString();

        MonsterHPBar.SetActive(true);
        HPBar.SetActive(true);
    }

    /// <summary>
    /// 몬스터 HP Bar 업데이트
    /// </summary>
    /// <param name="HPratio"></param>
    public void UpdateHPBar(float HPratio)
    {
        HPBarImage.fillAmount = HPratio;
        LVText.text = "LV. " + monsterState.Power_Attack.ToString();
        int monsterhptext = (int)monsterState.HP;
        HPText.text = monsterhptext.ToString() + " / " + monsterState.m_maxHP.ToString();
        //Debug.Log("Monster Bar: " + monsterhptext);
    }
}
