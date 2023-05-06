using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EazyCamera.Legacy
{
    public class EzCameraSettings : ScriptableObject
    {
#if UNITY_EDITOR
        [MenuItem("Eazy Camera/Create Camera Settings")]
        static void Init()
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EzCameraSettings>(), "Assets/NewCameraSettings.asset");
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
#endif // UNITY_EDITOR

        public EzCameraSettings()
        {
            StoreDefaultValues();
            ResetCameraSettings();
        }

        public float OffsetHeight = 1.5f;
        public float LateralOffset = 0f;
        public float MaxLateralOffset = 5f;
        public float OffsetDistance = 4.5f;
        public float MaxDistance = 15f;
        public float MinDistance = 1f;
        public float ZoomSpeed = 10f;
        public float ResetSpeed = 5f;

        public float RotateSpeed = 15f;

        public float MaxRotX = 90f;
        public float MinRotX = -90f;

        public float DesiredDistance { get; set; }
        public float CullDistance { get; set; }
        public float ResetPositionDistance { get; set; }
        public float ZoomDistance { get; set; }

        private float _defaultHeight = 1f;
        private float _defaultLateralOffset = 0f;
        private float _defaualtDistance = 5f;

        private void OnEnable()
        {
            StoreDefaultValues();
        }

        public void StoreDefaultValues()
        {
            _defaultHeight = OffsetHeight;
            _defaultLateralOffset = LateralOffset;
            _defaualtDistance = OffsetDistance;
        }

        public void ResetCameraSettings()
        {
            OffsetHeight = _defaultHeight;
            LateralOffset = _defaultLateralOffset;
            OffsetDistance = _defaualtDistance;
            DesiredDistance = _defaualtDistance;
            ZoomDistance = OffsetDistance;
        }
    }
}
