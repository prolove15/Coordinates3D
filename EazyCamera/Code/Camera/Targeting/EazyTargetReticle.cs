using UnityEngine;
using UnityEngine.UI;

namespace EazyCamera
{
    public class EazyTargetReticle : MonoBehaviour
    {
        [SerializeField] private Image[] _reticleIcons = null;
        [SerializeField] private bool _alwaysFaceCamera = true;
        [SerializeField] private Color _defaultColor = Constants.InactiveTargetColor;

        public Color CurrentColor { get; private set; }

        public bool IsActive { get; private set; }

        private Transform _transform = null;
        private EazyCam _activeCamera = null;

        private void Awake()
        {
            _transform = this.transform;
        }

        private void Update()
        {
            if (IsActive && _alwaysFaceCamera)
            {
                FaceCamera();
            }
        }

        private void FaceCamera()
        {
            if (_activeCamera != null)
            {
                Vector3 direction = _activeCamera.CameraTransform.position - _transform.position;
                _transform.rotation = Quaternion.LookRotation(direction);
            }
        }

        private void SetReticleColor(Color color)
        {
            if (_reticleIcons != null)
            {
                int numImages = _reticleIcons.Length;

                for (int i = 0; i < numImages; ++i)
                {
                    _reticleIcons[i].color = color;
                }
            }
        }

        public void Enable(EazyCam activeCamera, Color activeColor)
        {
            IsActive = true;
            this.gameObject.SetActive(true);
            _activeCamera = activeCamera;
            SetReticleColor(activeColor);
        }

        public void Disable()
        {
            this.gameObject.SetActive(false);
            SetReticleColor(_defaultColor);
            IsActive = false;
        }
    }
}