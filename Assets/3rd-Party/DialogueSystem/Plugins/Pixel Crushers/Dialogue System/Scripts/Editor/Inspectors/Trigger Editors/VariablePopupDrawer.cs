// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    [CustomPropertyDrawer(typeof(VariablePopupAttribute))]
    public class VariablePopupDrawer : PropertyDrawer
    {

        public VariablePicker variablePicker = null;

        private bool ShowReferenceDatabase()
        {
            var attr = attribute as QuestPopupAttribute;
            return (attr != null) ? attr.showReferenceDatabase : false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) +
                (ShowReferenceDatabase() ? EditorGUIUtility.singleLineHeight : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            // Set up variable picker:
            if (variablePicker == null)
            {
                variablePicker = new VariablePicker(EditorTools.FindInitialDatabase(), prop.stringValue, true);
            }

            // Set up property drawer:
            EditorGUI.BeginProperty(position, GUIContent.none, prop);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (EditorTools.selectedDatabase == null) EditorTools.SetInitialDatabaseIfNull();
            if (ShowReferenceDatabase())
            {
                var dbPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
                EditorTools.selectedDatabase = EditorGUI.ObjectField(dbPosition, variablePicker.database, typeof(DialogueDatabase), true) as DialogueDatabase;
            }
            if (EditorTools.selectedDatabase != variablePicker.database)
            {
                variablePicker.database = EditorTools.selectedDatabase;
                variablePicker.UpdateTitles();
            }

            variablePicker.Draw(position);
            prop.stringValue = variablePicker.currentVariable;

            EditorGUI.EndProperty();
        }

    }

}
