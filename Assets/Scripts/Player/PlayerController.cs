using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZenvaSurvival.Player
{
    public class PlayerController : MonoBehaviour
    {
        public Transform cameraContainer;
        public float minXLook, maxXLook;
        private float camCurXRot;
        public float lookSensitivity;

        private Vector2 mouseDelta;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void LateUpdate()
        {
            CameraLook();
        }

        void CameraLook()
        {
            camCurXRot += mouseDelta.y * lookSensitivity;
            camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
            cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

            transform.eulerAngles += new Vector3(0, mouseDelta.x - lookSensitivity, 0);
        }

        public void OnLookInput(InputAction.CallbackContext context)
        {
            mouseDelta = context.ReadValue<Vector2>();
        }
    }
}