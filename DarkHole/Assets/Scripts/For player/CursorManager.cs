using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }
    
    // Счётчик открытых интерфейсов (поддержка нескольких окон)
    private static int _openUiCount = 0;
    
    [Header("Settings")]
    [SerializeField] private bool pauseGameWhenUiOpen = true;
    [SerializeField] private CursorLockMode defaultLockState = CursorLockMode.Locked;
    [SerializeField] private bool defaultCursorVisible = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Применяем настройки курсора при старте
        ApplyCursorState();
    }

    // 🔹 Вызывать при открытии любого UI (плавильня, инвентарь, меню)
    public static void OpenUI()
    {
        _openUiCount++;
        Instance?.ApplyCursorState();
    }

    // 🔹 Вызывать при закрытии любого UI
    public static void CloseUI()
    {
        _openUiCount = Mathf.Max(0, _openUiCount - 1);
        Instance?.ApplyCursorState();
    }

    // 🔹 Проверка: открыт ли сейчас хоть один UI
    public static bool IsAnyUiOpen => _openUiCount > 0;

    // 🔹 Применить состояние курсора и времени
    private void ApplyCursorState()
    {
        if (_openUiCount > 0)
        {
            // UI открыт → показываем курсор, пауза
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            if (pauseGameWhenUiOpen)
            {
                Time.timeScale = 0f;
                Time.fixedDeltaTime = 0f;
            }
        }
        else
        {
            // Все UI закрыты → скрываем курсор, игра идёт
            Cursor.visible = defaultCursorVisible;
            Cursor.lockState = defaultLockState;
            
            if (pauseGameWhenUiOpen)
            {
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f; // Стандарт для 50 FPS физики
            }
        }
    }

    // 🔹 Публичный метод для принудительной настройки (если нужно)
    public static void SetDefaults(CursorLockMode lockState, bool visible)
    {
        if (Instance != null)
        {
            Instance.defaultLockState = lockState;
            Instance.defaultCursorVisible = visible;
            if (!IsAnyUiOpen)
                Instance.ApplyCursorState();
        }
    }
}