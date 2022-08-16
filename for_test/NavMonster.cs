using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMonster : MonoBehaviour
{
    public NavMeshAgent monster; // AI 몬스터

    public Transform player; // 캐릭터 플레이어

    public LayerMask whatIsGround, whatIsPlayer; // 레이어 마스크

    public float MonsterHealth; // 몬스터 체력

    public Animator MonsterAnimator; //몬스터 애니메이터

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("SK_GhostLadyS2_Style (4) Variant").transform;
        monster = GetComponent<NavMeshAgent>();
        MonsterAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        float temp = Mathf.Cos(Mathf.Deg2Rad * 45);
        float deg = Mathf.Rad2Deg * Mathf.Acos(temp);
        float dot = Vector3.Dot(monster.transform.forward, (monster.transform.position + player.transform.position).normalized);
        float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
        ///Vector3 targetDir = (player.position - monster.transform.position).normalized;
        ///float dot = Vector3.Dot(monster.transform.position, targetDir);
        ///float theta = Mathf.Acos(dot) * Mathf.Rad2Deg;
        //Debug.Log(theta);
        if (theta <= 45) 
            playerInSightRange = true;
        // player가 안보이면 playerInSightRange = false

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling(); // 어슬렁 거리기
        if (playerInSightRange && !playerInAttackRange) ChasePlayer(); // 캐릭터를 발견하면 쫒기
        if (playerInSightRange && playerInAttackRange) AttackPlayer(); // 만나면 공격
    }

    void Patroling()
    {
        MonsterAnimator.speed = 1f;
        TriggerAnimation("Idle_break");

        if (!walkPointSet) 
            SearchWalkPoint();

        if (walkPointSet)
            monster.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 0.1f)
        {
            //TriggerAnimation("Roar");
            walkPointSet = false;
        }
    }

    void SearchWalkPoint()
    {
        Debug.Log("SearchWalkPoint");
        Locomotion(1);
        MonsterAnimator.speed = 2f;
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        //walkPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        if (Physics.Raycast(walkPoint, -transform.up, 10f, whatIsGround))
            walkPointSet = true;
    }

    void ChasePlayer()
    {
        Debug.Log("ChasePlayer");
        Locomotion(1);
        MonsterAnimator.speed = 3f;
        monster.speed = 3f;
        if (Physics.Raycast(monster.transform.position, (player.position - monster.transform.position).normalized, 10f, whatIsPlayer))
            monster.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        Debug.Log("AttackPlayer");
        monster.SetDestination(transform.position);
        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            //공격하는 애니메이션 수행 && 공격 수행
            TriggerAnimation("Attack2");
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    void ResetAttack()
    {
        alreadyAttacked = false;
    }

    void TakeDamage(int damage)
    {
        MonsterHealth -= damage;

        if (MonsterHealth <= 0)
            Invoke(nameof(DestroyMonster), 1f);
    }

    void DestroyMonster()
    {
        Destroy(gameObject);
    }

    public void Locomotion(float value)
    {
        MonsterAnimator.SetFloat("Locomotion", value);
    }

    public void Turning(float value)
    {
        MonsterAnimator.SetFloat("Turning", value);
    }

    public void TriggerAnimation(string value)
    {
        MonsterAnimator.SetTrigger(value);
    }
}
