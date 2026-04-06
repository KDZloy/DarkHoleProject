using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Moving : MonoBehaviour
{
    public float currentSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    
    [SerializeField] private float gravity = 15f;
    [SerializeField] private float jumpForce = 6f;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CinemachineCamera _cinCam;
    
    // Аудио
    [SerializeField] private AudioClip footstepClip;
    [SerializeField] private AudioSource footstepAudio;
    [SerializeField] private AudioClip jumpStrainClip; // Звук напряжения
    [SerializeField] private AudioSource sfxAudio;     // Отдельный источник для прыжков/приземлений

    private Vector2 _move;
    private Vector3 _velocity;
    private bool _jumpInput;

    private void Start()
    {
        currentSpeed = walkSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        if (!footstepAudio) footstepAudio = GetComponent<AudioSource>();
        footstepAudio.playOnAwake = false;
        footstepAudio.loop = true;

        // Если источник для SFX не назначен, создаём его автоматически
        if (!sfxAudio)
        {
            sfxAudio = gameObject.AddComponent<AudioSource>();
            sfxAudio.playOnAwake = false;
            sfxAudio.spatialBlend = 1f; // 3D звук
            sfxAudio.priority = 128;    // Средний приоритет
        }
    }

    public void OnMove(InputValue val) => _move = val.Get<Vector2>();
    
    public void OnSprint(InputValue val)
    {
        bool isSprinting = val.Get<float>() > 0.5f;
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        
        if (footstepAudio.isPlaying)
            footstepAudio.pitch = Mathf.Clamp(currentSpeed / walkSpeed, 0.85f, 1.2f);
    }

    public void OnJump(InputValue val) { if (val.Get<float>() > 0.5f) _jumpInput = true; }

    private void Update()
    {
        // Проверка земли
        if (_characterController.isGrounded && _velocity.y < 0) _velocity.y = 0f;
        
        // Прыжок
        if (_jumpInput && _characterController.isGrounded)
        {
            _velocity.y = jumpForce;
            PlayJumpStrain(); // 🔊 Воспроизводим звук напряжения
        }
        _jumpInput = false;
        _velocity.y -= gravity * Time.deltaTime;

        // Движение
        Vector3 moveDirection = GetForward() * _move.y + GetRight() * _move.x;
        moveDirection = moveDirection.normalized * currentSpeed;
        moveDirection.y = _velocity.y;
        _characterController.Move(moveDirection * Time.deltaTime);

        // Шаги
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
            // Небольшой рандом питча, чтобы звук не "мозолил уши" однообразием
            sfxAudio.pitch = Random.Range(0.9f, 1.1f); 
            sfxAudio.PlayOneShot(jumpStrainClip, 0.8f); // Громкость 0.8
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