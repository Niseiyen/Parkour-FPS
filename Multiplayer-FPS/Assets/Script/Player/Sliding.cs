using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerObj;
    [SerializeField] private Transform cameraTransform; 
    private Rigidbody rb;
    private Movement movement;

    [Header("Sliding")]
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float slideForce;
    private float slideTimer;

    [SerializeField] private float slideYScale;
    private float startYScale;

    [Header("Camera Adjustments")]
    [SerializeField] private float cameraSlideOffset = 0.5f; 
    private Vector3 originalCameraPosition;

    [Header("Input")]
    private KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();

        startYScale = playerObj.localScale.y;
        originalCameraPosition = cameraTransform.localPosition; 
    }

    private void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide();

        if (Input.GetKeyUp(slideKey) && movement.sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (movement.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        movement.sliding = true;

        // Réduit la taille du joueur
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // Abaisse la caméra
        Vector3 newCameraPosition = originalCameraPosition;
        newCameraPosition.y -= cameraSlideOffset;
        cameraTransform.localPosition = newCameraPosition;

        slideTimer = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = gameObject.transform.forward * verticalInput + gameObject.transform.right * horizontalInput;

        // Sliding normal
        if (!movement.OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }
        // Sliding down a slope
        else
        {
            rb.AddForce(movement.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
            StopSlide();
    }

    private void StopSlide()
    {
        movement.sliding = false;

        // Restaure la taille du joueur
        playerObj.localScale = new Vector3(playerObj.localScale.x, startYScale, playerObj.localScale.z);

        // Remonte la caméra
        cameraTransform.localPosition = originalCameraPosition;
    }
}
