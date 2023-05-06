using UnityEngine;
using System.Collections;
using System;

namespace EazyCamera.Legacy
{
    [System.Serializable]
    public class EzFollowState : EzCameraState
    {
        private Vector3 _targetPosition = Vector3.zero;

        public EzFollowState(EzCamera camera, EzCameraSettings settings)
            : base(camera, settings)
        {
            //
        }

        //
        public override void EnterState()
        {
            //
        }

        public override void ExitState()
        {
            //
        }

        public override void LateUpdateState()
        {
            if (_controlledCamera != null)
            {
                if (_controlledCamera.FollowEnabled)
                {
                    UpdateCameraPosition();
                    UpdateCameraRotation();
                }
            }
        }

        public override void UpdateState()
        {
            //
        }

        public override void UpdateStateFixed()
        {
            //
        }

        public override void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetBehindPlayer();
            }
        }

        private void UpdateCameraPosition()
        {
            _stateSettings.OffsetDistance = Mathf.MoveTowards(_stateSettings.OffsetDistance, _stateSettings.DesiredDistance, Time.deltaTime * _stateSettings.ZoomSpeed);
            _targetPosition = _cameraTarget.position + ((_cameraTarget.up * _stateSettings.OffsetHeight) + (_cameraTarget.right * _stateSettings.LateralOffset) + (_cameraTransform.forward * -_stateSettings.OffsetDistance));
            _cameraTransform.position = Vector3.Lerp(_cameraTransform.position, _targetPosition, _stateSettings.RotateSpeed * Time.deltaTime);
        }

        private void UpdateCameraRotation()
        {
            Vector3 relativePos = (_cameraTarget.position + (Vector3.right * _stateSettings.LateralOffset) + (Vector3.up * _stateSettings.OffsetHeight)) - _cameraTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(relativePos);
            _cameraTransform.rotation = Quaternion.Lerp(_cameraTransform.rotation, lookRotation, _stateSettings.RotateSpeed * Time.deltaTime);
        }

        public void ResetBehindPlayer()
        {
            _targetPosition = _cameraTarget.position + ((_cameraTarget.up * _stateSettings.OffsetHeight) + (_cameraTarget.right * _stateSettings.LateralOffset) + (_cameraTarget.forward * -_stateSettings.OffsetDistance));
            _cameraTransform.position = _targetPosition;

            Vector3 relativePos = (_cameraTarget.position + (Vector3.right * _stateSettings.LateralOffset) + (Vector3.up * _stateSettings.OffsetHeight)) - _cameraTransform.position;
            Quaternion lookRotation = Quaternion.LookRotation(relativePos);
            _cameraTransform.rotation = lookRotation;
        }
    }
}
