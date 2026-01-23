
using UnityEngine;

public class Digging : MonoBehaviour
{
    [SerializeField] private Animator animator;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ЛКМ — стандартный Input (работает даже с Cinemachine)
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Click");
        }
    }
}