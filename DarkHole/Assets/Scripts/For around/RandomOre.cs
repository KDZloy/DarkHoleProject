using UnityEngine;
using System.Collections;

public class RandomOre : MonoBehaviour
{
    // Убедись, что на этом объекте Collider → Is Trigger = true

    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, что это игрок (например, по тегу)
        if (other.CompareTag("Player"))
        {
            StartCoroutine(RollWithDelay());
        }
    }

    IEnumerator RollWithDelay()
    {
        Debug.Log("Контакт с кучей! Ожидание 1 секунды перед выпадением...");
        yield return new WaitForSeconds(1f);

        int chance = Random.Range(1, 101); // 1–100

        string result;
        if (chance <= 40)        // 40%
            result = "Камень";
        else if (chance <= 65)   // +25% → 65
            result = "Медь";
        else if (chance <= 80)   // +15% → 80
            result = "Железо";
        else if (chance <= 90)   // +10% → 90
            result = "Золото";
        else if (chance <= 96)   // +6% → 96
            result = "Алмаз";
        else                     // +4% → 100
            result = "Кобальт";

        Debug.Log("Выпал предмет: " + result);
    }
}