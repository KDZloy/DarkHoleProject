using UnityEngine;
using System.Collections;

public class Pickaxe : MonoBehaviour
{
    [Header("=== Основные параметры ===")]
    [Tooltip("Урон за один удар киркой")]
    public int damagePerHit = 25;
    
    [Tooltip("Дальность удара (метры)")]
    public float attackRange = 3f;
    
    [Tooltip("Слой, на котором находятся глыбы руды")]
    public LayerMask oreLayer;
    
    [Tooltip("Кулдаун между ударами (секунды)")]
    [Range(0.2f, 2f)]
    public float swingCooldown = 0.6f;
    
    [Header("=== Анимация кирки ===")]
    [Tooltip("Объект кирки (дочерний элемент камеры)")]
    public Transform pickaxeTransform;
    
    [Tooltip("Анимировать через корутину (если нет Animator)")]
    public bool useCoroutineAnimation = true;
    
    [Tooltip("Продолжительность взмаха (секунды)")]
    public float swingDuration = 0.3f;
    
    [Tooltip("Если используете Animator — укажите его здесь")]
    public Animator pickaxeAnimator;
    
    [Tooltip("Триггер для анимации в Animator (например: 'Swing')")]
    public string swingTriggerName = "Swing";
    
    [Header("=== Визуальные эффекты ===")]
    [Tooltip("Система частиц для удара (пыль/искры)")]
    public ParticleSystem hitParticles;
    
    [Tooltip("Система частиц при разрушении глыбы")]
    public ParticleSystem breakParticles;
    
    [Header("=== Звуковые эффекты ===")]
    public AudioClip swingSound;
    public AudioClip hitStoneSound;
    public AudioClip hitMetalSound;
    public AudioClip breakSound;
    
    [Header("=== Дополнительные эффекты ===")]
    [Tooltip("Дрожание камеры при ударе")]
    public bool enableScreenShake = true;
    
    [Tooltip("Интенсивность дрожания (0.1 = слабое, 0.5 = сильное)")]
    [Range(0.05f, 1f)]
    public float shakeIntensity = 0.2f;
    
    [Tooltip("Продолжительность дрожания (секунды)")]
    [Range(0.1f, 0.5f)]
    public float shakeDuration = 0.15f;

    // Внутренние переменные
    private Camera playerCamera;
    private AudioSource audioSource;
    private float lastSwingTime = 0f;
    private bool isSwinging = false;
    private Vector3 pickaxeDefaultLocalPosition;
    private Quaternion pickaxeDefaultLocalRotation;

    void Start()
    {
        playerCamera = Camera.main;
        
        // Инициализация аудио
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 0f; // 2D звук
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
        
        // Сохраняем исходную позицию кирки для анимации
        if (pickaxeTransform != null && useCoroutineAnimation)
        {
            pickaxeDefaultLocalPosition = pickaxeTransform.localPosition;
            pickaxeDefaultLocalRotation = pickaxeTransform.localRotation;
        }
    }

    void Update()
    {
        // Левая кнопка мыши — удар киркой
        if (Input.GetMouseButtonDown(0))
        {
            TrySwingPickaxe();
        }
    }

