using UnityEngine;

namespace EazyCamera
{
    using Util = EazyCameraUtility;

    public class EazyController : MonoBehaviour
    {
        [SerializeField] private EazyCam _controlledCamera = null;

        //////////////////////////////////////////////////////////////////////
        // custmoize field
        //////////////////////////////////////////////////////////////////////
        #region Custmoize field

        public bool lockCamRot;

        #endregion

        private void Start()
        {
            Debug.Assert(_controlledCamera != null, "Attempting to use a controller on a GameOjbect without an EazyCam component");
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            float scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta > Constants.DeadZone || scrollDelta < -Constants.DeadZone)
            {
                _controlledCamera.IncreaseZoomDistance(scrollDelta, dt);
            }

            float horz = Input.GetAxis(Util.MouseX);
            float vert = Input.GetAxis(Util.MouseY);
            
            // customize
            if(lockCamRot || (!Input.GetMouseButton(0) && !Input.GetMouseButton(1)))
            {
                horz = 0f;
                vert = 0f;
            }
            else
            {
                vert = -vert;
            }

            _controlledCamera.IncreaseRotation(horz, vert, dt);

            if (Input.GetKeyDown(KeyCode.R))
            {
                _controlledCamera.ResetPositionAndRotation();
            }


            if (Input.GetKeyUp(KeyCode.T))
            {
                ToggleLockOn();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CycleTargets();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                CycleLeft();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                CycleRight();
            }
        }

        public void SetControlledCamera(EazyCam cam)
        {
            _controlledCamera = cam;
        }

        private void ToggleLockOn()
        {
            _controlledCamera.ToggleLockOn();
        }

        private void CycleTargets()
        {
            _controlledCamera.CycleTargets();
        }

        private void CycleRight()
        {
            _controlledCamera.CycleTargetsRight();
        }

        private void CycleLeft()
        {
            _controlledCamera.CycleTargetsLeft();
        }
    }
}
