using UnityEngine;
using System.Collections;

public class PlayerMiner : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private float miningRange = 5f;
    [SerializeField] private int damagePerHit = 10;

    private const string ANIM_ATTACK = "Mine";
    
    // Флаг блокировки повторного нажатия
    private bool isMining = false;

    void Update()
    {
        // Проверяем нажатие и доступность действия
        if (Input.GetMouseButtonDown(0) && !isMining)
        {
            isMining = true;

            // Запуск анимации (срабатывает сразу)
            animator.SetTrigger(ANIM_ATTACK);

            // Запуск корутины с задержкой удара (1.5 сек)
            StartCoroutine(MineWithDelay());
        }
    }

    private IEnumerator MineWithDelay()
    {
        // Ждем 1.5 секунды (время анимации)
        yield return new WaitForSeconds(1.5f);

        // Нанесение урона ПОСЛЕ задержки
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        
        if (Physics.Raycast(ray, out RaycastHit hit, miningRange))
        {
            if (hit.collider.TryGetComponent(out StonePile stonePile))
            {
                stonePile.TakeDamage(damagePerHit);
            }
        }

        // Разблокировка возможности майнить снова
        isMining = false;
    }
}