    private void TrySwingPickaxe()
    {
        // Проверка кулдауна
        if (Time.time - lastSwingTime < swingCooldown || isSwinging)
            return;
        
        lastSwingTime = Time.time;
        isSwinging = true;
        
        // Raycast из центра экрана
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        bool hitOre = false;
        
        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, oreLayer))
        {
            // Попали в глыбу — наносим урон
            if (hit.collider.transform.root.TryGetComponent(out OreVein oreVein))
            {
                oreVein.TakeDamage(damagePerHit, hit.point);
                hitOre = true;
                
                // Эффекты удара в точке попадания
                if (hitParticles != null)
                {
                    var particles = Instantiate(hitParticles, hit.point, Quaternion.LookRotation(hit.normal));
                    particles.Play();
                    Destroy(particles.gameObject, particles.main.duration);
                }
                
                // Звук в зависимости от типа руды (упрощённо — камень/металл)
                if (hitMetalSound != null && 
                    (hit.collider.name.Contains("Iron") || 
                     hit.collider.name.Contains("Copper") || 
                     hit.collider.name.Contains("Gold") ||
                     hit.collider.name.Contains("Cobalt")))
                {
                    PlaySound(hitMetalSound, hit.point);
                }
                else if (hitStoneSound != null)
                {
                    PlaySound(hitStoneSound, hit.point);
                }
            }
        }
        
        // Анимация удара
        if (useCoroutineAnimation && pickaxeTransform != null)
        {
            StartCoroutine(SwingAnimationCoroutine(hitOre));
        }
        else if (pickaxeAnimator != null)
        {
            pickaxeAnimator.SetTrigger(swingTriggerName);
            StartCoroutine(ResetSwingState(swingDuration));
        }
        else
        {
            // Минимальная анимация — просто звук и дрожание
            PlaySound(swingSound, transform.position);
            if (enableScreenShake) StartCoroutine(ScreenShake());
            isSwinging = false;
        }
    }

    private IEnumerator SwingAnimationCoroutine(bool hitSomething)
    {
        // Проигрываем звук замаха
        if (swingSound != null) audioSource.PlayOneShot(swingSound);
        
        // Фаза 1: замах (кирка уходит назад)
        float elapsed = 0f;
        while (elapsed < swingDuration * 0.4f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (swingDuration * 0.4f);
            
            // Простая анимация поворота кирки
            pickaxeTransform.localRotation = Quaternion.Slerp(
                pickaxeDefaultLocalRotation,
                pickaxeDefaultLocalRotation * Quaternion.Euler(-30f, 20f, -15f),
                t
            );
            yield return null;
        }
        
        // Фаза 2: удар (кирка резко вперёд)
        elapsed = 0f;
        Vector3 hitPosition = pickaxeDefaultLocalPosition + new Vector3(0.1f, -0.2f, 0.3f);
        
        while (elapsed < swingDuration * 0.6f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (swingDuration * 0.6f);
            
            pickaxeTransform.localPosition = Vector3.Lerp(
                pickaxeTransform.localPosition,
                hitPosition,
                t * 2f
            );
            
            pickaxeTransform.localRotation = Quaternion.Slerp(
                pickaxeTransform.localRotation,
                pickaxeDefaultLocalRotation * Quaternion.Euler(45f, -10f, 10f),
                t * 2f
            );
            yield return null;
        }
        
        // Если попали в глыбу — проигрываем звук разрушения и частицы
        if (hitSomething && breakSound != null)
        {
            audioSource.PlayOneShot(breakSound);
        }
        
        // Дрожание камеры
        if (enableScreenShake) StartCoroutine(ScreenShake());
        
        // Фаза 3: возврат в исходное положение
        elapsed = 0f;
        while (elapsed < swingDuration * 0.7f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (swingDuration * 0.7f);
            
            pickaxeTransform.localPosition = Vector3.Lerp(
                pickaxeTransform.localPosition,
                pickaxeDefaultLocalPosition,
                t
            );
            
            pickaxeTransform.localRotation = Quaternion.Slerp(
                pickaxeTransform.localRotation,
                pickaxeDefaultLocalRotation,
                t
            );
            yield return null;
        }
        
        // Восстанавливаем точное исходное положение
        pickaxeTransform.localPosition = pickaxeDefaultLocalPosition;
        pickaxeTransform.localRotation = pickaxeDefaultLocalRotation;
        
        isSwinging = false;
    }

    private IEnumerator ResetSwingState(float delay)
    {
        yield return new WaitForSeconds(delay);
        isSwinging = false;
    }

    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;
        Vector3 originalPosition = playerCamera.transform.localPosition;
        
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            
            // Случайное смещение для дрожания
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            
            playerCamera.transform.localPosition = originalPosition + new Vector3(x, y, 0f);
            
            yield return null;
        }
        
        // Возвращаем камеру на место
        playerCamera.transform.localPosition = originalPosition;
    }

    private void PlaySound(AudioClip clip, Vector3 position)
    {
        if (clip == null) return;
        AudioSource.PlayClipAtPoint(clip, position, 0.8f);
    }

    // Визуализация луча атаки в сцене (для отладки)
    private void OnDrawGizmos()
    {
        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCamera.transform.position, 
            playerCamera.transform.position + playerCamera.transform.forward * attackRange);
    }
}