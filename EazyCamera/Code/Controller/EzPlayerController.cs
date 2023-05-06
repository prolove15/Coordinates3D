using UnityEngine;
using System.Collections;

namespace EazyCamera.Legacy
{
    using Extensions;
    /// <summary>
    /// This is the main class for controlling both camera and player. It is recommended to attach this to the player or camera in the scene, but not necessary
    /// </summary>
    public class EzPlayerController : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        private Transform _cameraTransform = null;
        [SerializeField] private EzMotor _controlledPlayer = null;

        private void Awake()
        {
            _cameraTransform = _camera.transform;
        }

        private void Start()
        {
            // if either the player or camera are null, attempt to find them
            SetUpControlledPlayer();
        }

        private void Update()
        {
            if (_controlledPlayer != null && _camera != null)
            {
                HandleInput();
            }
        }

        private void SetUpControlledPlayer()
        {
            if (_controlledPlayer == null)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    _controlledPlayer = playerObj.GetComponent<EzMotor>();
                }
            }
        }

        private void HandleInput()
        {
            // Update player movement first
            // cache the inputs
            float horz = Input.GetAxis(ExtensionMethods.HORIZONTAL);
            float vert = Input.GetAxis(ExtensionMethods.VERITCAL);

            // Convert movement to camera space
            Vector3 moveVector = EazyCameraUtility.ConvertMoveInputToCameraSpace(_cameraTransform, horz, vert);

            // Move the Player
            _controlledPlayer.MovePlayer(moveVector.x, moveVector.z, Input.GetKey(KeyCode.LeftShift));
        }
    }
}
