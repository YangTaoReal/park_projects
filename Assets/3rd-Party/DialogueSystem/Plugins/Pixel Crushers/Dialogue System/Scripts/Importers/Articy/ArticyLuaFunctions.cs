#if USE_ARTICY
// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Collections.Generic;

namespace PixelCrushers.DialogueSystem.Articy
{

    /// <summary>
    /// Implements articy:expresso functions.
    /// </summary>
    [AddComponentMenu("")] // Use wrapper.
    public class ArticyLuaFunctions : MonoBehaviour
    {

        private void OnEnable()
        {
            Lua.RegisterFunction("getObj", this, SymbolExtensions.GetMethodInfo(() => getObj(string.Empty)));
            Lua.RegisterFunction("getObject", this, SymbolExtensions.GetMethodInfo(() => getObj(string.Empty)));
            Lua.RegisterFunction("getProp", this, SymbolExtensions.GetMethodInfo(() => getProp(string.Empty, string.Empty)));
            Lua.RegisterFunction("setProp", this, SymbolExtensions.GetMethodInfo(() => setProp(string.Empty, string.Empty, default(object))));
        }

        private void OnDisable()
        {
            Lua.UnregisterFunction("getObj");
            Lua.UnregisterFunction("getObject");
            Lua.UnregisterFunction("getProp");
            Lua.UnregisterFunction("setProp");
        }

        private void OnConversationLine(Subtitle subtitle)
        {
            Lua.Run("speaker = Actor_" + subtitle.speakerInfo.id);
            Lua.Run("self = Actor_" + subtitle.speakerInfo.id);
        }

        public static string getObj(string objectName)
        {
            var db = DialogueManager.MasterDatabase;
            var actor = db.GetActor(objectName);
            if (actor != null) return "Actor_" + actor.id;
            var item = db.GetItem(objectName);
            if (item != null) return "Item_" + item.id;
            var location = db.GetLocation(objectName);
            if (location != null) return "Location_" + location.id;
            var conversation = db.GetConversation(objectName);
            if (conversation != null) return "Conversation_" + conversation.id;
            return null;
        }

        public static object getProp(string objectIdentifier, string propertyName)
        {
            if (string.IsNullOrEmpty(objectIdentifier) || !objectIdentifier.Contains("_")) return null;
            var id = Tools.StringToInt(objectIdentifier.Substring(objectIdentifier.IndexOf('_') + 1));
            var db = DialogueManager.MasterDatabase;
            if (objectIdentifier.StartsWith("Actor_"))
            {
                return GetAssetFieldValue(db.GetActor(id), propertyName);
            }
            else if (objectIdentifier.StartsWith("Item_"))
            {
                return GetAssetFieldValue(db.GetItem(id), propertyName);
            }
            else if (objectIdentifier.StartsWith("Location_"))
            {
                return GetAssetFieldValue(db.GetLocation(id), propertyName);
            }
            else if (objectIdentifier.StartsWith("Conversation_"))
            {
                return GetAssetFieldValue(db.GetConversation(id), propertyName);
            }
            else
            {
                return null;
            }
        }

        public static void setProp(string objectIdentifier, string propertyName, object value)
        {
            if (string.IsNullOrEmpty(objectIdentifier) || !objectIdentifier.Contains("_")) return;
            var id = Tools.StringToInt(objectIdentifier.Substring(objectIdentifier.IndexOf('_') + 1));
            var db = DialogueManager.MasterDatabase;
            if (objectIdentifier.StartsWith("Actor_"))
            {
                SetAssetFieldValue(db.GetActor(id), propertyName, value);
            }
            else if (objectIdentifier.StartsWith("Item_"))
            {
                SetAssetFieldValue(db.GetItem(id), propertyName, value);
            }
            else if (objectIdentifier.StartsWith("Location_"))
            {
                SetAssetFieldValue(db.GetLocation(id), propertyName, value);
            }
            else if (objectIdentifier.StartsWith("Conversation_"))
            {
                SetAssetFieldValue(db.GetConversation(id), propertyName, value);
            }
        }

        private static object GetAssetFieldValue(Asset asset, string fieldName)
        {
            return (asset != null) ? GetFieldValue(asset.fields, fieldName) : null;
        }

        private static object GetFieldValue(List<Field> fields, string fieldName)
        {
            var field = Field.Lookup(fields, fieldName);
            if (field == null) return null;
            switch (field.type)
            {
                case FieldType.Boolean:
                    return Tools.StringToBool(field.value);
                case FieldType.Number:
                    return (double)Tools.StringToFloat(field.value);
                default:
                    return field.value;
            }
        }

        private static void SetAssetFieldValue(Asset asset, string fieldName, object value)
        {
            if (asset == null || string.IsNullOrEmpty(fieldName)) return;
            var field = Field.Lookup(asset.fields, fieldName);
            if (field == null) return;
            if (value == null)
            {
                field.value = string.Empty;
                return;
            }
            field.value = value.ToString();
            var valueType = value.GetType();
            if (valueType == typeof(bool))
            {
                field.type = FieldType.Boolean;
            }
            else if (valueType == typeof(double) || valueType == typeof(float) || valueType == typeof(int))
            {
                field.type = FieldType.Number;
            }
        }

    }
}
#endif
