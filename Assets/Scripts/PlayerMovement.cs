using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Xml;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    // Tunables
    [Header("Ship Properties")]
    [Tooltip("in m/s")][SerializeField] float xSpeed = 100.0f;
    [Tooltip("in m/s")][SerializeField] float ySpeed = 100.0f;
    [Header("Movement Constraints")]
    [SerializeField] float xPadding = 1.5f;
    [SerializeField] float yPadding = 1f;
    [Header("Rotation Constraints")]
    [SerializeField] float pitchOffset = -5f;
    [SerializeField] float pitchMagnitude = 20f;
    [SerializeField] float yawMagnitude = 30f;
    [SerializeField] float rotatePitchHigh = 5f;
    [SerializeField] float rotatePitchLow = -20f;
    [Header("Movement-Induced Rotations")]
    [SerializeField] float movementToRotationFactor = 60.0f;
    [SerializeField] float movementRotationDeadzone = 1f;
    [SerializeField] float movementRotationEdgeThrottle = 0.1f;
    [SerializeField] float movementRotationLerpFactor = 0.2f;

    // Fixed variables
    Vector3 cornerBottomLeft;
    Vector3 cornerTopRight;
    Vector3 midPoint;

    // State
    float horizontal = 0f;
    float vertical = 0f;
    float xMin = 0f;
    float xMax = 0f;
    float yMin = 0f;
    float yMax = 0f;
    float lastMovementPitch = 0f;
    float lastMovementRotate = 0f;
    bool movementEnabled = true;

    // Cached References
    Camera gameCamera;

    private void Start()
    {
        gameCamera = Camera.main;
        SetMoveBoundaries();
    }

    void Update()
    {
        ProcessMovementInput();
    }

    private void ProcessMovementInput()
    {
        float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal");
        float verticalThrow = CrossPlatformInputManager.GetAxis("Vertical");
        horizontal = horizontalThrow * xSpeed * Time.deltaTime;
        vertical = verticalThrow * ySpeed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!movementEnabled) { return; }
        Move();
        Rotate();
    }

    private void Move()
    {
        Vector3 positionShift = ReturnWindowClampedVector(horizontal, vertical);
        transform.localPosition = positionShift;
    }

    private void Rotate()
    {
        float pitch = pitchOffset - pitchMagnitude * transform.localPosition.y / yMax;
        float yaw = yawMagnitude * transform.localPosition.x / xMax;
        float rotateFactor = Mathf.Abs((yaw / yawMagnitude) * ((pitch - pitchOffset) / pitchMagnitude));

        float rotate = 0; // rotation compensation dependent on yaw/pitch inputs vs. camera rendering
        if (pitch > 0 && yaw < 0) { rotate = rotatePitchLow * rotateFactor; }
        else if (pitch < 0 && yaw < 0) { rotate = rotatePitchHigh * rotateFactor; }
        else if (pitch > 0 && yaw > 0) { rotate = -rotatePitchLow * rotateFactor; }
        else if (pitch < 0 && yaw > 0) { rotate = -rotatePitchHigh * rotateFactor; }

        float movementPitch, movementRotate;
        GetMovementInducedRotations(out movementPitch, out movementRotate);

        transform.localRotation = Quaternion.Euler(pitch + movementPitch, yaw, rotate + movementRotate);
    }

    private void GetMovementInducedRotations(out float movementPitch, out float movementRotate)
    {
        movementPitch = -vertical * movementToRotationFactor;
        movementPitch = Mathf.Lerp(lastMovementPitch, movementPitch, movementRotationLerpFactor);
        if (yMax - Mathf.Abs(transform.localPosition.y) < movementRotationDeadzone) { movementPitch = movementRotationEdgeThrottle * movementPitch; }
        lastMovementPitch = movementPitch;

        movementRotate = -horizontal * movementToRotationFactor * 2;
        movementRotate = Mathf.Lerp(lastMovementRotate, movementRotate, movementRotationLerpFactor);
        if (xMax - Mathf.Abs(transform.localPosition.x) < movementRotationDeadzone) { movementRotate = movementRotationEdgeThrottle * movementRotate; }
        lastMovementRotate = movementRotate;
    }

    private Vector3 ReturnWindowClampedVector(float xInput, float yInput)
    {
        float xClamped = Mathf.Clamp(transform.localPosition.x + xInput, xMin, xMax);
        float yClamped = Mathf.Clamp(transform.localPosition.y + yInput, yMin, yMax);
        Vector3 clampedVector = new Vector3(xClamped, yClamped, transform.localPosition.z);
        return clampedVector;
    }

    private void SetMoveBoundaries()
    {
        cornerBottomLeft = new Vector3(0f, 0f, transform.localPosition.z);
        cornerTopRight = new Vector3(1f, 1f, transform.localPosition.z);
        midPoint = new Vector3(0.5f, 0.5f, transform.localPosition.z);

        xMin = gameCamera.ViewportToWorldPoint(cornerBottomLeft).x - gameCamera.ViewportToWorldPoint(midPoint).x + xPadding;
        yMin = gameCamera.ViewportToWorldPoint(cornerBottomLeft).y - gameCamera.ViewportToWorldPoint(midPoint).y + yPadding;
        xMax = gameCamera.ViewportToWorldPoint(cornerTopRight).x - gameCamera.ViewportToWorldPoint(midPoint).x - xPadding;
        yMax = gameCamera.ViewportToWorldPoint(cornerTopRight).y - gameCamera.ViewportToWorldPoint(midPoint).y - yPadding;
    }

    public void EnableDisableMovement(bool setMovement)
    {
        movementEnabled = setMovement;
    }
}
