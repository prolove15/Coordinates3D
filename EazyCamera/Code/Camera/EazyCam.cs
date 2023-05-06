using UnityEngine;

using DG.Tweening;

namespace EazyCamera
{
    using Util = EazyCameraUtility;
    
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class EazyCam : MonoBehaviour
    {
        [System.Serializable]
        public struct Settings
        {
            public float Distance;
            public Vector2 DefaultRotation;
            public float DefaultDistance { get; set; }

            [Header("Orbit")]
            public bool OrbitEnabled;

            [Header("Movement")]
            public float MoveSpeed;
            [Range(.1f, 1f)] public float SnapFactor;
            [Range(0f, 10f)] public float MaxLagDistance;

            [Header("Rotation")]
            public float RotationSpeed;
            public FloatRange VerticalRotation;
            public AnimationCurve EaseCurve;

            public bool InvertY;

            [Header("Collision")]
            public bool EnableCollision;
            public LayerMask CollisionLayer;

            [Header("Zoom")]
            public bool EnableZoom;
            public float ZoomDistance { get; set; }
            public FloatRange ZoomRange;

            [Header("Targeting")]
            public bool EnableTargetLock;
            public EazyTargetReticle TargetLockIcon;
        }

        [SerializeField] private Transform _followTarget = null;

        public Settings CameraSettings => _settings;
        [SerializeField] private Settings _settings = new Settings()
        {
            Distance = -5f,
            DefaultDistance = -5f,
            OrbitEnabled = true,
            MoveSpeed = 5f,
            SnapFactor = .75f,
            MaxLagDistance = 1f,
            RotationSpeed = 30f,
            VerticalRotation = new FloatRange(-89f, 89f),
            EaseCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f),
            EnableCollision = true,
            EnableZoom = true,
            ZoomDistance = -5,
            ZoomRange = new FloatRange(-10f, -1f),
            EnableTargetLock = true,
        };
        
        public Transform TargetRoot { get; private set; }

        public Vector3 FocalPoint => _focalPoint;
        private Vector3 _focalPoint = new Vector3();

        public Camera UnityCamera { get; private set; }
        
        public Transform CameraTransform => _transform;
        private Transform _transform = null;

        private Vector2 _rotation = new Vector2();

        private EazyCollider _collider = null;
        private EazyTargetManager _targetManager = null;

        private bool IsTargetLockAllowed => _settings.EnableTargetLock && _targetManager != null;

        private ITargetable _lookTargetOverride = null;
        public bool OverrideLookTarget => _lookTargetOverride != null;

        private void Awake()
        {
            _transform = this.transform;
            
            Debug.Assert(_followTarget != null, "Target should not be null on an EazyCam component");
            TargetRoot = _followTarget.root;

            UnityCamera = this.GetComponent<Camera>();

            if (_settings.EnableCollision)
            {
                _collider = new EazyCollider(this);
            }

            if (_settings.EnableTargetLock)
            {
                _targetManager = new EazyTargetManager(this);
            }

            // customize
            rotY = _rotation.y;
            rotX = _rotation.x;
        }

        private void Start()
        {
            _settings.DefaultDistance = _settings.Distance;
            _settings.ZoomDistance = _settings.Distance;

            _rotation = _settings.DefaultRotation;
            Quaternion rotation = CalculateRotationFromVector(_rotation);
            Vector3 position = _focalPoint + ((rotation * Vector3.forward) * _settings.Distance);
            _transform.SetPositionAndRotation(position, Quaternion.LookRotation(_followTarget.position - position));

            _focalPoint = _followTarget.position;

            if (Application.isPlaying)
            {
                // customize
                // Cursor.lockState = CursorLockMode.Locked;
            }
        }

        private void LateUpdate()
        {
            float dt = Time.deltaTime;
            if (OverrideLookTarget)
            {
                TargetedLook(dt);
            }
            else
            {
                DefaultLook(dt);
            }

            DebugDrawFocualCross(_focalPoint);

            if (_targetManager != null)
            {
                _targetManager.Tick(dt);
            }

            if (_collider != null)
            {
                _collider.Tick();
            }

            Debug.DrawLine(_focalPoint, _transform.position, Color.black);
        }

        private void DefaultLook(float deltaTime)
        {
            UpdatePosition(deltaTime);
#if UNITY_EDITOR
            Quaternion rotation = CalculateRotationFromVector(Application.isPlaying ? _rotation : _settings.DefaultRotation);
#else
            Quaternion rotation = CalculateRotationFromVector(_rotation);
#endif // UNITY_EDITOR

            Vector3 position = _focalPoint + ((rotation * Vector3.forward) * _settings.Distance);

            _transform.SetPositionAndRotation(position, Quaternion.LookRotation(_followTarget.position - position));
        }

        private void TargetedLook(float deltaTime)
        {
            UpdatePosition(deltaTime);
            Vector3 lookDirection = _lookTargetOverride.LookAtPosition - CameraTransform.position;
            Quaternion rotation = Quaternion.LookRotation(lookDirection);
            Vector3 position = _focalPoint + ((rotation * Vector3.forward) * _settings.Distance);
            _transform.SetPositionAndRotation(position, rotation);
        }

