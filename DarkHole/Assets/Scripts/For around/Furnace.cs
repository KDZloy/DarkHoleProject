using UnityEngine;

public class Furnace : MonoBehaviour
{
    [Header("⚙️ Настройки")]
    [SerializeField] private float interactionRange = 2.5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject furnaceUI;

    [Header("🎯 Игрок")]
    [SerializeField] private LayerMask playerLayer;
    
    private bool _isUIOpen = false;
    private Transform _playerTransform;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;

        if (furnaceUI != null)
            furnaceUI.SetActive(false);
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, _playerTransform.position);

        if (distance <= interactionRange)
        {
            if (Input.GetKeyDown(interactKey) && !_isUIOpen)
            {
                OpenFurnace();
            }
            else if (Input.GetKeyDown(interactKey) && _isUIOpen)
            {
                CloseFurnace();
            }
        }
        else if (_isUIOpen)
        {
            CloseFurnace();
        }
    }

    private void OpenFurnace()
    {
        _isUIOpen = true;
        if (furnaceUI != null)
        {
            furnaceUI.SetActive(true);
            FurnaceUIManager.Instance?.UpdateRecipes();
        }
        
        // 🔹 Сообщаем менеджеру курсора, что открыли UI
        CursorManager.OpenUI();
    }

    private void CloseFurnace()
    {
        _isUIOpen = false;
        if (furnaceUI != null)
            furnaceUI.SetActive(false);
        
        // 🔹 Сообщаем менеджеру курсора, что закрыли UI
        CursorManager.CloseUI();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}