using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Moving : MonoBehaviour
{
    [Header("Movement")]
    public float currentSpeed;
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;

    [Header("Physics")]
    [SerializeField] private float gravity = 15f;
    [SerializeField] private float jumpForce = 6f;

    [Header("Audio")]
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioClip jumpStrainClip;
    [SerializeField, Range(0, 1)] private float jumpVolume = 0.4f;

    [Header("References")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CinemachineCamera _cinCam;
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AudioSource sfxAudio;

    private Vector2 _move;
    private Vector3 _velocity;
    private bool _jumpInput;

    private void Start()
    {
        currentSpeed = walkSpeed;

        // 🔹 Инициализация курсора через CursorManager (не напрямую!)
        if (CursorManager.Instance == null)
        {
            // Если менеджера нет — создаём базовое поведение
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Настройка источника шагов
        if (!footstepAudio) footstepAudio = GetComponent<AudioSource>();
        footstepAudio.playOnAwake = false;
        footstepAudio.loop = true;
        footstepAudio.spatialBlend = 1f;

        // Настройка источника для одноразовых звуков
        if (!sfxAudio)
        {
            sfxAudio = gameObject.AddComponent<AudioSource>();
            sfxAudio.playOnAwake = false;
            sfxAudio.spatialBlend = 1f;
        }
    }

    #region Input System
    public void OnMove(InputValue val) => _move = val.Get<Vector2>();

    public void OnSprint(InputValue val)
    {
        bool isSprinting = val.Get<float>() > 0.5f;
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        if (footstepAudio.isPlaying)
            footstepAudio.pitch = Mathf.Clamp(currentSpeed / walkSpeed, 0.85f, 1.2f);
    }

    public void OnJump(InputValue val)
    {
        if (val.Get<float>() > 0.5f) _jumpInput = true;
    }
    #endregion

    private void Update()
    {
        // 🔹 Если открыт любой UI — отключаем управление персонажем
        if (CursorManager.IsAnyUiOpen)
        {
            // Сбрасываем ввод, чтобы персонаж не двигался "сам по себе"
            _move = Vector2.zero;
            return;
        }

        // 1. Проверка земли
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = 0f;
        }

        // 2. Прыжок
        if (_jumpInput && _characterController.isGrounded)
        {
            _velocity.y = jumpForce;
            PlayJumpStrain();
        }
        _jumpInput = false;

        // 3. Гравитация
        _velocity.y -= gravity * Time.deltaTime;

        // 4. Горизонтальное движение (относительно камеры)
        Vector3 moveDirection = GetForward() * _move.y + GetRight() * _move.x;
        moveDirection = moveDirection.normalized * currentSpeed;
        moveDirection.y = _velocity.y;

        // 5. Применяем движение
        _characterController.Move(moveDirection * Time.deltaTime);

        // 6. Логика звуков шагов
        bool isMoving = _move.magnitude > 0.1f && _characterController.isGrounded;

        if (isMoving && !footstepAudio.isPlaying)
        {
            footstepAudio.clip = footstepClip;
            footstepAudio.pitch = Mathf.Clamp(currentSpeed / walkSpeed, 0.85f, 1.2f);
            footstepAudio.Play();
        }
        else if (!isMoving && footstepAudio.isPlaying)
        {
            footstepAudio.Stop();
        }
    }

    private void PlayJumpStrain()
    {
        if (jumpStrainClip && sfxAudio)
        {
            sfxAudio.pitch = Random.Range(0.9f, 1.1f);
            sfxAudio.PlayOneShot(jumpStrainClip, jumpVolume);
        }
    }

    private Vector3 GetForward()
    {
        Vector3 forward = _cinCam.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetRight()
    {
        Vector3 right = _cinCam.transform.right;
        right.y = 0;
        return right.normalized;
    }
}