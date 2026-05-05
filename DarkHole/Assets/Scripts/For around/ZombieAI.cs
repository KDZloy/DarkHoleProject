using UnityEngine;
using UnityEngine.AI; // Требуется пакет AI Navigation

public class ZombieAI : MonoBehaviour
{
    [Header("🎯 Цель")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 2f;

    [Header("⚔️ Бой")]
    [SerializeField] private int damage = 15; // Урон зомби (15 HP)
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("❤️ Здоровье зомби")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("🔧 Компоненты")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    private float lastAttackTime = 0f;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (isDead || player == null || agent == null) return;
        if (!PlayerHealth.Instance.IsAlive()) return; // Если игрок умер, зомби замирает

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            agent.SetDestination(player.position);

            if (distanceToPlayer <= attackRange)
            {
                agent.isStopped = true;
                TryAttack();
            }
            else
            {
                agent.isStopped = false;
            }

            // Поворот к игроку
            if (distanceToPlayer > attackRange && agent.velocity.magnitude > 0.1f)
            {
                Vector3 direction = agent.velocity.normalized;
                Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10f * Time.deltaTime);
            }
        }
        else
        {
            agent.isStopped = true;
        }

        // Анимация
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    private void TryAttack()
    {
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            Attack();
        }
    }

    private void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                break;
            }
        }
    }

    // 🔹 Метод для получения урона (если игрок сможет атаковать)
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"🧟 Зомби получил {amount} урона. Здоровье: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (agent != null) agent.isStopped = true;
        Debug.Log("🧟 Зомби умер!");

        if (animator != null)
            animator.SetBool("IsDead", true);

        Destroy(gameObject, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}