#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayModeSave : MonoBehaviour
{
    public static List<SavedComponent> savedComponents = new List<SavedComponent>();
    private static List<SavedComponent> pendingExitSaves = new List<SavedComponent>();

    [Serializable]
    public class SavedComponent
    {
        public GameObject targetObject;
        public string componentName;
        public string componentPath;
        public List<ComponentField> fields;

        [Serializable]
        public class ComponentField
        {
            public string fieldName;
            public string fieldValue;
        }
    }

    [Serializable]
    public class Wrapper<T>
    {
        public T value;
    }

    [InitializeOnLoadMethod]
    private static void RegisterCallbacks()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        PlayModeDebug(state.ToString());
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            if(pendingExitSaves.Count <= 0) return;
            
            foreach (var saved in pendingExitSaves)
            {
                ApplyComponentChanges(saved);
            }
            pendingExitSaves.Clear();
        }
    }

    public static void SaveComponent(Component component, bool saveWhenExiting)
    {
        if (component == null) return;

        SavedComponent savedComponent = new SavedComponent
        {
            targetObject = component.gameObject,
            componentName = component.GetType().Name,
            componentPath = component.gameObject.GetInstanceID().ToString(),
            fields = new List<SavedComponent.ComponentField>()
        };

        savedComponent = ComponentSerializer.Capture(component);

        var fields = component.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var field in fields)
        {
            PlayModeDebug("Try Field " + field.Name);
            if (field.IsPublic || Attribute.IsDefined(field, typeof(SerializeField)))
            {
                Debug.Log("Saved field " + field.Name);
                // object value = field.GetValue(component);
                // string fieldValue = value.ToString();
                // savedComponent.fields.Add(new SavedComponent.ComponentField { fieldName = field.Name, fieldValue = fieldValue });
                object value = field.GetValue(component);

                // Handle array serialization
                if (value is Array array)
                {
                    List<string> arrayValues = new List<string>();
                    foreach (var element in array)
                    {
                        arrayValues.Add(element?.ToString() ?? "null");
                    }
                    string arrayValue = string.Join("|", arrayValues);  // Join elements with '|'
                    savedComponent.fields.Add(new SavedComponent.ComponentField { fieldName = field.Name, fieldValue = arrayValue });
                }
                else if (value is AnimationCurve animationCurve)
                {
                    try
                    {
                        // Serialize the AnimationCurve into its keyframes
                        var keyframes = animationCurve.keys;
                        List<string> keyframeStrings = new List<string>();

                        foreach (var keyframe in keyframes)
                        {
                            // Correct serialization using only 6 properties
                            string keyframeString = $"{keyframe.time},{keyframe.value},{keyframe.inTangent},{keyframe.outTangent},{keyframe.inWeight},{keyframe.outWeight}";
                            keyframeStrings.Add(keyframeString);
                        }

                        // Save the serialized keyframes as a string
                        string serializedAnimationCurve = string.Join("|", keyframeStrings);
                        savedComponent.fields.Add(new SavedComponent.ComponentField { fieldName = field.Name, fieldValue = serializedAnimationCurve });
                    }
                    catch (Exception ex)
                    {
                        PlayModeDebug($"Failed to serialize AnimationCurve for {field.Name}: {ex.Message}");
                    }
                }
                else
                {
                    string fieldValue = value?.ToString() ?? "null";  // Handle nulls properly
                    savedComponent.fields.Add(new SavedComponent.ComponentField { fieldName = field.Name, fieldValue = fieldValue });
                }
            }
        }

        if(saveWhenExiting)
        {
            PlayModeDebug("Save For While Existing");
            pendingExitSaves.Add(savedComponent);
            return;
        }

        bool isContained = false;
        for(int i = 0; i < savedComponents.Count; i++)
        {
            if(savedComponents[i].componentName == savedComponent.componentName && savedComponents[i].targetObject == savedComponent.targetObject)
            {  
                PlayModeDebug("Perform Modify");
                isContained = true;
                savedComponents[i] = savedComponent; 
                break;
            }
        }

        if(!isContained)
        {
            PlayModeDebug("Perform Add");
            savedComponents.Add(savedComponent);
        }

        PlayModeDebug("Current Count:" + savedComponents.Count + " name " + savedComponent.componentName + " target " + savedComponent.targetObject);
        GranularApplyWindow.UpdateSaveComponents();
    }

    public static void ApplyComponentChanges(SavedComponent savedComponent)
    {
        GameObject targetObject = savedComponent.targetObject;
        if (targetObject == null)
        {
            PlayModeDebug("Object is null");
            return;
        }

        Component component = targetObject.GetComponent(savedComponent.componentName);
        if (component == null)        
        {
            PlayModeDebug("Components is null");
            return;
        }

        SerializedObject serializedObject = new SerializedObject(component);


        foreach (var field in savedComponent.fields)
        {
            SerializedProperty property = serializedObject.FindProperty(field.fieldName);
            if (property == null)
            {
                Debug.LogWarning($"Property not found: {field.fieldName} on {component.name}");
                continue;
            }

            try
            {
                switch (property.propertyType)
                {
                    case SerializedPropertyType.Float:
                        if (float.TryParse(field.fieldValue, out float floatVal))
                            property.floatValue = floatVal;
                        break;

                    case SerializedPropertyType.Integer:
                        if (int.TryParse(field.fieldValue, out int intVal))
                            property.intValue = intVal;
                        break;

                    case SerializedPropertyType.Boolean:
                        if (bool.TryParse(field.fieldValue, out bool boolVal))
                            property.boolValue = boolVal;
                        break;

                    case SerializedPropertyType.String:
                        property.stringValue = field.fieldValue;
                        break;

                    case SerializedPropertyType.Vector3:
                        if (TryParseVector3(field.fieldValue, out Vector3 vec3Val))
                            property.vector3Value = vec3Val;
                        break;

                    case SerializedPropertyType.Vector2:
                        if (TryParseVector2(field.fieldValue, out Vector2 vec2Val))
                            property.vector2Value = vec2Val;
                        break;

                    case SerializedPropertyType.Color:
                        if (ColorUtility.TryParseHtmlString(field.fieldValue, out Color col))
                            property.colorValue = col;
                        break;
                    case SerializedPropertyType.Enum:
                        if (int.TryParse(field.fieldValue, out int enumIndex))
                            property.enumValueIndex = enumIndex;
                        break;

                    case SerializedPropertyType.AnimationCurve:
                        try
                        {
                            // Deserialize the saved keyframes
                            var keyframeStrings = field.fieldValue.Split('|');
                            List<Keyframe> keyframes = new List<Keyframe>();

                            foreach (var keyframeString in keyframeStrings)
                            {
                                var values = keyframeString.Split(',');
                                if (values.Length == 6)
                                {
                                    float time = float.Parse(values[0]);
                                    float value = float.Parse(values[1]);
                                    float inTangent = float.Parse(values[2]);
                                    float outTangent = float.Parse(values[3]);
                                    float inWeight = float.Parse(values[4]);
                                    float outWeight = float.Parse(values[5]);

                                    keyframes.Add(new Keyframe(time, value, inTangent, outTangent, inWeight, outWeight));
                                }
                            }

                            // Create the AnimationCurve from the keyframes
                            property.animationCurveValue = new AnimationCurve(keyframes.ToArray());
                        }
                        catch (Exception ex)
                        {
                            PlayModeDebug($"Failed to deserialize AnimationCurve for {field.fieldName}: {ex.Message}");
                        }
                        break;
                    //References
                    case SerializedPropertyType.ObjectReference:
                        if (int.TryParse(field.fieldValue, out int refID))
                        {
                            UnityEngine.Object objRef = EditorUtility.InstanceIDToObject(refID);
                            if (objRef != null)
                            {
                                property.objectReferenceValue = objRef;
                            }
                            else
                            {
                                PlayModeDebug($"Reference not found for {field.fieldName}, instanceID: {refID}");
                            }
                        }
                        break;
                    default:
                            if (property.isArray && property.propertyType != SerializedPropertyType.String)
                            {
                                string[] parts = field.fieldValue.Split('|');
                                property.arraySize = parts.Length;

                                for (int i = 0; i < parts.Length; i++)
                                {
                                    var element = property.GetArrayElementAtIndex(i);
                                    string elementValue = parts[i];

                                    switch (element.propertyType)
                                    {
                                        case SerializedPropertyType.Float:
                                            if (float.TryParse(elementValue, out float arrFloat))
                                                element.floatValue = arrFloat;
                                            break;

                                        case SerializedPropertyType.Integer:
                                            if (int.TryParse(elementValue, out int arrInt))
                                                element.intValue = arrInt;
                                            break;

                                        case SerializedPropertyType.Boolean:
                                            if (bool.TryParse(elementValue, out bool arrBool))
                                                element.boolValue = arrBool;
                                            break;

                                        case SerializedPropertyType.String:
                                            element.stringValue = elementValue;
                                            break;

                                        case SerializedPropertyType.Vector2:
                                            if (TryParseVector2(elementValue, out Vector2 arrVec2))
                                                element.vector2Value = arrVec2;
                                            break;

                                        case SerializedPropertyType.Vector3:
                                            if (TryParseVector3(elementValue, out Vector3 arrVec3))
                                                element.vector3Value = arrVec3;
                                            break;

                                        case SerializedPropertyType.Color:
                                            if (ColorUtility.TryParseHtmlString(elementValue, out Color arrColor))
                                                element.colorValue = arrColor;
                                            break;

                                        case SerializedPropertyType.ObjectReference:
                                            if (int.TryParse(elementValue, out int refArrayID))
                                            {
                                                element.objectReferenceValue = EditorUtility.InstanceIDToObject(refArrayID);
                                            }
                                            break;

                                        default:
                                            PlayModeDebug($"Unsupported array element type for {field.fieldName}[{i}]");
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                PlayModeDebug($"Unsupported type");
                            }
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to apply property {field.fieldName}: {ex.Message}");
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    [MenuItem("CONTEXT/Component/Save Now", true)]
    private static bool ValidateSaveNow(MenuCommand command) => EditorApplication.isPlaying;

    [MenuItem("CONTEXT/Component/Save When Exiting Play Mode", true)]
    private static bool ValidateSaveWhenExiting(MenuCommand command) => EditorApplication.isPlaying;

    // Right-click context menu to save component
    [MenuItem("CONTEXT/Component/Save Now")]
    private static void SaveNow(MenuCommand command)
    {
        if(!EditorApplication.isPlaying)
        {
            Debug.LogError("[KurumiC Playmode Save]: You only allow to save during play mode!"); 
            return;     
        }

        Component component = (Component)command.context;
        SaveComponent(component, false);
    }

    [MenuItem("CONTEXT/Component/Save When Exiting Play Mode")]
    private static void SaveWhenExitingPlayMode(MenuCommand command)
    {
        if(!EditorApplication.isPlaying)
        {
            Debug.LogError("[KurumiC Playmode Save]: You only allow to save during play mode!"); 
            return;     
        }

        Component component = (Component)command.context;
        SaveComponent(component, true);
    }

    //[MenuItem("Tools/KurumiC Playmode Save/Apply All Changes")]
    public static void ApplyAllChanges()
    {
        foreach (var savedComponent in savedComponents)
        {
            ApplyComponentChanges(savedComponent);
        }
        savedComponents.Clear();
    }

    public static void ApplyChanges(List<PlayModeSave.SavedComponent> sc)
    {
        foreach (var savedComponent in sc)
        {
            PlayModeDebug("Start Apply: " + savedComponent.componentName + " from " + savedComponent.targetObject.name);
            ApplyComponentChanges(savedComponent);
        }
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnEditorPlayModeExited()
    {
        savedComponents.Clear(); //Clear the data as data is stored at GranualrApplyWindow.cs
    }

    private static bool TryParseVector2(string str, out Vector2 result)
    {
        result = Vector2.zero;
        str = str.Trim('(', ')');
        var parts = str.Split(',');
        return parts.Length == 2 &&
            float.TryParse(parts[0], out result.x) &&
            float.TryParse(parts[1], out result.y);
    }

    private static bool TryParseVector3(string str, out Vector3 result)
    {
        result = Vector3.zero;
        str = str.Trim('(', ')');
        var parts = str.Split(',');
        return parts.Length == 3 &&
            float.TryParse(parts[0], out result.x) &&
            float.TryParse(parts[1], out result.y) &&
            float.TryParse(parts[2], out result.z);
    }

    private static void PlayModeDebug(string text)
    {
        Debug.Log($"[KurumiC Playmode Save]: {text}");
    }

}

public static class ComponentSerializer
{
    public static PlayModeSave.SavedComponent Capture(Component component)
    {
        var serializedObject = new SerializedObject(component);
        var iterator = serializedObject.GetIterator();

        var saved = new PlayModeSave.SavedComponent
        {
            targetObject = component.gameObject,
            componentName = component.GetType().Name,
            componentPath = component.gameObject.GetInstanceID().ToString(),
            fields = new List<PlayModeSave.SavedComponent.ComponentField>()
        };

        iterator.NextVisible(true); // Skip "m_Script"

        while (iterator.NextVisible(false))
        {
            saved.fields.Add(new PlayModeSave.SavedComponent.ComponentField
            {
                fieldName = iterator.propertyPath,
                fieldValue = SerializePropertyValue(iterator)
            });
        }

        return saved;
    }

    private static string SerializeArray(SerializedProperty arrayProp)
    {
        List<string> elements = new List<string>();
        for (int i = 0; i < arrayProp.arraySize; i++)
        {
            var element = arrayProp.GetArrayElementAtIndex(i);
            elements.Add(SerializePropertyValue(element));
        }
        return string.Join("|", elements);
    }

    private static string SerializePropertyValue(SerializedProperty prop)
    {
        switch (prop.propertyType)
        {
            case SerializedPropertyType.Float: return prop.floatValue.ToString();
            case SerializedPropertyType.Integer: return prop.intValue.ToString();
            case SerializedPropertyType.Boolean: return prop.boolValue.ToString();
            case SerializedPropertyType.String: return prop.stringValue;
            case SerializedPropertyType.Vector3: return prop.vector3Value.ToString();
            case SerializedPropertyType.Vector2: return prop.vector2Value.ToString();
            case SerializedPropertyType.Color: return "#" + ColorUtility.ToHtmlStringRGBA(prop.colorValue);
            case SerializedPropertyType.Enum: return prop.enumValueIndex.ToString();
            case SerializedPropertyType.AnimationCurve:
                return JsonUtility.ToJson(prop.animationCurveValue);
            case SerializedPropertyType.ObjectReference:
                return prop.objectReferenceValue != null 
                    ? prop.objectReferenceValue.GetInstanceID().ToString() 
                    : "0";
            case SerializedPropertyType.Generic:
            case SerializedPropertyType.ArraySize:
            case SerializedPropertyType.ExposedReference:
            case SerializedPropertyType.Quaternion:
            case SerializedPropertyType.Rect:
            case SerializedPropertyType.Bounds:
            case SerializedPropertyType.Gradient:
            case SerializedPropertyType.LayerMask:
                return "<unsupported>";
            default:
                if (prop.isArray && prop.propertyType != SerializedPropertyType.String)
                {
                    return SerializeArray(prop);
                }
                return "<unsupported>";
        }
    }

}
#endif