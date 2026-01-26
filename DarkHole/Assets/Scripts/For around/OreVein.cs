using UnityEngine;

public class OreVein : MonoBehaviour
{
    public struct DropItem
    {
        public string name;
        public GameObject prefab;
        [Range(0f, 100f)] public float chance;
        public int minAmount;
        public int maxAmount;
    }

    [Header("Здоровье")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Дроп (сумма = 100%)")]
    public DropItem[] dropTable = new DropItem[]
    {
        new DropItem { name = "Камень",   prefab = null, chance = 40f, minAmount = 2, maxAmount = 4 },
        new DropItem { name = "Медь",     prefab = null, chance = 25f, minAmount = 1, maxAmount = 3 },
        new DropItem { name = "Железо",   prefab = null, chance = 15f, minAmount = 1, maxAmount = 2 },
        new DropItem { name = "Золото",   prefab = null, chance = 10f, minAmount = 1, maxAmount = 1 },
        new DropItem { name = "Алмаз",    prefab = null, chance = 6f,  minAmount = 1, maxAmount = 1 },
        new DropItem { name = "Кобальт",  prefab = null, chance = 4f,  minAmount = 1, maxAmount = 1 }
    };

    [Header("3 стадии разрушения")]
    public GameObject stage1_Full;      // Большая глыба
    public GameObject stage2_Medium;    // Средняя
    public GameObject stage3_Small;     // Маленькая с рудой

    [Header("Эффекты")]
    public ParticleSystem hitParticles; // Пыль/осколки при ударе
    public AudioClip hitSound;
    public AudioClip breakSound;

    private AudioSource audioSource;

    public void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        // Показываем только первую стадию
        SetActiveStage(1);
    }

    // Вызывается из скрипта кирки
    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // Обновляем визуальную стадию
        UpdateStage();

        // Эффект удара
        if (hitParticles != null)
        {
            var ps = Instantiate(hitParticles, hitPoint, Quaternion.LookRotation(-transform.up));
            ps.Play();
            Destroy(ps.gameObject, ps.main.duration);
        }

        // Звук
        if (hitSound != null) audioSource.PlayOneShot(hitSound);

        // Разрушение
        if (currentHealth <= 0)
        {
            DropLoot();
            if (breakSound != null) audioSource.PlayOneShot(breakSound);
            Destroy(gameObject, 0.3f); // Задержка для звука разрушения
        }
    }

    private void UpdateStage()
    {
        float percent = (float)currentHealth / maxHealth * 100f;

        if (percent > 66f)
            SetActiveStage(1);
        else if (percent > 33f)
            SetActiveStage(2);
        else
            SetActiveStage(3);
    }

    private void SetActiveStage(int stage)
    {
        // Скрываем все
        stage1_Full?.SetActive(false);
        stage2_Medium?.SetActive(false);
        stage3_Small?.SetActive(false);

        // Показываем нужную
        switch (stage)
        {
            case 1: stage1_Full?.SetActive(true); break;
            case 2: stage2_Medium?.SetActive(true); break;
            case 3: stage3_Small?.SetActive(true); break;
        }
    }

    private void DropLoot()
    {
        float roll = Random.Range(0f, 100f);
        float accumulated = 0f;

        foreach (var drop in dropTable)
        {
            accumulated += drop.chance;
            if (roll <= accumulated && drop.prefab != null)
            {
                int amount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                for (int i = 0; i < amount; i++)
                {
                    Vector3 pos = transform.position + new Vector3(
                        Random.Range(-0.6f, 0.6f),
                        0.5f,
                        Random.Range(-0.6f, 0.6f)
                    );

                    GameObject loot = Instantiate(drop.prefab, pos, Quaternion.identity);

                    if (loot.TryGetComponent(out Rigidbody rb))
                    {
                        rb.AddForce(Vector3.up * 3f + new Vector3(
                            Random.Range(-1.5f, 1.5f),
                            0,
                            Random.Range(-1.5f, 1.5f)
                        ), ForceMode.Impulse);
                        rb.linearDamping = 3f;
                    }
                }

                Debug.Log($"⛏️ Добыто: {drop.name} x{amount}");
                break;
            }
        }
    }
}