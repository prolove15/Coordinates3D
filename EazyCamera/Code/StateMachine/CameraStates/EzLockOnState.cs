using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace EazyCamera.Legacy
{
    public class EzLockOnState : EzCameraState
    {
        public LockOnStyle _lockOnStyle = LockOnStyle.Hold;
        public TargetSwitchStyle _switchStyle = TargetSwitchStyle.Nearest;

        private EzLockOnTarget _currentTarget = null;
        private List<EzLockOnTarget> _nearbyTargets = null;

        private bool _isActive = false;

        /// <summary>
        /// Camera will snap to target when the angle between the forward vector and the relative position is less than this value
        /// </summary>
        [SerializeField] private float _snapAngle = 2.5f;

        public EzLockOnState(EzCamera camera, EzCameraSettings settings)
            : base(camera, settings)
        {
            _nearbyTargets = new List<EzLockOnTarget>();
        }

        public override void EnterState()
        {
            //
        }

        public override void UpdateState()
        {
            //
        }

        public override void ExitState()
        {
            if (_currentTarget != null)
            {
                _currentTarget.SetIconActive(false);
                _currentTarget = null;
            }
        }

        public override void LateUpdateState()
        {
            LockOnTarget();
            _controlledCamera.UpdatePosition();
        }

        public override void UpdateStateFixed()
        {
            //
        }

        public override void HandleInput()
        {
            if (!_controlledCamera.LockOnEnabled)
            {
                return;
            }

            if (_nearbyTargets.Count == 0)
            {
                return;
            }

            if (_isActive)
            {
                if (_switchStyle == TargetSwitchStyle.Nearest)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        MoveToNextTarget(_cameraTransform.right);
                    }
                    else if (Input.GetKeyDown(KeyCode.Q))
                    {
                        MoveToNextTarget(-_cameraTransform.right);
                    }
                }
                else if (_switchStyle == TargetSwitchStyle.Cycle)
                {
                    if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Q))
                    {
                        CycleTargets();
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (!_controlledCamera.IsLockedOn)
                {
                    _controlledCamera.SetState(State.LOCKON);
                }

                if (_lockOnStyle == LockOnStyle.Toggle)
                {
                    _isActive = !_isActive;
                }
                else
                {
                    _isActive = true;
                }

                if (_isActive)
                {
                    SetInitialTarget();
                }
                else
                {
                    _controlledCamera.SetState(_controlledCamera.DefaultState);
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space) && _lockOnStyle == LockOnStyle.Hold)
            {
                _isActive = false;
                _controlledCamera.SetState(_controlledCamera.DefaultState);
            }
        }

        private void LockOnTarget()
        {
            if (_currentTarget != null)
            {
                float step = Time.deltaTime * _stateSettings.RotateSpeed;

                Vector3 relativePos = _currentTarget.transform.position - _cameraTransform.position;

                if (Vector3.Angle(_cameraTransform.forward, relativePos) > _snapAngle)
                {
                    Quaternion nextRot = Quaternion.Lerp(_cameraTransform.rotation, Quaternion.LookRotation(relativePos), step);
                    _cameraTransform.rotation = nextRot;
                }
                else
                {
                    _cameraTransform.rotation = Quaternion.LookRotation(relativePos);
                }
            }
        }

        private void SetInitialTarget()
        {
            // Find the closest Target

            // for now set to the initial one in the list
            _currentTarget = _nearbyTargets[0];
            _currentTarget.SetIconActive();
        }

        public void MoveToNextTarget(Vector3 direction)
        {
            // if one target early out
            if (_nearbyTargets.Count <= 1)
            {
                return;
            }

            // if two targets, toggle between them
            if (_nearbyTargets.Count == 2)
            {
                _currentTarget.SetIconActive(false);
                _currentTarget = _currentTarget == _nearbyTargets[0] ? _nearbyTargets[1] : _nearbyTargets[0];
                _currentTarget.SetIconActive(true);
                return;
            }

            // if more than two targets:
            // Find the target nearest to the direction we want to move 
            EzLockOnTarget nearestTarget = _currentTarget;
            EzLockOnTarget nextTarget = null;
            Vector3 relativeDirection = direction;
            float currentNearestDistance = float.MaxValue;
            float sqDstance = float.MaxValue;

            for (int i = 0; i < _nearbyTargets.Count; ++i)
            {
                nextTarget = _nearbyTargets[i];
                if (nextTarget == _currentTarget)
                {
                    continue;
                }

                relativeDirection = nextTarget.transform.position - _cameraTransform.position;
                if (Vector3.Dot(relativeDirection, direction) > 0)
                {
                    //sqDstance = relativeDirection.sqrMagnitude;
                    sqDstance = (_currentTarget.transform.position - nextTarget.transform.position).sqrMagnitude;
                    if (sqDstance < currentNearestDistance)
                    {
                        nearestTarget = nextTarget;
                        currentNearestDistance = sqDstance;
                    }
                }
            }

            _currentTarget.SetIconActive(false);
            _currentTarget = nearestTarget;
            _currentTarget.SetIconActive(true);
        }

        private void CycleTargets()
        {
            // if one target early out
            if (_nearbyTargets.Count <= 1)
            {
                return;
            }

            // if two targets, toggle between them
            if (_nearbyTargets.Count == 2)
            {
                _currentTarget = _currentTarget == _nearbyTargets[0] ? _nearbyTargets[1] : _nearbyTargets[0];
                return;
            }

            _currentTarget.SetIconActive(false);
            _currentTarget = _nearbyTargets[CycleIndex(_nearbyTargets.IndexOf(_currentTarget), _nearbyTargets.Count)];
            _currentTarget.SetIconActive(true);
        }

        private int CycleIndex(int startIndex, int numElements)
        {
            return (numElements + (startIndex + 1)) % numElements;
        }

        public void AddTarget(EzLockOnTarget newTarget)
        {
            if (!_nearbyTargets.Contains(newTarget))
            {
                _nearbyTargets.Add(newTarget);
            }
        }

        public void RemoveTarget(EzLockOnTarget targetToRemove)
        {
            if (_nearbyTargets.Contains(targetToRemove))
            {
                _nearbyTargets.Remove(targetToRemove);

                if (_currentTarget == targetToRemove)
                {
                    _currentTarget.SetIconActive(false);
                    _currentTarget = null;
                    if (_nearbyTargets.Count > 0)
                    {
                        _currentTarget = _nearbyTargets[0];
                        _currentTarget.SetIconActive(true);
                    }
                    else
                    {
                        _isActive = false;
                        _controlledCamera.SetState(_controlledCamera.DefaultState);
                    }
                }
            }
        }
    }
}
