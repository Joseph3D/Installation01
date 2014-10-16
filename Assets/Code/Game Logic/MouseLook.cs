using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GameLogic
{
    [AddComponentMenu("Camera-Control/Mouse Look")]
    public class MouseLook : MonoBehaviour
    {

        public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
        public RotationAxes Axes = RotationAxes.MouseXAndY;
        public float sensitivityX = 15F;
        public float SensitivityY = 15F;

        public float MinimumX = -360F;
        public float MaximumX = 360F;

        public float MinimumY = -60F;
        public float MaximumY = 60F;

        float RotationX = 0F;
        float RotationY = 0F;

        Quaternion originalRotation;

        void Update()
        {
            if (Axes == RotationAxes.MouseXAndY)
            {
                UpdateMouseLookXY();
            }
            else if (Axes == RotationAxes.MouseX)
            {
                UpdateMouseLookX();
            }
            else
            {
                UpdateMouseLookY();
            }
        }

        private void UpdateMouseLookY()
        {
            RotationY += Input.GetAxis("Mouse Y") * SensitivityY;
            RotationY = ClampAngle(RotationY, MinimumY, MaximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(-RotationY, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }

        private void UpdateMouseLookX()
        {
            RotationX += Input.GetAxis("Mouse X") * sensitivityX;
            RotationX = ClampAngle(RotationX, MinimumX, MaximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(RotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }

        private void UpdateMouseLookXY()
        {
            RotationX += Input.GetAxis("Mouse X") * sensitivityX;
            RotationY += Input.GetAxis("Mouse Y") * SensitivityY;

            RotationX = ClampAngle(RotationX, MinimumX, MaximumX);
            RotationY = ClampAngle(RotationY, MinimumY, MaximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(RotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(RotationY, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }

        void Start()
        {
            // Make the rigid body not change rotation
            if (rigidbody)
                rigidbody.freezeRotation = true;
            originalRotation = transform.localRotation;
        }

        public static float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360F)
                angle += 360F;
            if (angle > 360F)
                angle -= 360F;
            return Mathf.Clamp(angle, min, max);
        }
    }
}