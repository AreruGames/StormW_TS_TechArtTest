using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(V2_Button), true)]
[CanEditMultipleObjects]
public class V2ButtonEditor : ButtonEditor
{
    private SerializedProperty click;
    private SerializedProperty enter;
    private SerializedProperty exit;
    private SerializedProperty onEnd;
    private SerializedProperty animationSpeed;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        click = serializedObject.FindProperty("click");
        enter = serializedObject.FindProperty("enter");
        exit = serializedObject.FindProperty("exit");
        onEnd = serializedObject.FindProperty("onEnd");
        animationSpeed = serializedObject.FindProperty("animationSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        base.OnInspectorGUI();

        EditorGUILayout.PropertyField(onEnd);
        EditorGUILayout.PropertyField(animationSpeed);

        EditorGUILayout.PropertyField(click);
        EditorGUILayout.PropertyField(enter);
        EditorGUILayout.PropertyField(exit);
        serializedObject.ApplyModifiedProperties();
    }
}
