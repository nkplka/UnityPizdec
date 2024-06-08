using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float sprintSpeed = 8.0f;
    public float sensitivityX = 2f;
    public float sensitivityY = 2f;
    public float minimumY = -60f;
    public float maximumY = 60f;
    public float smoothTime = 0.1f;
    public Transform cameraTransform;
    public Transform weaponTransform;

    public float maxStamina = 100f;
    private float currentStamina;
    public float staminaRegenRate = 5f;
    public float staminaUsageRate = 10f;
    public float minStaminaForSprint = 50f;
    public Text staminaText;
    public Text speedText;

    private float rotationX = 0f;
    private float rotationY = 0f;
    private Vector3 currentRotation;
    private Vector3 currentVelocity;
    private Rigidbody rb;
    private bool isGrounded;
    private bool isSprinting;
    private bool canSprint;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        currentStamina = maxStamina;
    }

    void Update()
    {
        MouseLook();
        HandleSprinting();
        UpdateUI();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MouseLook()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        Vector3 targetRotation = new Vector3(-rotationY, rotationX, 0);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref currentVelocity, smoothTime);

        cameraTransform.localEulerAngles = new Vector3(currentRotation.x, 0, 0);
        transform.eulerAngles = new Vector3(0, currentRotation.y, 0);
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        float currentSpeed = isSprinting ? sprintSpeed : speed;

        Vector3 move = cameraTransform.right * moveX + cameraTransform.forward * moveZ;
        move.y = 0;

        rb.velocity = move * currentSpeed;
    }

    void HandleSprinting()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (currentStamina > (maxStamina * minStaminaForSprint / 100))
            {
                canSprint = true;
            }

            if (canSprint && currentStamina > 0)
            {
                isSprinting = true;
                UseStamina(staminaUsageRate * Time.deltaTime);
            }
            else
            {
                isSprinting = false;
            }
        }
        else
        {
            isSprinting = false;
            canSprint = false;
        }

        if (currentStamina < maxStamina && !isSprinting)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }
    }

    void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (currentStamina <= 0)
        {
            isSprinting = false;
            canSprint = false;
        }
    }

    void UpdateUI()
    {
        if (staminaText != null)
        {
            staminaText.text = $"{(currentStamina / maxStamina) * 100:0}%";
        }

        if (speedText != null)
        {
            float horizontalSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
            speedText.text = $"Speed: {horizontalSpeed:0.0} m/s";
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
