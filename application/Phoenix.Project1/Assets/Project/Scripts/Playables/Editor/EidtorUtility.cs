using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.DOTweenEditor.Core;
using UnityEngine;
using UnityEditor;

public class EidtorUtility
{
    public static object DrawObjectField(string name, FieldInfo field, object obj, float width)
    {
        if (obj == null)
            return null;                        

        Type type = obj.GetType();     

        if (type.IsPrimitive || type.IsEnum)
        {
            if (type.IsEnum)
            {
                obj = EnumPopup(name, width, obj as Enum, width);
            }
            if (typeof(bool) == type)
            {                    
                // ReSharper disable once HeapView.BoxingAllocation
                obj = ToggleField(name, width, (bool)obj, width);
            }
            else if (typeof(int) == type)
            {               
                // ReSharper disable once HeapView.BoxingAllocation
                obj = IntField(name, width, (int)obj, width);
            }
            else if (typeof(float) == type)
            {
                // ReSharper disable once HeapView.BoxingAllocation
                obj = FloatField(name, width, (float)obj, width);
            }
            else if (typeof(Guid) == type)
            {
                return null;
            }
        }
        else if (type == typeof(string))
        {
            obj = TextField(name, width, (string)obj, width);
        }
        return obj;
    } 
    
     #region Fields
        public static int IntField(string labelcontent, float labelWidth, int value, float width)
        {
            return IntField(new GUIContent(labelcontent), labelWidth, value, width);
        }

        public static int IntField(GUIContent labelcontent, float labelWidth, int value, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));
            int result = EditorGUILayout.IntField(value, GUILayout.Width(width));
            GUILayout.EndHorizontal();

            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static int IntPopup(GUIContent labelcontent, float labelWidth, int value, string[] optionList, int[] valueList, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));
            int result = EditorGUILayout.IntPopup(value, optionList, valueList, GUILayout.Width(width));
            GUILayout.EndHorizontal();

            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static int IntPopup(string labelcontent, float labelWidth, int value, string[] optionList, int[] valueList, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));
            int result = EditorGUILayout.IntPopup(value, optionList, valueList, GUILayout.Width(width));
            GUILayout.EndHorizontal();

            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static int Popup(string label, float labelWidth, int value, string[] optionList, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(labelWidth));
            int result = EditorGUILayout.Popup(value, optionList, GUILayout.Width(width));
            GUILayout.EndHorizontal();
            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static int Popup(GUIContent labelcontent, float labelWidth, int value, string[] optionList, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));
            int result = EditorGUILayout.Popup(value, optionList, GUILayout.Width(width));
            GUILayout.EndHorizontal();
            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static Enum EnumPopup(string labelcontent, float labelWidth, Enum value, float width)
        {
            return EnumPopup(new GUIContent(labelcontent), labelWidth, value, width);
        }

        public static Enum EnumPopup(GUIContent labelcontent, float labelWidth, Enum value, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));
            Enum result = EditorGUILayout.EnumPopup(value, GUILayout.Width(width));
            GUILayout.EndHorizontal();

            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static float FloatField(GUIContent labelcontent, float labelWidth, float value, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));            
            float result = EditorGUILayout.FloatField(value, GUILayout.Width(width));
            GUILayout.EndHorizontal();

            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static float FloatField(string labelcontent, float labelWidth, float value, float width)
        {
            return FloatField(new GUIContent(labelcontent), labelWidth, value, width);
        }

        public static string TextField(GUIContent labelcontent, float labelWidth, string value, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));
            string result = EditorGUILayout.TextField(value, GUILayout.Width(width));           
            GUILayout.EndHorizontal();

            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static string TextField(string labelcontent, float labelWidth, string value, float width)
        {
            return TextField(new GUIContent(labelcontent), labelWidth, value, width);
        }

        public static bool ToggleField(GUIContent labelcontent, float labelWidth, bool value, float width)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));
            bool result = EditorGUILayout.Toggle(value, GUILayout.Width(width));
            GUILayout.EndHorizontal();

            if (Application.isPlaying)
                return value;
            else
                return result;
        }

        public static bool ToggleField(string labelcontent, float labelWidth, bool value, float width)
        {
            return ToggleField(new GUIContent(labelcontent), labelWidth, value, width);
        }

        public static T ObjectField<T>(GUIContent labelcontent, float labelWidth, UnityEngine.Object value, float width, bool allowSceneObjects) where T : UnityEngine.Object
        {      
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelcontent, GUILayout.Width(labelWidth));
            T obj = (T)EditorGUILayout.ObjectField(value, typeof(T), allowSceneObjects, GUILayout.Width(width));
            GUILayout.EndHorizontal();

            return obj;
        }

        public static T ObjectField<T>(string labelcontent, float labelWidth, UnityEngine.Object value, float width, bool allowSceneObjects) where T : UnityEngine.Object
        {
            return ObjectField<T>(new GUIContent(labelcontent), labelWidth, value, width, allowSceneObjects);
        }

        #endregion
}
