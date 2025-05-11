using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
public class PlayerController1 : MonoBehaviour
{
    [Header("Движение")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("Управление мышкой")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    [Header("Настройки приседания")]
    [SerializeField] private float crouchHeight = 0.29f;
    [SerializeField] private float crouchSpeed = 2f;
    [SerializeField] private float crouchSmoothTime = 0.1f;

    public static PlayerController1 instance;
    public Rigidbody rb;
    public CharacterController controller;
    private Vector3 velocity;
    private float xRotation;
    private bool isGrounded;
    private Animator animator;
    private float originalHeight;
    private float currentHeight;
    private float velo_city;
    private bool isCrouching;
    private float currentSpeed;
    public int health = 100;

    void Awake()
    {
        instance = this;
        health = this.health;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        animator = GetComponent<Animator>();
        originalHeight = controller.height;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleCrouchInput();
        SmoothCrouch();
        if (controller.isGrounded)
        {
            animator.SetBool("ground", true);
        }
        else
        {
            animator.SetBool("ground", false);
        }
        if (health <= 0)
        {
            health = 0;
            Debug.Log("мэд ка один ди");
        }
    }

    void HandleMovement()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // умные анимки блять
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                SetAnimStates(false, false, true, false, false);
            }
            else
            {
                if (Input.GetKey(KeyCode.A)) SetAnimStates(false, false, false, true, false);
                else if (Input.GetKey(KeyCode.D)) SetAnimStates(false, false, false, false, true);
                else SetAnimStates(false, true, false, false, false);
            }
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) SetAnimStates(false, false, false, true, false);
            else if (Input.GetKey(KeyCode.D)) SetAnimStates(false, false, false, false, true);
            else SetAnimStates(true, false, false, false, false);
        }
        if (!isCrouching) 
        {
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        }
        Vector3 move = transform.right * -z + transform.forward * x;
        controller.Move(move * currentSpeed * Time.deltaTime);
        if (Input.GetButton("Jump")) animator.SetTrigger("jmp_start");
        else animator.SetBool("jmp_start", false);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, -90f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    void SetAnimStates(bool idle, bool walki, bool bega, bool vleva, bool vpravo)
    {
        animator.SetBool("idle", idle);
        animator.SetBool("walki", walki);
        animator.SetBool("bega", bega);
        animator.SetBool("vleva", vleva);
        animator.SetBool("vpravo", vpravo);
    }
    void HandleCrouchInput()
    {
        // Приседание по нажатию LeftCtrl
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            isCrouching = true;
            currentSpeed = crouchSpeed; // Меняем скорость движения
            animator.SetBool("ctrl", true);
        }

        // Выход из приседа при отпускании
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            isCrouching = false;
            currentSpeed = walkSpeed;
            animator.SetBool("ctrl", false);
        }
    }

    void SmoothCrouch()
    {
        // Плавно меняем высоту контроллера
        float targetHeight = isCrouching ? crouchHeight : originalHeight;
        controller.height = Mathf.SmoothDamp(
            controller.height,
            targetHeight,
            ref velo_city,
            crouchSmoothTime
        );

        // Корректируем центр коллайдера
        Vector3 newCenter = controller.center;
        newCenter.y = controller.height * 0.5f;
        controller.center = newCenter;
    }
}