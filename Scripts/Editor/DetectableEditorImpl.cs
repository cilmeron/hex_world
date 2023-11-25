using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

[CustomEditor(typeof(Detector))]
public class DetectableEditorImpl : Editor
{
    private SerializedProperty implementingClassProperty;

    private void OnEnable()
    {
        implementingClassProperty = serializedObject.FindProperty("_detactables");
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector for the Detector class.
        DrawDefaultInspector();

        // Continue with your custom code for the dropdown.
        serializedObject.Update();

        // Get all classes that implement the IDetectable interface.
        Type[] implementingClasses = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(Detectable).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
            .ToArray();

        // Convert the array of implementing classes to an array of their names.
        string[] classNames = implementingClasses.Select(type => type.FullName).ToArray();

        // Find the selected index in the array.
        int selectedIndex = implementingClasses.ToList().FindIndex(type => type.FullName == implementingClassProperty.stringValue);

        // Display a custom dropdown-like field in the inspector for the programmer to choose an implementing class.
        Rect controlRect = EditorGUILayout.GetControlRect();
        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUI.Popup(controlRect, "Detectable Classes", selectedIndex, classNames);
        if (EditorGUI.EndChangeCheck())
        {
            implementingClassProperty.stringValue = selectedIndex >= 0 ? implementingClasses[selectedIndex].FullName : string.Empty;
        }

        serializedObject.ApplyModifiedProperties();
    }
}