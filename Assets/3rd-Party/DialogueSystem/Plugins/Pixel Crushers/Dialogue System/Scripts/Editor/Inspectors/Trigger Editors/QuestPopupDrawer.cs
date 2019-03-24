// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    [CustomPropertyDrawer(typeof(QuestPopupAttribute))]
    public class QuestPopupDrawer : PropertyDrawer
    {

        public QuestPicker questPicker = null;

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
            // Set up quest picker:
            if (questPicker == null)
            {
                questPicker = new QuestPicker(EditorTools.FindInitialDatabase(), prop.stringValue, true);
            }

            // Set up property drawer:
            EditorGUI.BeginProperty(position, GUIContent.none, prop);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (EditorTools.selectedDatabase == null) EditorTools.SetInitialDatabaseIfNull();
            if (ShowReferenceDatabase())
            {
                var dbPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
                EditorTools.selectedDatabase = EditorGUI.ObjectField(dbPosition, questPicker.database, typeof(DialogueDatabase), true) as DialogueDatabase;
            }
            if (EditorTools.selectedDatabase != questPicker.database)
            {
                questPicker.database = EditorTools.selectedDatabase;
                questPicker.UpdateTitles();
            }

            questPicker.Draw(position);
            prop.stringValue = questPicker.currentQuest;

            EditorGUI.EndProperty();
        }

    }

}
