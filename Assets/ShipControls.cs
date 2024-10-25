using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShipControls : MonoBehaviour
{
    public float forwardSpeed = 25f, strafeSpeed = 7.5f, hoverSpeed = 5f;
    private float activeForwardSpeed, activeStrafeSpeed, activeHoverSpeed;
    private float forwardAcceleration = 2.5f, strafeAcceleration = 2f, hoverAcceleration = 2f;

    public float lookRateSpeed = 90f;
    private Vector2 lookInput, screenCenter, mouseDistance;

    private float rollInput;
    public float rollSpeed = 45f, rollAcceleration = 3.5f;

    public float rotationSpeed = 50f;

    // Boost variables
    public float boostMultiplier = 2f;
    public float boostDuration = 5f;
    public float boostCooldown = 10f;
    private float boostTimeRemaining;
    private float boostCooldownRemaining;
    private bool isBoosting;

    // UI Text for displaying speed
    public TMP_Text speedText;

    // Camera shake variables
    public CameraShake cameraShake; // Reference to the CameraShake script
    public float shakeMagnitude = 0.1f; // Magnitude of shake while moving
    public float boostShakeMagnitude = 0.3f; // Magnitude of shake while boosting

    void Start()
    {
        screenCenter.x = Screen.width * .5f;
        screenCenter.y = Screen.height * .5f;

        Cursor.lockState = CursorLockMode.Confined;

        boostTimeRemaining = boostDuration;
        boostCooldownRemaining = 0f;
        isBoosting = false;

        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    void FixedUpdate()
    {
        HandleBoost();

        // Mouse-based rotation
        lookInput.x = Input.mousePosition.x;
        lookInput.y = Input.mousePosition.y;

        mouseDistance.x = (lookInput.x - screenCenter.x) / screenCenter.y;
        mouseDistance.y = (lookInput.y - screenCenter.y) / screenCenter.y;

        mouseDistance = Vector2.ClampMagnitude(mouseDistance, 1f);

        rollInput = Mathf.Lerp(rollInput, Input.GetAxisRaw("Roll"), rollAcceleration * Time.fixedDeltaTime);

        transform.Rotate(-mouseDistance.y * lookRateSpeed * Time.fixedDeltaTime, mouseDistance.x * lookRateSpeed * Time.fixedDeltaTime, rollInput * rollSpeed * Time.fixedDeltaTime, Space.Self);

        // Apply boost to speeds if active
        float currentForwardSpeed = isBoosting ? forwardSpeed * boostMultiplier : forwardSpeed;
        float currentStrafeSpeed = isBoosting ? strafeSpeed * boostMultiplier : strafeSpeed;
        float currentHoverSpeed = isBoosting ? hoverSpeed * boostMultiplier : hoverSpeed;

        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, Input.GetAxisRaw("Vertical") * currentForwardSpeed, forwardAcceleration * Time.fixedDeltaTime);
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, Input.GetAxisRaw("Horizontal") * currentStrafeSpeed, strafeAcceleration * Time.fixedDeltaTime);
        activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, Input.GetAxisRaw("Hover") * currentHoverSpeed, hoverAcceleration * Time.fixedDeltaTime);

        transform.position += transform.forward * activeForwardSpeed * Time.fixedDeltaTime;
        transform.position += transform.right * activeStrafeSpeed * Time.fixedDeltaTime;
        transform.position += transform.up * activeHoverSpeed * Time.fixedDeltaTime;

        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Rotate(0, 0, -horizontalInput * rotationSpeed * Time.fixedDeltaTime);

        // Update the speed text on the UI
        speedText.text = "Speed: " + Mathf.Clamp(activeForwardSpeed, 0, 100).ToString("F2") + " / 100";

        // Trigger camera shake based on movement
        if (activeForwardSpeed > 0)
        {
            float shakeAmount = isBoosting ? boostShakeMagnitude : shakeMagnitude;

            // Only shake when the ship is moving
            cameraShake.Shake(Time.fixedDeltaTime, shakeAmount);
        }
    }


    void HandleBoost()
    {
        if (Input.GetKey(KeyCode.LeftShift) && boostTimeRemaining > 0 && boostCooldownRemaining <= 0)
        {
            isBoosting = true;
            boostTimeRemaining -= Time.fixedDeltaTime;

            // Start cooldown once boost time is used up
            if (boostTimeRemaining <= 0)
            {
                boostTimeRemaining = 0; // Ensure it doesn't go negative
                boostCooldownRemaining = boostCooldown;
            }
        }
        else
        {
            isBoosting = false;

            // If cooldown is active, decrease cooldown timer
            if (boostCooldownRemaining > 0)
            {
                boostCooldownRemaining -= Time.fixedDeltaTime;

                // Reset boost time when cooldown is finished
                if (boostCooldownRemaining <= 0)
                {
                    boostCooldownRemaining = 0; // Reset cooldown timer
                    boostTimeRemaining = boostDuration; // Reset boost duration
                }
            }
        }
    }
}
