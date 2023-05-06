using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EazyCamera
{
    public static class Constants
    {
        public const float DeadZone = .001f;
        public const float RotationDeadZone = 1f;
        public static readonly float RotDeadZoneCos = .95f;
        public static readonly Color EnemyTargetColor = Color.red;
        public static readonly Color InactiveTargetColor = Color.gray;
        public static readonly Color NeutralTargetColor = Color.cyan;

        public static readonly string CameraStaticLayerName = "CameraStatic";

        public static readonly Rect ViewBounds = new Rect(new Vector3(), new Vector3(1f, 1f));
    }
}
