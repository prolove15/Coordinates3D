using UnityEngine;
using System.Collections;

namespace EazyCamera.Legacy
{
    using Extensions;
    [System.Serializable]
    [ExecuteInEditMode]
    public class EzCamera : MonoBehaviour
    {
        // Values to be set in the inspector
        [SerializeField] private EzCameraSettings _settings = null;
        public EzCameraSettings Settings { get { return _settings; } }
        public void ReplaceSettings(EzCameraSettings newSettings)
        {
            if (newSettings != null)
            {
                _settings = newSettings;
                _settings.StoreDefaultValues();
            }
        }

        [SerializeField] private Transform _target = null;
        public Transform Target { get { return _target; } }

        public Transform CameraTransform => _transform;
        private Transform _transform = null;

        private Vector3 _relativePosition = Vector3.zero;

        // State Machine and default state
        private EzStateMachine _stateMachine = null;
        private EzCameraState.State _defaultState = EzCameraState.State.FOLLOW;
        public EzCameraState.State DefaultState { get { return _defaultState; } }

        // State for a stationary camera that rotates to look at a target but does not follow it
        private EzStationaryState _stationaryState = null;
        public EzStationaryState StationaryState
        {
            get { return _stationaryState; }
            set { _stationaryState = value; }
        }

        // State for orbiting around a target
        private EzOrbitState _orbitState = null;
        public EzOrbitState OrbitState
        {
            get { return _orbitState; }
            set { _orbitState = value; }
        }
        [SerializeField] private bool _orbitEnabled = false;
        /// <summary>
        /// True when the camera is  allowed to orbit the taget
        /// </summary>
        public bool OribtEnabled { get { return _orbitEnabled; } }
        public void SetOrbitEnabled(bool allowOrbit)
        {
            _orbitEnabled = allowOrbit;
            if (_orbitState != null)
            {
                _orbitState.Enabled = _orbitEnabled;
            }
            else
            {
                if (_orbitEnabled)
                {
                    _orbitState = new EzOrbitState(this, _settings);
                    if (CameraController != null)
                    {
                        CameraController.HandleInputCallback += _orbitState.HandleInput;
                    }
                }
            }
        }

        // State for tracking a target object's position around the environment
        private EzFollowState _followState = null;
        public EzFollowState FollowState
        {
            get { return _followState; }
            set { _followState = value; }
        }

        [SerializeField] private bool _followEnabled = false;
        public bool FollowEnabled { get { return _followEnabled; } }

        public void SetFollowEnabled(bool followEnabled)
        {
            _followEnabled = followEnabled;
            if (_followState != null)
            {
                _followState.Enabled = _followEnabled;
            }
            else
            {
                if (_followEnabled)
                {
                    _followState = new EzFollowState(this, _settings);
                    if (CameraController != null)
                    {
                        CameraController.HandleInputCallback += _followState.HandleInput;
                    }
                }
            }
        }

        /// <summary>
        /// Set the value to true if you want the camera to be able to track an object while still following the player
        /// </summary>
        private EzLockOnState _lockOnState = null;
        public EzLockOnState LockOnState
        {
            get { return _lockOnState; }
            set { _lockOnState = value; }
        }

        [SerializeField] private bool _lockOnEnabled = true;
        public bool LockOnEnabled { get { return _lockOnEnabled; } }
        public void SetLockOnEnabled(bool enableLockOn)
        {
            _lockOnEnabled = enableLockOn;
            if (_lockOnState != null)
            {
                _lockOnState.Enabled = _lockOnEnabled;

            }
            else
            {
                if (_lockOnEnabled)
                {
                    _lockOnState = new EzLockOnState(this, _settings);
                    if (CameraController != null)
                    {
                        CameraController.HandleInputCallback += _lockOnState.HandleInput;
                    }
                }
            }
        }

        public bool ZoomEnabled { get { return _zoomEnabled; } }
        [SerializeField] private bool _zoomEnabled = true;
        private float _zoomDelta = 0f;
        private const float ZOOM_DEAD_ZONE = .01f;
        public void SetZoomEnabled(bool isEnabled) { _zoomEnabled = isEnabled; }


        [SerializeField] private bool _checkForCollisions = true;
        public bool CollisionsEnabled { get { return _checkForCollisions; } }
        public void EnableCollisionCheck(bool checkForCollisions)
        {
            _checkForCollisions = checkForCollisions;
            if (_cameraCollilder != null)
            {
                if (!checkForCollisions)
                {
                    DestroyImmediate(_cameraCollilder);
                    _cameraCollilder = null;
                }
                else
                {
                    _cameraCollilder.enabled = _checkForCollisions;
                }
            }
            else
            {
                if (_checkForCollisions)
                {
                    _cameraCollilder = this.GetOrAddComponent<EzCameraCollider>();
                }
            }
        }

        private EzCameraCollider _cameraCollilder = null;

        public EzCameraController CameraController { get; private set; }

