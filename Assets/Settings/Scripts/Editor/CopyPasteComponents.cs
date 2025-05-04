#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TOOLS
{
    public class CopyPasteComponents : EditorWindow
    {
        private static GameObject sourceObject;
        private static Component[] copiedComponents;
        [SerializeField] private List<GameObject> targetObjects = new List<GameObject>(); // Ensure it is serialized

        private SerializedObject serializedObjectEditor;
        private SerializedProperty targetListProperty;

        [MenuItem("Tools/Copy & Paste Components")]
        public static void ShowWindow()
        {
            GetWindow<CopyPasteComponents>("Copy & Paste Components");
        }

        private void OnEnable()
        {
            serializedObjectEditor = new SerializedObject(this);
            targetListProperty = serializedObjectEditor.FindProperty("targetObjects");
        }

        private void OnGUI()
        {
            GUILayout.Label("Copy & Paste Components Tool", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            sourceObject = (GameObject)EditorGUILayout.ObjectField("Source Object", sourceObject, typeof(GameObject), true);
            EditorGUILayout.Space();

            if (GUILayout.Button("Copy All Components"))
            {
                CopyAllComponents();
            }
            EditorGUILayout.Space();

            serializedObjectEditor.Update();
            EditorGUILayout.PropertyField(targetListProperty, new GUIContent("Target Objects"), true);
            serializedObjectEditor.ApplyModifiedProperties();

            EditorGUILayout.Space();

            if (GUILayout.Button("Paste All Components"))
            {
                PasteAllComponents();
            }
        }

        private static void CopyAllComponents()
        {
            if (sourceObject != null)
            {
                copiedComponents = sourceObject.GetComponents<Component>();
                Debug.Log($"Copied {copiedComponents.Length} components from {sourceObject.name}");
            }
            else
            {
                Debug.LogWarning("No source object selected!");
            }
        }

        private void PasteAllComponents()
        {
            if (targetObjects == null || targetObjects.Count == 0)
            {
                Debug.LogWarning("No target objects selected!");
                return;
            }

            if (copiedComponents == null || copiedComponents.Length == 0)
            {
                Debug.LogWarning("No components to paste! Please copy components first.");
                return;
            }

            foreach (GameObject targetObject in targetObjects)
            {
                if (targetObject == null) continue;
                PasteComponentsToTarget(targetObject);
            }
        }

        private static void PasteComponentsToTarget(GameObject targetObject)
        {
            foreach (Component component in copiedComponents)
            {
                Type type = component.GetType();

                if (type == typeof(Transform) || type == typeof(RectTransform))
                {
                    CopyComponentValues(component, targetObject.GetComponent(type));
                }
                else
                {
                    Component targetComponent = targetObject.GetComponent(type);
                    if (targetComponent != null)
                    {
                        CopyComponentValues(component, targetComponent);
                    }
                    else
                    {
                        Component newComponent = targetObject.AddComponent(type);
                        CopyComponentValues(component, newComponent);
                    }
                }
            }

            Debug.Log($"Pasted components to {targetObject.name}");
        }

        private static void CopyComponentValues(Component source, Component destination)
        {
            if (source == null || destination == null) return;
            Type type = source.GetType();

            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                try
                {
                    field.SetValue(destination, field.GetValue(source));
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Field {field.Name} could not be copied: {ex.Message}");
                }
            }

            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!property.CanWrite || !property.CanRead || property.Name == "name" || property.Name == "hideFlags")
                    continue;

                try
                {
                    if (type == typeof(AudioSource))
                    {
                        CopyAudioSourceProperties(source as AudioSource, destination as AudioSource);
                    }
                    else
                    {
                        property.SetValue(destination, property.GetValue(source, null), null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Property {property.Name} could not be copied: {ex.Message}");
                }
            }
        }

        private static void CopyAudioSourceProperties(AudioSource source, AudioSource destination)
        {
            if (source == null || destination == null) return;

            destination.minDistance = source.minDistance;
            destination.maxDistance = source.maxDistance;
            destination.rolloffMode = source.rolloffMode;
            destination.volume = source.volume;
            destination.pitch = source.pitch;
            destination.spatialBlend = source.spatialBlend;
            destination.dopplerLevel = source.dopplerLevel;
        }
    }
}
#endif