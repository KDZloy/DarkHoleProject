using UnityEngine;

public class PlayerMiner : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;   // Перетащите сюда Main Camera
    [SerializeField] private Animator animator;      // Аниматор кирки
    [SerializeField] private float miningRange = 5f;
    [SerializeField] private int damagePerHit = 10;

    private const string ANIM_ATTACK = "Mine"; // Имя триггера в Animator

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Запуск анимации
            animator.SetTrigger(ANIM_ATTACK);

            // Raycast из центра экрана
            Ray ray = playerCamera.ViewportPointToRay(Vector2.one * 0.5f); // Центр экрана
            if (Physics.Raycast(ray, out RaycastHit hit, miningRange))
            {
                // Проверяем, попали ли в каменную кучу
                if (hit.collider.TryGetComponent(out StonePile stonePile))
                {
                    stonePile.TakeDamage(damagePerHit);
                }
            }
        }
    }
}