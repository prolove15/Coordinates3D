using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyCamera
{
    using EazyCamera.Events;
    using Util = EazyCameraUtility;

    public class EazyLockOnTarget : MonoBehaviour, IEventSource, ITargetable
    {
        public Vector3 LookAtPosition => _lookTarget?.position ?? this.transform.position;
        [SerializeField] private Transform _lookTarget = null;
        [SerializeField] private GameObject _interactionIcon = null;
        
        public Color TargetColor => _targetColor;
        [SerializeField] private Color _targetColor = Constants.NeutralTargetColor;

        public bool IsActive { get; private set; }

        private void Start()
        {
            if (_interactionIcon != null && _interactionIcon.activeSelf)
            {
                _interactionIcon.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.CompareTag(Util.PlayerTag))
            {
                BroadcastEvent(EazyEventKey.OnEnterFocasableRange, new EnterFocusRangeData(this));
                
                if (_interactionIcon != null)
                {
                    _interactionIcon.SetActive(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform.root.CompareTag(Util.PlayerTag))
            {
                BroadcastEvent(EazyEventKey.OnExitFocasableRange, new ExitFocusRangeData(this));

                if (_interactionIcon != null)
                {
                    _interactionIcon.SetActive(false);
                }
            }
        }

        public void BroadcastEvent(string key, EventData data)
        {
            EazyEventManager.TriggerEvent(key, data);
        }

        public void OnFocusReceived()
        {
            IsActive = true;
        }

        public void OnFocusLost()
        {
            IsActive = false;
        }
    }
}
