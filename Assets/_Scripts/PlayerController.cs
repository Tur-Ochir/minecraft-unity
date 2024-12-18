using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce = 2;
    [SerializeField] private float mass = 1;
    private Vector3 inputVec;
    private Vector3 movementVec;
    private Vector3 velocity;
    private CharacterController controller;
    private Transform camTransform;
    private bool canFly = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        camTransform = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        inputVec = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        UpdateMovement();
        if (!canFly)
        {
            UpdateGravity();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            canFly = !canFly;
        }
        if (inputVec.magnitude >= 0.01f)
        {
            // RotateToLookDir();
        }
    }

    private void FixedUpdate()
    {
        RotateToLookDir();
    }

    private void UpdateMovement()
    {
        movementVec = camTransform.right * inputVec.x + camTransform.forward * inputVec.z;

        velocity.x = movementVec.x;
        velocity.z = movementVec.z;

        if (canFly)
        {
            velocity.y = movementVec.y;
        }


        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocity.y += jumpForce;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed *= 5f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed /= 5f;
        }

        // controller.SimpleMove(movementVec * speed);
        controller.Move(velocity * speed * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        var gravity = Physics.gravity * mass * Time.deltaTime;
        velocity.y = controller.isGrounded ? -1f : velocity.y + gravity.y;
    }

    private void Jump()
    {
        if (!controller.isGrounded) return;

        movementVec.y += jumpForce;
        Debug.Log("Jump");
    }

    private void RotateToLookDir()
    {
        // float angle;
        Vector3 cameraForwardOnPlane = new Vector3(camTransform.forward.x, 0, camTransform.forward.z).normalized;

        float angle = Vector3.SignedAngle(transform.forward, cameraForwardOnPlane, Vector3.up);
        transform.Rotate(Vector3.up, angle);
    }
}