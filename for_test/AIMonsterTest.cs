using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class AIMonsterTest : MonoBehaviour
{
    // AI 몬스터
    public NavMeshAgent Monster;

    // 몬스터 애니메이터
    public Animator MonsterAnimator;

    // 몬스터 HP 바
    public GameObject MonsterHPBar;
    public Vector3 HPBarOffset = new Vector3(0, 1f, 0);
    private Canvas UICanvas;
    private Image HPBarImage;
    
    // 몬스터 체력
    private float MonsterHealth;
    public bool isdie = false;

    // 몬스터 상태
    public enum State {
        PATROL, 
        CHASE, 
        ATTACK, 
        DIE
    }
    public State state = State.PATROL;

    // 레이어 마스크(땅, 플레이어 캐릭터, 장애물)
    public LayerMask GroundMask, CharacterPlayerMask, ObstacleMask;

    // Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    private float walkPointRange;

    // Attacking
    private float timeBetweenAttacks;
    bool alreadyAttacked;
    bool attacked;

    // States
    private float attackRange;
    public bool playerInSightRange, playerInAttackRange;

    // 캐릭터 플레이어
    private GameObject CharacterPlayer;
    
    // 테스트용
    public GameObject Sphere;

    private void Awake()
    {
        //var monster = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        //StartCoroutine(CheckState());
    }
    void Start()
    {
        //CharacterPlayer = GameObject.Find("SK_GhostLadyS2_Style (4) Variant"); //************private로 가져오기->보내주신코드참고
        Monster = GetComponent<NavMeshAgent>();
        CharacterPlayer = GameObject.FindGameObjectWithTag("Player");       
        MonsterAnimator = GetComponent<Animator>();
        MonsterHealth = 100f;
        walkPointRange = 10f;
        timeBetweenAttacks = 1f;
        attackRange = 2f;
        attacked = false;
        SetHPBar();
    }

    void Update()
    {
        Vector3 DirToCharacter = (CharacterPlayer.transform.position - Monster.transform.position).normalized;
        float DstToCharacter = Vector3.Distance(Monster.transform.position, CharacterPlayer.transform.position);

        if (Vector3.Angle(Monster.transform.forward, DirToCharacter) < 60 && !Physics.Raycast(Monster.transform.position, DirToCharacter, DstToCharacter, ObstacleMask))
            playerInSightRange = true;
        
        if (DstToCharacter < 10f && !Physics.Raycast(Monster.transform.position, DirToCharacter, DstToCharacter, ObstacleMask))
            playerInSightRange = true;

        if (DstToCharacter > 20f || (DstToCharacter > 5f && Physics.Raycast(Monster.transform.position, DirToCharacter, DstToCharacter, ObstacleMask)))
            playerInSightRange = false;

        playerInAttackRange = Physics.CheckSphere(Monster.transform.position, attackRange, CharacterPlayerMask);

        if (!playerInSightRange && !playerInAttackRange) Patroling(); // 어슬렁 거리기
        if (playerInSightRange && !playerInAttackRange)
        {
            if (attacked)
            {
                StartCoroutine(WaitForChase(1));
            }
            else
            {
                ChasePlayer(); // 캐릭터를 발견하면 쫒기
            }
        }
        if (playerInSightRange && playerInAttackRange)
        {
            attacked = true;
            //AttackPlayer(); // 만나면 공격 
        }

        if (MonsterHealth < 0) // 죽었을 경우
        {
            MonsterAnimator.Play("Death");
            //TriggerAnimation("Death");
            StartCoroutine(WaitForDie(0.7f));
        }
    }

    /// <summary>
    /// 적이 캐릭터 플레이어를 따라갈 때 일정 간격(delay) 설정
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator WaitForChase(float delay)
    {
        Locomotion(0);
        Turning(0);
        yield return new WaitForSeconds(delay);
        ChasePlayer();
        attacked = false;
    }
    /// <summary>
    /// 몬스터가 죽었을 때 없어질 때까지 간격(delay) 설정
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator WaitForDie(float delay)
    {
        yield return new WaitForSeconds(delay);
        DestroyMonster();
    }

    IEnumerator CheckState()
    {
        while(!isdie)
        {
            if (state == State.DIE) yield break;

            float dist = Vector3.Distance(CharacterPlayer.transform.position, Monster.transform.position);

            if(dist <= attackRange)
            {
                state = State.ATTACK;
            }
            else if(playerInSightRange && !playerInAttackRange && !attacked)
            {
                state = State.CHASE;
            }
            else
            {
                state = State.PATROL;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    /// <summary>
    /// 적의 시야에 캐릭터 플레이어가 없을 경우 
    /// 적은 순찰 수행
    /// </summary>
    void Patroling()
    {
        MonsterAnimator.speed = 2f;
        Locomotion(1);

        if (!walkPointSet)
            SearchWalkPoint();

        if (walkPointSet)
        {
            Monster.SetDestination(walkPoint);
            if(Vector3.Angle(Monster.transform.forward, walkPoint) < 10f)
            {
                Turning(0);
            }
        }
            
        Vector3 distanceToWalkPoint = Monster.transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 5f || Physics.Raycast(walkPoint, (Monster.transform.position - walkPoint).normalized, 10f, ObstacleMask))
        {
            //TriggerAnimation("Roar");
            walkPointSet = false;
        }
    }

    /// <summary>
    /// 적의 순찰 할 곳의 위치를 지정
    /// </summary>
    void SearchWalkPoint()
    {
        //Debug.Log("SearchWalkPoint");
        MonsterAnimator.speed = 2f;
        Locomotion(0);

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        
        if (Physics.Raycast(walkPoint, -transform.up, 10f, GroundMask))
        {
            Vector3 RelativePos = Monster.transform.InverseTransformPoint(walkPoint);

            if (RelativePos.x > 0f)
            {
                //Debug.Log("Turning_1");
                Turning(0.5f);
            }
            else if (RelativePos.x < 0f)
            {
                //Debug.Log("Turning_2");
                Turning(-0.5f);
            }
            walkPointSet = true;
            Sphere.transform.position = walkPoint;
        }
    }

    /// <summary>
    /// 적의 시야에 캐릭터 플레이어가 있을 경우 
    /// 캐릭터 플레이어를 쫒아감
    /// </summary>
    void ChasePlayer()
    {
        //Debug.Log("ChasePlayer");
        Locomotion(1);
        MonsterAnimator.speed = 3f;
        Monster.speed = 0.5f;

        Vector3 RelativePos = Monster.transform.InverseTransformPoint(CharacterPlayer.transform.position);

        if (Vector3.Angle(Monster.transform.forward, (CharacterPlayer.transform.position - Monster.transform.position).normalized) < 10)
        {
            //Debug.Log("Angle");
            Turning(0);
        }
        else if (RelativePos.x > 0f)
        {
            //Debug.Log("Turning_1");
            Turning(0.5f);
        }
        else if (RelativePos.x < 0f)
        {
            //Debug.Log("Turning_2");
            Turning(-0.5f);
        }

        if (Physics.Raycast(Monster.transform.position, (CharacterPlayer.transform.position - Monster.transform.position).normalized, 10f, CharacterPlayerMask))
            Monster.SetDestination(CharacterPlayer.transform.position);
    }

    /// <summary>
    /// 일정 거리 이하일 경우 캐릭터 플레이어 공격
    /// </summary>
    void AttackPlayer()
    {
        //Debug.Log("AttackPlayer");
        Monster.SetDestination(transform.position);
        transform.LookAt(CharacterPlayer.transform);

        if (MonsterHealth < 0f)
            TriggerAnimation("Death");

        if (!alreadyAttacked)
        {
            //공격하는 애니메이션 수행 && 공격 수행
            TriggerAnimation("Attack2");
            alreadyAttacked = true;
            
            TakeDamage(50);
            HPBarImage.fillAmount = MonsterHealth / 100f;

            Invoke(nameof(ResetAttack), timeBetweenAttacks);
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
    /// 캐릭터 플레이어에게서 받은 공격 계산
    /// </summary>
    /// <param name="damage"></param>
    void TakeDamage(int damage)
    {
        MonsterHealth -= damage;
    }

    /// <summary>
    /// 몬스터의 HP가 없을 경우 
    /// </summary>
    void DestroyMonster()
    {
        //MonsterAnimator.SetBool("Death", false);
        //UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        Destroy(gameObject);
        //Destroy(UICanvas);
        //HPBarImage.GetComponentsInChildren<Image>()[1].color = Color.clear;
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
        UICanvas = GameObject.Find("UI Canvas").GetComponent<Canvas>();
        GameObject HPBar = Instantiate<GameObject>(MonsterHPBar, UICanvas.transform);
        HPBarImage = HPBar.GetComponentsInChildren<Image>()[1]; //자식, 0번은 자기자신

        var _hpbar = HPBar.GetComponent<AIMonsterHPBar>();
        _hpbar.MonsterTr = this.gameObject.transform;
        _hpbar.offset = HPBarOffset;
    }

    /// <summary>
    /// 유진 추가
    /// 몬스터 HP Bar 업데이트
    /// HP 나중에 위에서 몬스터 HP연결해주고 인자 지우기
    /// </summary>
    public void UpdateHPBar(float HPratio)
    {
        HPBarImage.fillAmount = HPratio;
    }
}
