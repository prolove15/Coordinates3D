using UnityEngine;

namespace EazyCamera
{
    public class EazyCollider
    {
        private EazyCam _controlledCamera = null;
        private Camera _cameraComponent = null;
        private Transform _cameraTransform = null;
        public bool IsOccluded { get; private set; }

        private Vector3[] _nearClipPlanePoints = new Vector3[4];
        private Vector3[] _originalClipPlanePoints = new Vector3[4];
        private Vector3 _pointBehindCamera = new Vector3();

        private float _nearPlaneDistance = 0f;
        private float _aspectHalfHeight = 0f;
        private float _aspectHalfWidth = 0f;

        private int _layermask = 0;

        private bool _enabled = true;

        public EazyCollider(EazyCam camera)
        {
            Debug.Assert(camera != null, "Attempting to create collsions on a non-camera object");
            _controlledCamera = camera;
            _cameraComponent = camera.UnityCamera;
            _cameraTransform = camera.CameraTransform;

            _nearPlaneDistance = _cameraComponent.nearClipPlane;

            _layermask = _controlledCamera.CameraSettings.CollisionLayer;

            UpdateNearClipPlanePoints();
        }

        public void Tick()
        {
            if (_enabled)
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
            Debug.DrawLine(_pointBehindCamera, _controlledCamera.FocalPoint, Color.red);
        }

        private void DrawOriginalPlane()
        {
            Debug.DrawLine(_originalClipPlanePoints[0], _originalClipPlanePoints[1], Color.cyan);
            Debug.DrawLine(_originalClipPlanePoints[1], _originalClipPlanePoints[2], Color.cyan);
            Debug.DrawLine(_originalClipPlanePoints[2], _originalClipPlanePoints[3], Color.cyan);
            Debug.DrawLine(_originalClipPlanePoints[3], _originalClipPlanePoints[0], Color.cyan);
            Debug.DrawLine(_pointBehindCamera, _controlledCamera.FocalPoint, Color.cyan);
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

                if (Physics.Linecast(_controlledCamera.FocalPoint, _nearClipPlanePoints[i], out hit, _layermask))
                {
                    if (hit.collider.gameObject.transform.root != _controlledCamera.TargetRoot)
                    {
                        if (hit.distance > hitDistance)
                        {
                            hitDistance = hit.distance;

                            IsOccluded = true;
                            _controlledCamera.SetDistance(-_nearPlaneDistance - hitDistance);

#if UNITY_EDITOR
                            lineColor = Color.red;
                            //Debug.Log("camera is occluded by " + hit.collider.gameObject.name);
#else
                        return;
#endif
                        }
                    }
                }

#if UNITY_EDITOR
                Debug.DrawLine(_nearClipPlanePoints[i], _controlledCamera.FocalPoint, lineColor);
#endif
            }

            if (!IsOccluded)
            {
                if (Physics.Linecast(_controlledCamera.FocalPoint, _pointBehindCamera, out hit, _layermask))
                {
#if UNITY_EDITOR
                    lineColor = Color.red;
                    //Debug.Log("camera is occluded by " + hit.collider.gameObject.name);
#endif
                    IsOccluded = true;
                    _controlledCamera.SetDistance(-_nearPlaneDistance - hit.distance);
                }
            }
        }

        private void UpdateOriginalClipPlanePoints()
        {
            Vector3 originalCameraPosition = _controlledCamera.GetDefaultPosition();
            Vector3 originalPlaneCenter = originalCameraPosition + _cameraTransform.forward * _nearPlaneDistance;

            float halfFOV = Mathf.Deg2Rad * (_cameraComponent.fieldOfView / 2);
            _aspectHalfHeight = Mathf.Tan(halfFOV) * _nearPlaneDistance;
            _aspectHalfWidth = _aspectHalfHeight * _cameraComponent.aspect;

            _originalClipPlanePoints[0] = originalPlaneCenter + _cameraTransform.rotation * new Vector3(-_aspectHalfWidth, _aspectHalfHeight);
            _originalClipPlanePoints[1] = originalPlaneCenter + _cameraTransform.rotation * new Vector3(_aspectHalfWidth, _aspectHalfHeight);
            _originalClipPlanePoints[2] = originalPlaneCenter + _cameraTransform.rotation * new Vector3(_aspectHalfWidth, -_aspectHalfHeight);
            _originalClipPlanePoints[3] = originalPlaneCenter + _cameraTransform.rotation * new Vector3(-_aspectHalfWidth, -_aspectHalfHeight);

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
                if (Physics.Linecast(_controlledCamera.FocalPoint, _originalClipPlanePoints[i], out hit, _layermask))
                {
                    lineColor = Color.red;
                    objectWasHit = true;

                    if (hit.distance < closestHitDistance)
                    {
                        closestHitDistance = hit.distance;
                    }
                }

                Debug.DrawLine(_controlledCamera.FocalPoint, _originalClipPlanePoints[i], lineColor);
            }

            if (!objectWasHit)
            {
                _controlledCamera.ResetToUnoccludedDistance();
                IsOccluded = false;
            }
            else
            {
                if (closestHitDistance > _controlledCamera.GetDistance())
                {
                    _controlledCamera.SetDistance(-closestHitDistance);
                }
            }
        }

        public void SetEnabled(bool isEnabled)
        {
            _enabled = isEnabled;
        }
    }
}
