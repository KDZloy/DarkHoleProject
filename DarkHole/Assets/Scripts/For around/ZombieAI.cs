using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("🎯 Цель")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float despawnTime = 10f; // Время до возврата

    [Header("🏃 Скорость")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float runSpeed = 6f;

    [Header("⚔️ Бой")]
    [SerializeField] private int damage = 15;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private LayerMask playerLayer;

    [Header("❤️ Здоровье")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [Header("🔧 Компоненты")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("🎬 Анимации")]
    [SerializeField] private string attackAnimTrigger = "Attack";
    [SerializeField] private string deathAnimBool = "IsDead";
    [SerializeField] private string speedAnimFloat = "Speed";

    [Header("🏠 Спавн")]
    [SerializeField] private Vector3 spawnPoint; // Точка спавна

    private float lastAttackTime = 0f;
    private bool isDead = false;
    private bool isAttacking = false;
    private float lastSeenPlayerTime = 0f;
    private bool isReturning = false; // 🔹 Возвращается ли зомби

    private void Start()
    {
        currentHealth = maxHealth;

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        // 🔹 Сохраняем точку спавна
        spawnPoint = transform.position;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (animator != null)
            animator.SetBool(deathAnimBool, false);
    }

    private void Update()
    {
        if (isDead) return;

        // 🔹 Проверка: жив ли игрок
        if (PlayerHealth.Instance == null || !PlayerHealth.Instance.IsAlive())
        {
            player = null;
        }
        else if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        // 🔹 Логика поведения
        if (player != null && !isReturning)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                // 🔹 ИГРОК УВИДЕН — БЕЖИМ!
                lastSeenPlayerTime = Time.time;
                agent.speed = runSpeed;
                agent.SetDestination(player.position);

                if (animator != null)
                    animator.SetFloat(speedAnimFloat, agent.velocity.magnitude);

                // Атака если близко
                if (distanceToPlayer <= attackRange)
                {
                    agent.isStopped = true;
                    if (!isAttacking) TryAttack();
                }
                else
                {
                    agent.isStopped = false;
                    isAttacking = false;
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
                // 🔹 ИГРОК ДАЛЕКО — ИДЁМ ШАГОМ
                agent.speed = walkSpeed;
                agent.isStopped = true;

                if (animator != null)
                    animator.SetFloat(speedAnimFloat, 0f);
            }

            // 🔹 Проверка: если игрока нет рядом больше N секунд — ВОЗВРАЩАЕМСЯ
            if (Time.time - lastSeenPlayerTime > despawnTime)
            {
                ReturnToSpawn();
            }
        }
        else if (isReturning)
        {
            // 🔹 Зомби возвращается в точку спавна
            float distanceToSpawn = Vector3.Distance(transform.position, spawnPoint);

            if (distanceToSpawn > 1f)
            {
                agent.speed = walkSpeed;
                agent.SetDestination(spawnPoint);
                agent.isStopped = false;

                if (animator != null)
                    animator.SetFloat(speedAnimFloat, agent.velocity.magnitude);
            }
            else
            {
                // 🔹 Вернулся на место
                isReturning = false;
                agent.isStopped = true;
                agent.ResetPath();

                if (animator != null)
                {
                    animator.SetFloat(speedAnimFloat, 0f);
                    animator.SetBool(deathAnimBool, false);
                }

                Debug.Log("🧟 Зомби вернулся на точку спавна!");
            }
        }
        else
        {
            // 🔹 Игрока нет в сцене — возвращаемся
            if (Time.time - lastSeenPlayerTime > despawnTime)
            {
                ReturnToSpawn();
            }
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
        isAttacking = true;
        if (animator != null) animator.SetTrigger(attackAnimTrigger);
        Invoke(nameof(DealDamage), 0.3f);
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void DealDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null) playerHealth.TakeDamage(damage);
                break;
            }
        }
    }

    private void ResetAttack() { isAttacking = false; }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"🧟 Зомби получил {amount} урона. Здоровье: {currentHealth}/{maxHealth}");

        if (animator != null && currentHealth > 0)
            animator.SetTrigger("Hit");

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        isDead = true;
        if (agent != null) agent.isStopped = true;

        if (animator != null)
        {
            animator.SetBool(deathAnimBool, true);
            animator.SetFloat(speedAnimFloat, 0f);
        }

        Debug.Log("🧟 Зомби умер!");
        Destroy(gameObject, 3f);
    }

    // 🔹 Зомби возвращается в точку спавна
    private void ReturnToSpawn()
    {
        isReturning = true;
        Debug.Log("🧟 Зомби возвращается на точку спавна!");
    }

    private void OnDrawGizmosSelected()
    {
        // Радиус обнаружения
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Радиус атаки
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Точка спавна
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(spawnPoint, 0.5f);
        Gizmos.DrawLine(transform.position, spawnPoint);
    }
}