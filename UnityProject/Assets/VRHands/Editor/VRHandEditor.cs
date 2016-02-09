using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRHand))]
public class VRHandEditor : Editor
{

    private bool AToggle, BToggle = false;
    private VRHand _hand = null;

    SerializedProperty grabPointProperty, deviceIDProperty, animProperty, grabModeProperty, useAxisProperty, axisIDProperty, grabThresholdProperty, useButtonProperty, buttonIDProperty, grabSpeedProperty = null;

    void OnEnable()
    {
        _hand = (VRHand)target;
        AToggle = _hand.UseAxis;
        BToggle = _hand.UseButton;
        grabPointProperty = serializedObject.FindProperty("GrabPoint");
        deviceIDProperty = serializedObject.FindProperty("DeviceID");
        animProperty = serializedObject.FindProperty("Animator");
        grabModeProperty = serializedObject.FindProperty("grabMode");
        useAxisProperty = serializedObject.FindProperty("UseAxis");
        axisIDProperty = serializedObject.FindProperty("GrabAxis");
        grabThresholdProperty = serializedObject.FindProperty("GrabThreshold");
        useButtonProperty = serializedObject.FindProperty("UseButton");
        buttonIDProperty = serializedObject.FindProperty("GrabButton");
        grabSpeedProperty = serializedObject.FindProperty("GrabSpeed");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(grabPointProperty);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(deviceIDProperty);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(animProperty);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(grabModeProperty);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(useAxisProperty);
        AToggle = useAxisProperty.boolValue;
        _hand.UseAxis = AToggle;
        GUILayout.EndHorizontal();
        if (AToggle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(axisIDProperty);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(grabThresholdProperty);
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(useButtonProperty);
        BToggle = useButtonProperty.boolValue;
        _hand.UseButton = BToggle;
        GUILayout.EndHorizontal();
        if (BToggle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(buttonIDProperty);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            EditorGUILayout.PropertyField(grabSpeedProperty);
            GUILayout.EndHorizontal();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
