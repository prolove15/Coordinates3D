using UnityEngine;
using System.Collections;

namespace EazyCamera.Legacy
{
    public class EzMotor : MonoBehaviour
    {
        [SerializeField] private float _walkSpeed = 5f;
        [SerializeField] private float _runSpeed = 15f;
        [SerializeField] private float _acceleration = 10f;
        [SerializeField] private float _rotateSpeed = 5f;
        private float _currentSpeed = 5f;
        private float _speedDelta = 0f;

        private Vector3 _moveVector = new Vector3();

        private void Start()
        {
            _speedDelta = _runSpeed - _walkSpeed;
            if (_speedDelta == 0)
            {
                _speedDelta = .01f;
            }
        }

        public void MovePlayer(float moveX, float moveZ, bool isRunning)
        {
            // Update the move Deltas
            _moveVector.x = moveX;
            _moveVector.z = moveZ;
            _moveVector.Normalize();

            // gradually move toward the desired speed
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, (isRunning ? _runSpeed : _walkSpeed), _acceleration * Time.deltaTime);

            // Scale the vector by the move speed
            _moveVector *= _currentSpeed;

            // Move the character
            //_charController.Move(_moveVector);

            if (moveX != 0 || moveZ != 0)
            {
                float step = _rotateSpeed * Time.deltaTime;
                Quaternion targetRotation = Quaternion.LookRotation(_moveVector, Vector3.up);
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, targetRotation, step);
            }
        }

        //public void Jump()
        //{
        //    //
        //}

        public float GetNormalizedSpeed()
        {
            return _moveVector.magnitude / _runSpeed;
        }
    }
}

