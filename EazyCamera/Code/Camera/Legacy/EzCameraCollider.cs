using UnityEngine;
using System.Collections;

namespace EazyCamera.Legacy
{
    public class EzCameraCollider : MonoBehaviour
    {
        private EzCamera _controlledCamera = null;
        private Camera _cameraComponent = null;
        private Transform _cameraTransform = null;
        public bool IsOccluded { get; private set; }

        private Vector3[] _nearClipPlanePoints = new Vector3[4];
        private Vector3[] _originalClipPlanePoints = new Vector3[4];
        private Vector3 _pointBehindCamera = new Vector3();

        private float _nearPlaneDistance = 0f;
        private float _aspectHalfHeight = 0f;
        private float _aspectHalfWidth = 0f;

        [SerializeField] private string _playerLayer = "Player";
        private int _layermask = 0;

        private void Start()
        {
            _controlledCamera = this.GetComponent<EzCamera>();
            _cameraComponent = this.GetComponent<Camera>();
            _cameraTransform = this.transform;

            _nearPlaneDistance = _cameraComponent.nearClipPlane;

            _layermask = (1 << LayerMask.NameToLayer(_playerLayer)) | (1 << LayerMask.NameToLayer("Ignore Raycast"));
            _layermask = ~_layermask;

            UpdateNearClipPlanePoints();
        }

        private void LateUpdate()
        {
            if (_controlledCamera.CollisionsEnabled)
            {
                HandleCollisions();
            }
        }

        public void HandleCollisions()
        {
            UpdateNearClipPlanePoints();
#if UNITY_EDITOR
            DrawNearPlane();
#endif
            if (IsOccluded)
            {
                CheckOriginalPlanePoints();
#if UNITY_EDITOR
                DrawOriginalPlane();
#endif
            }

            CheckCameraPlanePoints();
        }

        //
        // Camera Occlusion Functions
        //

        private void UpdateNearClipPlanePoints()
        {
            Vector3 nearPlaneCenter = _cameraTransform.position + _cameraTransform.forward * _nearPlaneDistance;
            _pointBehindCamera = _cameraTransform.position - _cameraTransform.forward * _nearPlaneDistance;

            float halfFOV = Mathf.Deg2Rad * (_cameraComponent.fieldOfView / 2);
            _aspectHalfHeight = Mathf.Tan(halfFOV) * _nearPlaneDistance;
            _aspectHalfWidth = _aspectHalfHeight * _cameraComponent.aspect;

            _nearClipPlanePoints[0] = nearPlaneCenter + _cameraTransform.rotation * new Vector3(-_aspectHalfWidth, _aspectHalfHeight);
            _nearClipPlanePoints[1] = nearPlaneCenter + _cameraTransform.rotation * new Vector3(_aspectHalfWidth, _aspectHalfHeight);
            _nearClipPlanePoints[2] = nearPlaneCenter + _cameraTransform.rotation * new Vector3(_aspectHalfWidth, -_aspectHalfHeight);
            _nearClipPlanePoints[3] = nearPlaneCenter + _cameraTransform.rotation * new Vector3(-_aspectHalfWidth, -_aspectHalfHeight);
        }

        #region Editor Only Functions
#if UNITY_EDITOR
        private void DrawNearPlane()
        {
            Debug.DrawLine(_nearClipPlanePoints[0], _nearClipPlanePoints[1], Color.red);
            Debug.DrawLine(_nearClipPlanePoints[1], _nearClipPlanePoints[2], Color.red);
            Debug.DrawLine(_nearClipPlanePoints[2], _nearClipPlanePoints[3], Color.red);
            Debug.DrawLine(_nearClipPlanePoints[3], _nearClipPlanePoints[0], Color.red);
            Debug.DrawLine(_pointBehindCamera, _controlledCamera.Target.position, Color.red);
        }

        private void DrawOriginalPlane()
        {
            Debug.DrawLine(_originalClipPlanePoints[0], _originalClipPlanePoints[1], Color.cyan);
            Debug.DrawLine(_originalClipPlanePoints[1], _originalClipPlanePoints[2], Color.cyan);
            Debug.DrawLine(_originalClipPlanePoints[2], _originalClipPlanePoints[3], Color.cyan);
            Debug.DrawLine(_originalClipPlanePoints[3], _originalClipPlanePoints[0], Color.cyan);
            Debug.DrawLine(_pointBehindCamera, _controlledCamera.Target.position, Color.cyan);
        }
#endif
        #endregion

