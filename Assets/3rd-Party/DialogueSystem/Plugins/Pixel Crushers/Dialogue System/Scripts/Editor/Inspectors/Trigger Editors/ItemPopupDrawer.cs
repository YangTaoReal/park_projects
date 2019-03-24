// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    [CustomPropertyDrawer(typeof(ItemPopupAttribute))]
    public class ItemPopupDrawer : PropertyDrawer
    {
        //[TODO] Share item/quest picker code.

        public ItemPicker itemPicker = null;

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
            // Set up item picker:
            if (itemPicker == null)
            {
                itemPicker = new ItemPicker(EditorTools.FindInitialDatabase(), prop.stringValue, true);
            }

            // Set up property drawer:
            EditorGUI.BeginProperty(position, GUIContent.none, prop);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (EditorTools.selectedDatabase == null) EditorTools.SetInitialDatabaseIfNull();
            if (ShowReferenceDatabase())
            {
                var dbPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
                EditorTools.selectedDatabase = EditorGUI.ObjectField(dbPosition, itemPicker.database, typeof(DialogueDatabase), true) as DialogueDatabase;
            }
            if (EditorTools.selectedDatabase != itemPicker.database)
            {
                itemPicker.database = EditorTools.selectedDatabase;
                itemPicker.UpdateTitles();
            }

            itemPicker.Draw(position);
            prop.stringValue = itemPicker.currentItem;

            EditorGUI.EndProperty();
        }

    }

}
