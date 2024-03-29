﻿using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ZenvaSurvival.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Move")] public float moveSpeed;
        private Vector2 curMovementInput;
        public float jumpForce;
        public LayerMask groundLayerMask;

        [Header("Look")] public Transform cameraContainer;
        public float minXLook, maxXLook;
        private float camCurXRot;
        public float lookSensitivity;

        private Vector2 mouseDelta;

        [HideInInspector]
        public bool canLook = true;

        //components
        private Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Move();
        }

        private void LateUpdate()
        {
            if (canLook)
                CameraLook();
        }

        void Move()
        {
            Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
            dir *= moveSpeed;
            dir.y = _rigidbody.velocity.y;

            _rigidbody.velocity = dir;
        }

        void CameraLook()
        {
            camCurXRot += mouseDelta.y * lookSensitivity;
            camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
            cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

            transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
        }

        public void OnLookInput(InputAction.CallbackContext context)
        {
            mouseDelta = context.ReadValue<Vector2>();
        }

        public void OnMoveInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                curMovementInput = context.ReadValue<Vector2>();
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                curMovementInput = Vector2.zero;
            }
        }

        public void OnJumpInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                if (IsGrounded())
                {
                    _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                }
            }
        }

        bool IsGrounded()
        {
            Ray[] rays = new Ray[4]
            {
                new Ray(transform.position + (transform.forward * .2f) + (Vector3.up * .01f), Vector3.down),
                new Ray(transform.position + (-transform.forward * .2f) + (Vector3.up * .01f), Vector3.down),
                new Ray(transform.position + (transform.right * .2f) + (Vector3.up * .01f), Vector3.down),
                new Ray(transform.position + (-transform.right * .2f) + (Vector3.up * .01f), Vector3.down)
            };

            for (int i = 0; i < rays.Length; i++)
            {
                if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
                {
                    return true;
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + (transform.forward * .2f), Vector3.down);
            Gizmos.DrawRay(transform.position + (-transform.forward * .2f), Vector3.down);
            Gizmos.DrawRay(transform.position + (transform.right * .2f), Vector3.down);
            Gizmos.DrawRay(transform.position + (-transform.right * .2f), Vector3.down);
        }

        public void ToggleCursor(bool toggle)
        {
            Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
            canLook = !toggle;
        }
    }
}