        private void CheckCameraPlanePoints()
        {
#if UNITY_EDITOR
            Color lineColor = Color.black;
#endif
            RaycastHit hit;
            float hitDistance = 0;

            for (int i = 0; i < _nearClipPlanePoints.Length; ++i)
            {

                if (Physics.Linecast(_controlledCamera.Target.position, _nearClipPlanePoints[i], out hit, _layermask))
                {
                    if (hit.collider.gameObject.transform.root != _controlledCamera.Target.root)
                    {
                        if (hit.distance > hitDistance)
                        {
                            hitDistance = hit.distance;

                            if (!IsOccluded) // Only store the original position on the original hit
                            {
                                _controlledCamera.Settings.ResetPositionDistance = _controlledCamera.Settings.OffsetDistance;
                                //_controlledCamera.Settings.ResetPositionDistance = _controlledCamera.Settings.DesiredDistance;
                            }

                            IsOccluded = true;
                            _controlledCamera.Settings.DesiredDistance = hitDistance - _nearPlaneDistance;

#if UNITY_EDITOR
                            lineColor = Color.red;
                            Debug.Log("camera is occluded by " + hit.collider.gameObject.name);
#else
                        return;
#endif


                        }
                    }
                }

#if UNITY_EDITOR
                Debug.DrawLine(_nearClipPlanePoints[i], _controlledCamera.Target.position, lineColor);
#endif
            }

            if (!IsOccluded)
            {
                if (Physics.Linecast(_controlledCamera.Target.position, _pointBehindCamera, out hit, _layermask))
                {
#if UNITY_EDITOR
                    lineColor = Color.red;
                    Debug.Log("camera is occluded by " + hit.collider.gameObject.name);
#endif
                    IsOccluded = true;
                    _controlledCamera.Settings.ResetPositionDistance = _controlledCamera.Settings.OffsetDistance;
                    _controlledCamera.Settings.DesiredDistance = hit.distance - _nearPlaneDistance;
                }
            }

            //if (!IsOccluded)
            //{
            //    _controlledCamera.Settings.DesiredDistance = _controlledCamera.Settings.ResetPositionDistance;
            //}
        }

        private void UpdateOriginalClipPlanePoints()
        {
            Vector3 originalCameraPosition = (_controlledCamera.Target.position + (Vector3.up * _controlledCamera.Settings.OffsetHeight)) + (_cameraTransform.rotation * (Vector3.forward * -_controlledCamera.Settings.ResetPositionDistance)) + (_cameraTransform.right * _controlledCamera.Settings.LateralOffset);
            Vector3 originalPlaneCenter = originalCameraPosition + _cameraTransform.forward * _nearPlaneDistance;

            float halfFOV = Mathf.Deg2Rad * (_cameraComponent.fieldOfView / 2);
            _aspectHalfHeight = Mathf.Tan(halfFOV) * _nearPlaneDistance;
            _aspectHalfWidth = _aspectHalfHeight * _cameraComponent.aspect;

            _originalClipPlanePoints[0] = originalPlaneCenter + _cameraTransform.rotation * new Vector3(-_aspectHalfWidth, _aspectHalfHeight);
            _originalClipPlanePoints[1] = originalPlaneCenter + _cameraTransform.rotation * new Vector3(_aspectHalfWidth, _aspectHalfHeight);
            _originalClipPlanePoints[2] = originalPlaneCenter + _cameraTransform.rotation * new Vector3(_aspectHalfWidth, -_aspectHalfHeight);
            _originalClipPlanePoints[3] = originalPlaneCenter + _cameraTransform.rotation * new Vector3(-_aspectHalfWidth, -_aspectHalfHeight);

            //Vector3 rearPlaneCenter = _transform.position - _transform.forward * _nearPlaneDistance;
            _pointBehindCamera = _cameraTransform.position - _cameraTransform.forward * _nearPlaneDistance;
        }


        private void CheckOriginalPlanePoints()
        {
            UpdateOriginalClipPlanePoints();

            bool objectWasHit = false;
            RaycastHit hit;
            float closestHitDistance = float.MaxValue;

            for (int i = 0; i < _originalClipPlanePoints.Length; ++i)
            {
                Color lineColor = Color.blue;
                if (Physics.Linecast(_controlledCamera.Target.position, _originalClipPlanePoints[i], out hit, _layermask))
                {
                    lineColor = Color.red;
                    objectWasHit = true;

                    if (hit.distance < closestHitDistance)
                    {
                        closestHitDistance = hit.distance;
                    }
                }

                Debug.DrawLine(_controlledCamera.Target.position, _originalClipPlanePoints[i], lineColor);
            }

            if (!objectWasHit)
            {
                _controlledCamera.Settings.DesiredDistance = _controlledCamera.Settings.ResetPositionDistance;
                IsOccluded = false;
            }
            else
            {
                if (closestHitDistance > _controlledCamera.Settings.DesiredDistance)
                {
                    _controlledCamera.Settings.DesiredDistance = closestHitDistance;
                }
            }
        }
    }
}