        private void UpdatePosition(float deltaTime)
        {
            if (_settings.MaxLagDistance > 0f && _settings.SnapFactor < 1f)
            {
                Vector3 travelDirection = _followTarget.position - _focalPoint;
                float travelDistance = travelDirection.sqrMagnitude;
                float maxDistance = _settings.MaxLagDistance.Squared();

                if (travelDistance > maxDistance)
                {
                    _focalPoint = Vector3.MoveTowards(_focalPoint, _followTarget.position - (travelDirection.normalized * _settings.MaxLagDistance), _settings.MoveSpeed * deltaTime);
                }
                else if (travelDistance < Constants.DeadZone.Squared())
                {
                    _focalPoint = _followTarget.position;
                }

                float pointOnCurve = 1 - Mathf.Clamp01(travelDistance / maxDistance);
                float speed = _settings.MoveSpeed * _settings.EaseCurve.Evaluate(pointOnCurve);

                float step = _settings.SnapFactor * deltaTime * speed;

                _focalPoint = Vector3.MoveTowards(_focalPoint, _followTarget.position, step);
            }
            else
            {
                _focalPoint = _followTarget.position;
            }
        }

        public void SetRotation(float horzRot, float vertRot)
        {
            _rotation.x = vertRot;
            _rotation.y = horzRot;

            ClampHorizontalRotation();
            ClampVerticalRotation();
        }

        public void SetRotationUnclamped(float horzRot, float vertRot)
        {
            _rotation.x = vertRot;
            _rotation.y = horzRot;
        }

        //-------------------------------------------------- customize
        public void IncreaseRotation(float horzRotDelta, float vertRotDelta, float deltaTime)
        {
            if (_settings.OrbitEnabled)
            {
                // ClampHorizontalRotation_Custom();
                float step = deltaTime * _settings.RotationSpeed;
                // _rotation.y += horzRotDelta * step;
                rotY = rotY + horzRotDelta * step;
                Tween_RotationY(rotY);

                // _rotation.x += vertRotDelta * step * (_settings.InvertY ? 1f : -1f);
                rotX = rotX + vertRotDelta * step * (_settings.InvertY ? 1f : -1f);
                ClampVerticalRotation_Custom();
                Tween_RotationX(rotX);
            }
        }

        //-------------------------------------------------- customize
        public float rotY;
        Sequence rotYSeq_tp;
        void Tween_RotationY(float target_pr)
        {
            rotYSeq_tp.Kill();

            rotYSeq_tp = DOTween.Sequence();

            rotYSeq_tp.Append(DOTween.To(() => _rotation.y, x => _rotation.y = x,
                target_pr, 1f));
            
            rotYSeq_tp.OnComplete(() => {
                if (rotY > 360f)
                {
                    rotY -= 360f;
                }
                else if (rotY < -360f)
                {
                    rotY += 360f;
                }
            });

            rotYSeq_tp.Play();
        }

        //-------------------------------------------------- customize
        float rotX;
        void Tween_RotationX(float target_pr)
        {
            Sequence rotXSeq_tp = DOTween.Sequence();

            rotXSeq_tp.Append(DOTween.To(() => _rotation.x, x => _rotation.x = x,
                target_pr, 1f));

            rotXSeq_tp.Play();
        }

        //-------------------------------------------------- customize
        void ClampHorizontalRotation_Custom()
        {
            if (rotY > 360f)
            {
                rotY -= 360f;
            }
            else if (rotY < -360f)
            {
                rotY += 360f;
            }
        }

        //-------------------------------------------------- customize
        void ClampVerticalRotation_Custom()
        {
            rotX = Mathf.Clamp(rotX, _settings.VerticalRotation.Min, _settings.VerticalRotation.Max);
        }

        private void ClampHorizontalRotation()
        {
            if (_rotation.y > 360f)
            {
                _rotation.y -= 360f;
            }
            else if (_rotation.y < -360f)
            {
                _rotation.y += 360f;
            }
        }

        private void ClampVerticalRotation()
        {
            _rotation.x = Mathf.Clamp(_rotation.x, _settings.VerticalRotation.Min, _settings.VerticalRotation.Max);
        }

        public Vector3 GetDefaultPosition()
        {
            float distance = _settings.EnableZoom ? _settings.ZoomDistance : _settings.Distance;
            return _focalPoint + ((CalculateRotationFromVector(_rotation) * Vector3.forward) * distance);
        }

        private Quaternion CalculateRotationFromVector(Vector3 rotation)
        {
            Quaternion addRot = Quaternion.Euler(0f, rotation.y, 0f);
            return addRot * Quaternion.Euler(rotation.x, 0f, 0f); // Not commutative
        }

        private void DebugDrawFocualCross(Vector3 position)
        {
            Debug.DrawLine(position - (Vector3.up / 2f), position + (Vector3.up / 2f), Color.green);
            Debug.DrawLine(position - (Vector3.right / 2f), position + (Vector3.right / 2f), Color.red);
            Debug.DrawLine(position - (Vector3.forward / 2f), position + (Vector3.forward / 2f), Color.blue);
        }
        
