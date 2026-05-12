using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("📱 Панели меню (перетащи объекты Panel)")]
    public GameObject mainPanel;
    public GameObject settingsPanel;
    public GameObject audioPanel;
    public GameObject controlsPanel;

    private GameObject currentPanel;

    private void Start()
    {
        // При старте сцены показываем главное меню
        SwitchPanel(mainPanel);
    }

    // 🔹 Универсальный метод переключения
    public void SwitchPanel(GameObject newPanel)
    {
        if (currentPanel != null) 
            currentPanel.SetActive(false);
            
        currentPanel = newPanel;
        if (currentPanel != null) 
            currentPanel.SetActive(true);
            
        Debug.Log($"[Меню] Открыта панель: {currentPanel?.name}");
    }

    // 🎮 Кнопки главного меню
    public void PlayGame()
    {
        Debug.Log("Запуск игры...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OpenSettings() => SwitchPanel(settingsPanel);
    public void BackToMain() => SwitchPanel(mainPanel);
    public void ExitGame()
    {
        Debug.Log("Игра закрылась");
        Application.Quit();
    }

    // ⚙️ Кнопки внутри панели "Настройки"
    public void OpenAudioSettings() => SwitchPanel(audioPanel);
    public void OpenControlSettings() => SwitchPanel(controlsPanel);

    // 🔙 Кнопки "Назад" (работают и для Аудио, и для Управления)
    public void BackToSettings() => SwitchPanel(settingsPanel);
}