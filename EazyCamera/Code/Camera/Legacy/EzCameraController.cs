using UnityEngine;
using System;
using System.Collections;

namespace EazyCamera.Legacy
{
    [RequireComponent(typeof(EzCamera))]
    public class EzCameraController : MonoBehaviour
    {
        EzCamera _controlledCamera = null;
        public Action HandleInputCallback = null;

        public void Init(EzCamera camera)
        {
            _controlledCamera = camera;
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (_controlledCamera != null)
            {
                if (HandleInputCallback != null)
                {
                    HandleInputCallback();
                }
            }
        }
    }
}
