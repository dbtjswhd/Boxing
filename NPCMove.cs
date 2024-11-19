using System.Collections;
using UnityEngine;

public class NPCMove : MonoBehaviour
{
    public float moveSpeed = 1f;         // 이동 속도
    public float attackRange = 1.5f;       // 공격 범위
    public float attackCooldown = 0.8f;    // 공격 쿨타임
    public Transform player;               // 플레이어 위치 참조

    private float lastAttackTime;          // 마지막 공격 시간
    [SerializeField] private Animator animator;

    private enum State { Idle, Moving, Attacking }
    private State currentState = State.Idle;
    [SerializeField] private Transform opponent; // 상대 플레이어 참조 추가

    private void Start()
    {
        StartCoroutine(AIBehavior());
    }
    private void Update()
    {
        if (opponent != null)
        {
            transform.LookAt(new Vector3(opponent.position.x, transform.position.y, opponent.position.z));
        }
    }

    private IEnumerator AIBehavior()
    {
        while (true)
        {
            switch (currentState)
            {
                case State.Idle:
                    yield return Idle();
                    break;
                case State.Moving:
                    yield return MoveTowardsPlayer();
                    break;
                case State.Attacking:
                    yield return AttackPlayer();
                    break;
            }
        }
    }

    private IEnumerator Idle()
    {
        float idleTime = Random.Range( 0.1f, 1f);  // 랜덤한 대기 시간
        yield return new WaitForSeconds(idleTime);

        // 상태 전환: 플레이어와 거리에 따라 이동 또는 공격
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            currentState = State.Attacking;
        }
        else
        {
            currentState = State.Moving;
        }
    }

    private IEnumerator MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float moveDuration = Random.Range(0.5f, 2f);  // 랜덤 이동 시간

        float elapsedTime = 0f;
        animator.SetFloat("MoveX", direction.z);
        animator.SetFloat("MoveY", direction.x);
        while (elapsedTime < moveDuration)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
            elapsedTime += Time.deltaTime;

            // 중간에 공격 가능하면 즉시 공격 상태로 전환
            if (Vector3.Distance(transform.position, player.position) < attackRange)
            {
                currentState = State.Attacking;
                yield break;
            }

            yield return null;
        }

        currentState = State.Idle;
    }

    private IEnumerator AttackPlayer()
    {
        if (Time.time - lastAttackTime > attackCooldown)
        {
            // 공격 애니메이션 또는 공격 동작 호출
            int skillNum = Random.Range(0, 5);
            
            switch (skillNum)
            {
                case 0:
                    animator.SetTrigger("Jab");
                    break;
                case 1:
                    animator.SetTrigger("Straight");
                    break;
                case 2:
                    animator.SetTrigger("LeftHook");
                    break;
                case 3:
                    animator.SetTrigger("Upper");
                    break;
                case 4:
                    animator.SetTrigger("OneTwo");
                    break;
            }

            lastAttackTime = Time.time;
        }

        // 공격 후 대기 또는 이동
        currentState = State.Idle;
        yield return new WaitForSeconds(attackCooldown);
    }
}