using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyCamera
{
    public enum EnabledState
    {
        Disabled = 0,
        Enabled = 1,
    }

    public enum LockOnStyle
    {
        Toggle, // Press button to turn targeting on/off
        Hold // Targeting active while button held down
    }

    public enum TargetSwitchStyle
    {
        Cycle,
        Nearest
    }

    public static class EazyCameraUtility
    {
        public static readonly string MouseX = "Mouse X";
        public static readonly string MouseY = "Mouse Y";
        public static readonly string PlayerTag = "Player";

        public static void NoOp<T>(T t) { }

        public static Vector3 ConvertMoveInputToCameraSpace(Transform cameraTransform, float horz, float vert)
        {
            float moveX = (horz * cameraTransform.right.x) + (vert * cameraTransform.forward.x);
            float moveZ = (horz * cameraTransform.right.z) + (vert * cameraTransform.forward.z);
            return new Vector3(moveX, 0f, moveZ);
        }

        public static float Squared(this float num)
        {
            return num * num;
        }

        public static float ClampToRange(float value, FloatRange range)
        {
            return Mathf.Clamp(value, range.Min, range.Max);
        }

        public static float Lerp(this FloatRange range, float t)
        {
            return Mathf.Lerp(range.Min, range.Max, t);
        }
    }
}
