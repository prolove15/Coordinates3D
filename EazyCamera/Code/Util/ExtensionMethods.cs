using UnityEngine;
using System.Collections;

namespace EazyCamera.Extensions
{
    public static class ExtensionMethods
    {
        public const string HORIZONTAL = "Horizontal";
        public const string VERITCAL = "Vertical";
        public const string MOUSEX = "Mouse X";
        public const string MOUSEY = "Mouse Y";
        public const string MOUSE_WHEEL = "Mouse ScrollWheel";

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                component = obj.AddComponent<T>();
            }

            return component;
        }

        public static T GetOrAddComponent<T>(this Component comp) where T : Component
        {
            T component = comp.GetComponent<T>();
            if (component == null)
            {
                component = comp.gameObject.AddComponent<T>();
            }

            return component;
        }

        public static ScriptableObject Clone(this ScriptableObject obj)
        {
            return UnityEngine.Object.Instantiate(obj) as ScriptableObject;
        }

        public static T Clone<T>(this T obj) where T : ScriptableObject
        {
            return UnityEngine.Object.Instantiate<T>(obj);
        }

        public static Vector3 GetMidpointTo(this Vector3 vect, Vector3 other)
        {
            // ((B - A / 2) + A
            Vector3 result = other - vect;
            result /= 2;
            result += vect;
            return result;
        }
    }
}
