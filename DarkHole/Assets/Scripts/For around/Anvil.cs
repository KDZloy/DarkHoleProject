using UnityEngine;

public class Anvil : MonoBehaviour
{
    [Header("⚙️ Настройки")]
    [SerializeField] private float interactionRange = 2.5f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject anvilUI;

    [Header("🎯 Игрок")]
    [SerializeField] private LayerMask playerLayer;
    
    private bool _isUIOpen = false;
    private Transform _playerTransform;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            _playerTransform = player.transform;

        if (anvilUI != null)
            anvilUI.SetActive(false);
    }

    private void Update()
    {
        if (_playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, _playerTransform.position);

        if (distance <= interactionRange)
        {
            if (Input.GetKeyDown(interactKey) && !_isUIOpen)
            {
                OpenAnvil();
            }
            else if (Input.GetKeyDown(interactKey) && _isUIOpen)
            {
                CloseAnvil();
            }
        }
        else if (_isUIOpen)
        {
            CloseAnvil();
        }
    }

    private void OpenAnvil()
    {
        _isUIOpen = true;
        if (anvilUI != null)
        {
            anvilUI.SetActive(true);
            AnvilUIManager.Instance?.UpdateAllTabs();
        }
        CursorManager.OpenUI();
    }

    private void CloseAnvil()
    {
        _isUIOpen = false;
        if (anvilUI != null)
            anvilUI.SetActive(false);
        CursorManager.CloseUI();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}