using UnityEngine;
using System.Collections;
using System;

namespace EazyCamera.Legacy
{
    [System.Serializable]
    public class EzStationaryState : EzCameraState
    {
        public EzStationaryState(EzCamera camera, EzCameraSettings settings)
            : base(camera, settings) { }

        public override void EnterState()
        {
            //
        }

        public override void ExitState()
        {
            //
        }



        public override void LateUpdateState()
        {
            _controlledCamera.SmoothLookAt();
        }

        public override void UpdateState()
        {
            //
        }

        public override void UpdateStateFixed()
        {
            //
        }

        public override void HandleInput()
        {
            //
        }
    }
}