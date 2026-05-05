using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Header("🎯 Цель")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float despawnTime = 10f; // Время до ухода зомби

    [Header("🏃 Скорость")]
    [SerializeField] private float walkSpeed = 2f;    // Шаг (когда нет игрока)
    [SerializeField] private float runSpeed = 6f;     // Бег (когда видит игрока)

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

    private float lastAttackTime = 0f;
    private bool isDead = false;
    private bool isAttacking = false;
    private float lastSeenPlayerTime = 0f;
    private bool playerInRange = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();

        // 🔹 Отключаем Root Motion для NavMeshAgent
        if (animator != null)
            animator.applyRootMotion = false;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (animator != null)
            animator.SetBool(deathAnimBool, false);
    }

    private void Update()
    {
        if (isDead) return;

        // 🔹 Проверка: жив ли игрок и существует ли
        if (PlayerHealth.Instance == null || !PlayerHealth.Instance.IsAlive())
        {
            player = null;
        }
        else if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        // 🔹 Логика поведения
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                // 🔹 ИГРОК УВИДЕН — БЕЖИМ!
                playerInRange = true;
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
                playerInRange = false;
                agent.speed = walkSpeed;
                agent.isStopped = true;

                if (animator != null)
                    animator.SetFloat(speedAnimFloat, 0f);
            }

            // 🔹 Проверка: если игрока нет рядом больше N секунд — зомби уходит
            if (Time.time - lastSeenPlayerTime > despawnTime)
            {
                DespawnZombie();
            }
        }
        else
        {
            // 🔹 ИГРОКА НЕТ В СЦЕНЕ — зомби уходит
            if (Time.time - lastSeenPlayerTime > despawnTime)
            {
                DespawnZombie();
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
        
        // 🔹 Анимация смерти
        if (animator != null)
        {
            animator.SetBool(deathAnimBool, true);
            animator.SetFloat(speedAnimFloat, 0f);
        }
        
        Debug.Log("🧟 Зомби умер!");
        Destroy(gameObject, 3f);
    }

    // 🔹 Зомби уходит (игрок исчез)
    private void DespawnZombie()
    {
        Debug.Log("🧟 Зомби ушёл (игрок не найден)");
        
        // Можно добавить анимацию ухода или просто удалить
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}