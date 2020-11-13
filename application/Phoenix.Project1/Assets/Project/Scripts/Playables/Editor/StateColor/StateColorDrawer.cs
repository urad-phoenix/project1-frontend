namespace Phoenix.Playables.Editor
{
    using UnityEditor;
    using UnityEngine;

    [CustomPropertyDrawer(typeof(StateColorBehaviourData))]
    public class StateColorDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            int fieldCount = 3;
            return fieldCount * EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty UseTintColorProp = property.FindPropertyRelative("UseTintColor");
            SerializedProperty ColorProp = property.FindPropertyRelative("Color");
            SerializedProperty UseEffectGainProp = property.FindPropertyRelative("UseEffectGain");
            SerializedProperty EffectGainProp = property.FindPropertyRelative("EffectGain");
            SerializedProperty UseDissolveControlProp = property.FindPropertyRelative("UseDissolveControl");
            SerializedProperty DissolveControlProp = property.FindPropertyRelative("DissolveControl");
            SerializedProperty UseShadowColorProp = property.FindPropertyRelative("UseShadowColor");
            SerializedProperty ShadowColorProp = property.FindPropertyRelative("ShadowColor");
            SerializedProperty UseMapColorProp = property.FindPropertyRelative("UseMapColor");
            SerializedProperty MapColorProp = property.FindPropertyRelative("MapColor");
            SerializedProperty SpShaderProp = property.FindPropertyRelative("SpShader");
            SerializedProperty FresnelColorProp = property.FindPropertyRelative("FresnelColor");
            Rect singleFieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(singleFieldRect, UseTintColorProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, ColorProp);

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, UseEffectGainProp);
            if (UseEffectGainProp.boolValue)
            {
                singleFieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(singleFieldRect, EffectGainProp);
            }
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, UseDissolveControlProp);
            if (UseDissolveControlProp.boolValue)
            {
                singleFieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(singleFieldRect, DissolveControlProp);
            }
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, UseShadowColorProp);
            if (UseShadowColorProp.boolValue)
            {
                singleFieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(singleFieldRect, ShadowColorProp);
            }
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, UseMapColorProp);
            if (UseMapColorProp.boolValue)
            {
                singleFieldRect.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.PropertyField(singleFieldRect, MapColorProp);
            }

            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, SpShaderProp);
            singleFieldRect.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(singleFieldRect, FresnelColorProp);
        }
    }
}