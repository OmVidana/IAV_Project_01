using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    public float mouseSensitivityX;
    public Rigidbody rb;
    private Vector3 _movement;
    private float rotationY;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.fixedDeltaTime;

        rotationY += mouseX;
        transform.rotation = Quaternion.Euler(0f, rotationY, 0f);
        
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        _movement = (forward * verticalInput + right * horizontalInput).normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + _movement * (speed * Time.fixedDeltaTime));
    }
}
