using UnityEngine;
using System.Collections;

public class PlayerMiner : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private float miningRange = 5f;
    [SerializeField] private int damagePerHit = 10;
    [SerializeField] private float hitCooldown = 0.8f; // ⬅️ Кулдаун между ударами
    
    private const string ANIM_ATTACK = "Mine";
    private bool isMining = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMining)
        {
            StartCoroutine(MineSequence());
        }
    }

    private IEnumerator MineSequence()
    {
        isMining = true;
        
        // Запуск анимации
        animator.SetTrigger(ANIM_ATTACK);

        // ⬅️ Момент нанесения урона (подгоните под вашу анимацию)
        yield return new WaitForSeconds(0.2f);
        
        ApplyDamage();

        // ⬅️ Ждём кулдаун перед следующим ударом
        yield return new WaitForSeconds(hitCooldown);
        
        isMining = false;
    }

    private void ApplyDamage()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        
        if (Physics.Raycast(ray, out RaycastHit hit, miningRange))
        {
            if (hit.collider.TryGetComponent(out StonePile stonePile))
            {
                stonePile.TakeDamage(damagePerHit);
            }
        }
    }
}