        /// <summary>
        /// Directly sets the distance of the camera. If zoom is enabled, it will chose the farther of the two values
        /// </summary>
        /// <param name="distance"></param>
        public void SetDistance(float distance)
        {
            if (_settings.EnableZoom)
            {
                // #DG: account for camera in front (take abs of both values and pick the smaller)
                _settings.Distance = Mathf.Max(distance, _settings.ZoomDistance);
            }
            else
            {
                _settings.Distance = distance;
            }
        }

        /// <summary>
        /// Sets Zoom Distance of the camera. If distance is not in the allowed range, it will be clamped to the range
        /// </summary>
        /// <param name="zoomDistance"></param>
        public void SetZoomDistance(float zoomDistance)
        {
            _settings.ZoomDistance = Util.ClampToRange(zoomDistance, _settings.ZoomRange);
            SetDistance(_settings.ZoomDistance);
        }

        public void IncreaseZoomDistance(float inputDelta, float deltaTime)
        {
            if (_settings.EnableZoom)
            {
                inputDelta *= _settings.MoveSpeed * deltaTime;
                inputDelta += _settings.ZoomDistance;
                _settings.ZoomDistance = Util.ClampToRange(inputDelta, _settings.ZoomRange);
                SetDistance(_settings.ZoomDistance);
            }
        }

        public float GetDistance()
        {
            if (_settings.EnableZoom)
            {
                return Mathf.Max(_settings.Distance, _settings.ZoomDistance);
            }

            return _settings.Distance;
        }

        public void ResetToUnoccludedDistance()
        {
            _settings.Distance = _settings.EnableZoom ? _settings.ZoomDistance : _settings.DefaultDistance;
        }

        public void ResetToDefaultDistance()
        {
            _settings.Distance = _settings.DefaultDistance;
        }

        public void SetZoomEnabled(EnabledState state)
        {
            _settings.EnableZoom = state == EnabledState.Enabled;
        }

        public void SetCollisionEnabled(EnabledState state)
        {
            _settings.EnableCollision = state == EnabledState.Enabled;
            if (_settings.EnableCollision)
            {
                if (_collider == null)
                {
                    _collider = new EazyCollider(this);
                }
            }
            else
            {
                if (_collider != null)
                {
                    ResetToUnoccludedDistance();
                }

                _collider = null;
            }
        }

        public void ResetPositionAndRotation()
        {
            _rotation = new Vector2();
            _focalPoint = _followTarget.position;
            ResetToDefaultDistance();
        }

        public bool PointIsOnScreen(Vector2 point)
        {
            Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
            return rect.Contains(point);
        }

        public void SetOrbitEnabled(EnabledState state)
        {
            _settings.OrbitEnabled = state == EnabledState.Enabled ? true : false;
        }

        //
        // Targeting
#region Targeting
        public void SetTargetingEnabled(EnabledState state)
        {
            _settings.EnableTargetLock = state == EnabledState.Enabled;
            if (_settings.EnableCollision)
            {
                if (_targetManager == null)
                {
                    _targetManager = new EazyTargetManager(this);
                }
            }
            else
            {
                if (_targetManager != null)
                {
                    _targetManager.ClearTargetsInRange();
                }

                _targetManager = null;
            }
        }

        public void AddTargetInRange(ITargetable target)
        {
            if (IsTargetLockAllowed)
            {
                _targetManager.AddTargetInRange(target);
            }
        }

        public void RemoveTargetInRange(ITargetable target)
        {
            if (IsTargetLockAllowed)
            {
                _targetManager.AddTargetInRange(target);
            }
        }

        public void BeginLockOn()
        {
            if (IsTargetLockAllowed)
            {
                _targetManager.BeginTargetLock();
            }
        }

        public void EndLockOn()
        {
            if (IsTargetLockAllowed)
            {
                _targetManager.EndTargetLock();
            }
        }

        public void ToggleLockOn()
        {
            if (IsTargetLockAllowed)
            {
                _targetManager.ToggleLockOn();
            }
        }

        public void SetLookTargetOverride(ITargetable targetOverride)
        {
            _lookTargetOverride = targetOverride;
        }

        public void ClearLookTargetOverride()
        {
            _lookTargetOverride = null;
            Vector3 currentRotation = _settings.OrbitEnabled ? _transform.rotation.eulerAngles : _settings.DefaultRotation;

            SetRotation(currentRotation.y, currentRotation.x);
        }

        public void CycleTargets()
        {
            _targetManager.CycleTargets();
        }

        public void CycleTargetsRight()
        {
            _targetManager.CycleTargets(_transform.right);
        }

        public void CycleTargetsLeft()
        {
            _targetManager.CycleTargets(-_transform.right);
        }

        public void CycleTargets(Vector3 direction)
        {
            _targetManager.CycleTargets(direction);
        }

#endregion // Targeting

        public void OverrideSettings(Settings settings)
        {
            _settings = settings;
        }
    }
}

