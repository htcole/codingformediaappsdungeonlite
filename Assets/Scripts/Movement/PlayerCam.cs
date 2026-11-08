using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    public Transform orientation;

    float xRotation;
    float yRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Check if mouse exists to prevent null reference errors
        if (Mouse.current == null) return;

        // Read mouse delta values using the new Input System
        // Note: Mouse delta is already raw pixel movement, so we don't need GetAxisRaw strings
        float mouseX = Mouse.current.delta.x.ReadValue() * Time.deltaTime * sensX;
        float mouseY = Mouse.current.delta.y.ReadValue() * Time.deltaTime * sensY;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
