// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem
{

    [CustomPropertyDrawer(typeof(ActorPopupAttribute))]
    public class ActorPopupDrawer : PropertyDrawer
    {
        //[TODO] v2: Share actor/item/quest picker code.

        public ActorPicker actorPicker = null;

        private bool ShowReferenceDatabase()
        {
            var attr = attribute as ActorPopupAttribute;
            return (attr != null) ? attr.showReferenceDatabase : false;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) +
                (ShowReferenceDatabase() ? EditorGUIUtility.singleLineHeight : 0);
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            // Set up actor picker:
            if (actorPicker == null)
            {
                actorPicker = new ActorPicker(EditorTools.FindInitialDatabase(), prop.stringValue, true);
            }

            // Set up property drawer:
            EditorGUI.BeginProperty(position, GUIContent.none, prop);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            if (EditorTools.selectedDatabase == null) EditorTools.SetInitialDatabaseIfNull();
            if (ShowReferenceDatabase())
            {
                var dbPosition = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, position.height - EditorGUIUtility.singleLineHeight);
                EditorTools.selectedDatabase = EditorGUI.ObjectField(dbPosition, actorPicker.database, typeof(DialogueDatabase), true) as DialogueDatabase;
            }
            if (EditorTools.selectedDatabase != actorPicker.database)
            {
                actorPicker.database = EditorTools.selectedDatabase;
                actorPicker.UpdateTitles();
            }

            actorPicker.Draw(position);
            prop.stringValue = actorPicker.currentActor;

            EditorGUI.EndProperty();
        }

    }

}
