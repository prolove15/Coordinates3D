using UnityEngine;
using System.Collections;

namespace EazyCamera.Legacy
{
    [RequireComponent(typeof(SphereCollider))]
    public class EzLockOnTarget : MonoBehaviour
    {
        // need AABB and  transform
        //public GameObject TargetIcon { get { return _targetIcon; } }
        [SerializeField] private GameObject _targetIcon = null;
        [SerializeField] private EzCamera _playerCamera = null;
        [SerializeField] private Color32 _inactiveColor = new Color32(127, 127, 127, 127);
        [SerializeField] private Color32 _activeColor = new Color32(255, 0, 0, 255);
        [SerializeField] private float _activationDistance = 10f;

        private SphereCollider _collider = null;
        private Renderer _iconRenderer = null;

        private void Awake()
        {
            _collider = this.GetComponent<SphereCollider>();
            _collider.isTrigger = true;
            _collider.radius = _activationDistance;
        }

        private void Start()
        {
            if (_playerCamera == null)
            {
                _playerCamera = GameObject.FindObjectOfType<EzCamera>();
            }

            _iconRenderer = _targetIcon.GetComponent<Renderer>();
            _iconRenderer.enabled = false;

            SetIconActive(false);

            // Set the physics layer as not to interfere with Camera Occlusion
            this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<EzMotor>() != null)
            {
                SetIconVisible(true);

                if (_playerCamera != null)
                {
                    EzLockOnState lockonState = _playerCamera.LockOnState;
                    if (lockonState != null)
                    {
                        lockonState.AddTarget(this);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<EzMotor>() != null && _playerCamera != null)
            {
                SetIconVisible(false);

                EzLockOnState lockonState = _playerCamera.LockOnState;
                if (lockonState != null)
                {
                    lockonState.RemoveTarget(this);
                }
            }
        }

        public void SetIconActive(bool isActive = true)
        {
            if (_targetIcon != null)
            {
                //_targetIcon.SetActive(isActive);
                _iconRenderer.material.color = (isActive) ? _activeColor : _inactiveColor;
            }
        }

        private void SetIconVisible(bool isVisible)
        {
            _iconRenderer.enabled = isVisible;
        }
    }
}