        private void Start()
        {
            _transform = this.transform;

            // reset the offset distance be 1/3 of the distance from the min to max
            if (_settings != null)
            {
                if (Application.isPlaying)
                {
                    _settings = Instantiate(_settings);
                }

                _settings.OffsetDistance = (_settings.MaxDistance - _settings.MinDistance) / 3f;
                _settings.DesiredDistance = _settings.OffsetDistance;
                _settings.StoreDefaultValues();

                _relativePosition = (_target.position + (Vector3.up * _settings.OffsetHeight)) + (_transform.rotation * (Vector3.forward * -_settings.OffsetDistance)) + (_transform.right * _settings.LateralOffset);
                _transform.position = _relativePosition;
            }

            CameraController = this.GetOrAddComponent<EzCameraController>();
            CameraController.Init(this);

            SetLockOnEnabled(_lockOnEnabled);
            SetFollowEnabled(_followEnabled);
            SetOrbitEnabled(_orbitEnabled);

            if (_checkForCollisions)
            {
                _cameraCollilder = this.GetOrAddComponent<EzCameraCollider>();
            }

            _stateMachine = new EzStateMachine();

            _defaultState = _followEnabled ? EzCameraState.State.FOLLOW : EzCameraState.State.STATIONARY;
            SetState(_defaultState);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
#endif
                if (_stateMachine != null)
                {
                    HandleInput();
                    _stateMachine.UpdateState();
                }
#if UNITY_EDITOR
            }
#endif
        }

        private void LateUpdate()
        {
            if (_target != null && _settings != null) // prevent updating if the target is null
            {
                if (_stateMachine != null)
                {
                    if (_zoomEnabled && Mathf.Abs(_zoomDelta) > ZOOM_DEAD_ZONE)
                    {
                        ZoomCamera(_zoomDelta);
                    }
                    _stateMachine.LateUpdateState();
                }
            }
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_lockOnEnabled && !IsLockedOn)
                {
                    SetState(EzCameraState.State.LOCKON);
                }
            }

            // Zoom the camera using the middle mouse button + drag
            if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.Z))
            {
                _zoomDelta = Input.GetAxis(ExtensionMethods.MOUSEY);
            }
            else
            {
                _zoomDelta = 0;
            }
        }

        public void UpdatePosition()
        {
            // Update the position of the camera to reflect any rotation changes
            _settings.OffsetDistance = Mathf.MoveTowards(_settings.OffsetDistance, _settings.DesiredDistance, Time.deltaTime * _settings.ZoomSpeed);
            _relativePosition = (_target.position + (Vector3.up * _settings.OffsetHeight)) + (_transform.rotation * (Vector3.forward * -_settings.OffsetDistance)) + (_transform.right * _settings.LateralOffset);
            this.transform.position = _relativePosition;
        }

        public void SmoothLookAt()
        {
            Vector3 relativePlayerPosition = _target.position - _transform.position + _transform.right * _settings.LateralOffset;

            Vector3 destDir = Vector3.ProjectOnPlane(relativePlayerPosition, _transform.up);
            Quaternion lookAtRotation = Quaternion.LookRotation(destDir, Vector3.up);
            _transform.rotation = Quaternion.Lerp(_transform.rotation, lookAtRotation, _settings.RotateSpeed * Time.deltaTime);
        }

        public void ZoomCamera(float zDelta)
        {
            // clamp the value to the min/max ranges
            if (!IsOccluded)
            {
                float step = Time.deltaTime * _settings.ZoomSpeed * zDelta;
                _settings.DesiredDistance = Mathf.Clamp(_settings.OffsetDistance + step, _settings.MinDistance, _settings.MaxDistance);
            }
        }

        public void SetCameraTarget(Transform target)
        {
            _target = target;
        }

        public void SetState(EzCameraState.State nextState)
        {
            switch (nextState)
            {
                case EzCameraState.State.FOLLOW:
                    SetFollowEnabled(true);
                    _stateMachine.SetCurrentState(_followState);
                    break;
                case EzCameraState.State.ORBIT:
                    SetOrbitEnabled(true);
                    _stateMachine.SetCurrentState(_orbitState);
                    break;
                case EzCameraState.State.LOCKON:
                    SetLockOnEnabled(true);
                    _stateMachine.SetCurrentState(_lockOnState);
                    break;
                case EzCameraState.State.STATIONARY:
                default:
                    if (_stationaryState == null)
                    {
                        _stationaryState = new EzStationaryState(this, _settings);
                        if (CameraController != null)
                        {
                            CameraController.HandleInputCallback = null;
                        }
                    }

                    _stateMachine.SetCurrentState(_stationaryState);
                    break;
            }
        }


        public bool IsOccluded
        {
            get
            {
                if (!_checkForCollisions)
                {
                    return false;
                }

                return _cameraCollilder.IsOccluded;
            }
        }

        public bool IsInOrbit
        {
            get
            {
                if (_orbitState != null)
                {
                    return _stateMachine.CurrentState == _orbitState;
                }

                return false;
            }
        }

        public bool IsLockedOn
        {
            get
            {
                if (_lockOnState != null)
                {
                    return _stateMachine.CurrentState == _lockOnState;
                }

                return false;
            }
        }

        public IEnumerator Reset()
        {
            yield return null;
        }
    }
}
