using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections; // ← обязательно!

public class Doom : MonoBehaviour
{
    public Button button;

    public void OnButtonClick()
    {
        // Запускаем корутину для последовательной анимации
        StartCoroutine(ScaleButtonSequence());
    }

    IEnumerator ScaleButtonSequence()
    {
        // Увеличиваем масштаб до 2 за 1 секунду
        button.transform.DOScale(2f, 1f).SetEase(Ease.OutQuad); // опционально: плавность
        yield return new WaitForSeconds(1f);

        // Уменьшаем масштаб обратно до 1 за 2 секунды
        button.transform.DOScale(1f, 2f).SetEase(Ease.OutQuad);
    }
}