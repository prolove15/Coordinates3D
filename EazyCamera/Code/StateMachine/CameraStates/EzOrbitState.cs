using UnityEngine;
using System.Collections;
using System;

namespace EazyCamera.Legacy
{
    [System.Serializable]
    public class EzOrbitState : EzCameraState
    {
        private float _rotY = 0; // Camera's current rotation around the X axis (up/down)
        private float _rotX = 0; // Camera's current rotation around the Y axis (left/right)

        private float _horizontalInput = 0;
        private float _verticalInput = 0;

        Quaternion _destRot = Quaternion.identity;

        public EzOrbitState(EzCamera camera, EzCameraSettings settings)
            : base(camera, settings) { }


        public override void EnterState()
        {
            Vector3 rotation = _controlledCamera.CameraTransform.rotation.eulerAngles;
            _rotY = rotation.y;
            _rotX = rotation.x;
        }

        public override void ExitState()
        {
            //
        }

        public override void UpdateState()
        {
            if (_controlledCamera.OribtEnabled)
            {
                HandleInput();
            }
        }

        public override void LateUpdateState()
        {
            OrbitCamera(_horizontalInput, _verticalInput);
        }

        public override void UpdateStateFixed()
        {
            //
        }

        private void OrbitCamera(float horz, float vert)
        {
            // cache the step and update the roation from input
            float step = Time.deltaTime * _stateSettings.RotateSpeed;
            _rotY += horz * step;
            _rotX += vert * step;
            _rotX = Mathf.Clamp(_rotX, _stateSettings.MinRotX, _stateSettings.MaxRotX);

            // compose the quaternions from Euler vectors to get the new rotation
            Quaternion addRot = Quaternion.Euler(0f, _rotY, 0f);
            _destRot = addRot * Quaternion.Euler(_rotX, 0f, 0f); // Not commutative

            _cameraTransform.rotation = _destRot;

#if UNITY_EDITOR
            Debug.DrawLine(_cameraTransform.position, _cameraTarget.transform.position, Color.red);
            Debug.DrawRay(_cameraTransform.position, _cameraTransform.forward, Color.blue);
#endif

            _controlledCamera.UpdatePosition();
        }

        public override void HandleInput()
        {
            if (_controlledCamera.OribtEnabled && Input.GetMouseButtonDown(0))
            {
                _controlledCamera.SetState(State.ORBIT);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _controlledCamera.SetState(State.FOLLOW);
                return;
            }

            // cache the inputs
            float horz = Input.GetAxis(MOUSEX);
            float vert = Input.GetAxis(MOUSEY);

            _horizontalInput = horz;
            _verticalInput = vert;
        }
    }
}
