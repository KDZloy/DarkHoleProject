// PlayerMining.cs
using UnityEngine;

public class PlayerMining : MonoBehaviour
{
    [Header("Настройки кирки")]
    public float pickaxeDamage = 20f;
    public float miningRange = 5f;
    public LayerMask oreLayer; // создай слой "Ore" и назначь кучам

    [Header("Анимация")]
    public Animator pickaxeAnimator; // перетащи сюда Animator кирки

    [Header("Визуальная обратная связь")]
    public float maxAngle = 30f; // максимальный угол отклонения взгляда от центра кучи

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryMine();
        }
    }

    void TryMine()
    {
        // Стреляем лучом из центра экрана
        Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);

        if (Physics.Raycast(ray, out RaycastHit hit, miningRange, oreLayer))
        {
            // Проверяем угол между направлением взгляда и направлением на кучу
            Vector3 toOre = (hit.point - Camera.main.transform.position).normalized;
            float angle = Vector3.Angle(Camera.main.transform.forward, toOre);

            if (angle <= maxAngle)
            {
                // Проигрываем анимацию
                if (pickaxeAnimator != null)
                    pickaxeAnimator.SetTrigger("Click");

                // Наносим урон
                OrePile ore = hit.collider.GetComponent<OrePile>();
                if (ore != null)
                {
                    ore.TakeDamage(pickaxeDamage);
                }
            }
            else
            {
                Debug.Log("Слишком большой угол — не бьём!");
            }
        }
    }
}