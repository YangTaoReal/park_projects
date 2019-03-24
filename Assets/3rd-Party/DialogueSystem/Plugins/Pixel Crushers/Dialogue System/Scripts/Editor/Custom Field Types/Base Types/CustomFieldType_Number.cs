using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

namespace PixelCrushers.DialogueSystem
{

    [CustomFieldTypeService.Name("Number")]
    public class CustomFieldType_Number : CustomFieldType
    {
        public override FieldType storeFieldAsType
        {
            get
            {
                return FieldType.Number;
            }
        }
        public override string Draw(string currentValue, DialogueDatabase dataBase)
        {
            return EditorGUILayout.FloatField(StringToFloat(currentValue, 0)).ToString();
        }

        public override string Draw(Rect rect, string currentValue, DialogueDatabase dataBase)
        {
            return EditorGUI.FloatField(rect, StringToFloat(currentValue, 0)).ToString();
        }

        private float StringToFloat(string s, int defaultValue)
        {
            try
            {
                return (float)System.Convert.ToDouble(s);
            }
            catch (FormatException)
            {
                return defaultValue;
            }
        }

    }
}