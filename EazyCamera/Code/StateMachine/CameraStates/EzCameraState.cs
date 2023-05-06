using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyCamera.Legacy
{
    public abstract class EzCameraState
    {
        public enum State : int
        {
            FOLLOW = 0
            , ORBIT
            , STATIONARY
            , LOCKON
        }

        public State StateName { get; protected set; }
        public bool Enabled { get; set; }


        [SerializeField] protected EzCamera _controlledCamera = null;
        protected EzCameraSettings _stateSettings = null;
        protected Transform _cameraTransform = null;
        protected Transform _cameraTarget = null;

        protected bool _initialized = false;

        public EzCameraState(EzCamera camera, EzCameraSettings stateCameraSettings)
        {
            if (!_initialized)
            {
                _controlledCamera = camera;
                _cameraTransform = _controlledCamera.transform;
                _cameraTarget = _controlledCamera.Target;
                _stateSettings = stateCameraSettings;
                _initialized = true;
            }
        }

        public abstract void EnterState();
        public abstract void UpdateState();
        public abstract void UpdateStateFixed();
        public abstract void LateUpdateState();
        public abstract void ExitState();

        // Cache any needed strings
        protected static readonly string HORIZONTAL = "Horizontal";
        protected static readonly string VERITCAL = "Vertical";
        protected static readonly string MOUSEX = "Mouse X";
        protected static readonly string MOUSEY = "Mouse Y";
        protected static readonly string MOUSE_WHEEL = "Mouse ScrollWheel";

        public abstract void HandleInput();
        public virtual void Reset() { }
    }
}
