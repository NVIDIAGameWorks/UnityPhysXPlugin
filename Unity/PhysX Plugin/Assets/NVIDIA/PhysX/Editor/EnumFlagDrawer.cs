using NVIDIA.PhysX.Unity;
using System;
using UnityEditor;
using UnityEngine;

namespace NVIDIA.PhysX.UnityEditor
{
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            Enum targetEnum = (Enum)fieldInfo.GetValue(property.serializedObject.targetObject);

            string propName = flagSettings.name;
            if (string.IsNullOrEmpty(propName))
                propName = ObjectNames.NicifyVariableName(property.name);

            EditorGUI.BeginProperty(position, label, property);
            //Enum enumNew = EditorGUI.EnumMaskPopup(position, propName, targetEnum);
            Enum enumNew = EditorGUI.EnumFlagsField(position, label, targetEnum);
            property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
            EditorGUI.EndProperty();
        }
    }
}