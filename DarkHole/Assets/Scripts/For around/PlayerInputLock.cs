using UnityEngine;

public class PlayerInputLock : MonoBehaviour
{
    public static PlayerInputLock Instance { get; private set; }

    private bool _isInputLocked = false;

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
        }
    }

    public void LockInput(bool locked)
    {
        _isInputLocked = locked;
        
        if (locked)
        {
            CursorManager.OpenUI();
        }
        else
        {
            CursorManager.CloseUI();
        }
        
        Debug.Log($"🔒 Управление: {(locked ? "Заблокировано" : "Разблокировано")}");
    }

    public bool IsInputLocked() => _isInputLocked;

    private void Update()
    {
        // Автоматическая разблокировка если нет активных UI
        if (_isInputLocked && !CursorManager.IsAnyUiOpen)
        {
            LockInput(false);
        }
    }
}
