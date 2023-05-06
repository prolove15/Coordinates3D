using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace EazyCamera.Legacy
{
    [CustomEditor(typeof(EzCamera))]
    public class EzCamInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            EzCamera cam = (EzCamera)target;

            EditorGUI.BeginChangeCheck();
            {
                if (cam != null)
                {
                    Transform camTarget = EditorGUILayout.ObjectField("Target", cam.Target, typeof(Transform), true) as Transform;
                    if (camTarget != cam.Target)
                    {
                        cam.SetCameraTarget(camTarget);
                    }

                    EzCameraSettings settings = EditorGUILayout.ObjectField("Camera Settings", cam.Settings, typeof(EzCameraSettings), false) as EzCameraSettings;
                    if (settings != cam.Settings)
                    {
                        cam.ReplaceSettings(settings);
                    }

                    // #DG: TODO:
                    //SerializedProperty settingsProperty = serializedObject.FindProperty("_settings");
                    //if (settingsProperty != null)
                    //{
                    //    using (new EditorGUI.IndentLevelScope())
                    //    {
                    //        EditorGUILayout.PropertyField(settingsProperty);
                    //    }
                    //}

                    // Additional States
                    SerializedProperty orbitProperty = serializedObject.FindProperty("_orbitEnabled");
                    orbitProperty.boolValue = EditorGUILayout.Toggle("Orbit Enabled", orbitProperty.boolValue);

                    EditorGUI.BeginDisabledGroup(orbitProperty.boolValue);
                    {
                        SerializedProperty followProperty = serializedObject.FindProperty("_followEnabled");
                        if (orbitProperty.boolValue == false)
                        {
                            followProperty.boolValue = EditorGUILayout.Toggle("Follow Enabled", followProperty.boolValue);
                        }
                        else
                        {
                            followProperty.boolValue = EditorGUILayout.Toggle("Follow Enabled", true);
                        }

                    }
                    EditorGUI.EndDisabledGroup();

                    SerializedProperty lockOnProperty = serializedObject.FindProperty("_lockOnEnabled");
                    lockOnProperty.boolValue = EditorGUILayout.Toggle("Lock On Enabled", lockOnProperty.boolValue);

                    SerializedProperty zoomProperty = serializedObject.FindProperty("_zoomEnabled");
                    zoomProperty.boolValue = EditorGUILayout.Toggle("Zoom Enabled", zoomProperty.boolValue);

                    SerializedProperty collisionProperty = serializedObject.FindProperty("_checkForCollisions");
                    collisionProperty.boolValue = EditorGUILayout.Toggle("Collisions Enabled", collisionProperty.boolValue);
                }

                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    serializedObject.Update();
                    EditorUtility.SetDirty(cam);
                }
            }
        }
    }
}
