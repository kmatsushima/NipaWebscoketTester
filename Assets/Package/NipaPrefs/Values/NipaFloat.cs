using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaPrefs.Hidden;

namespace NipaPrefs
{
    public class NipaFloat : NipaValue<float>
    {
        float fieldValue;
        string fieldValueStr;

        public NipaFloat(string managerId, string id, float defaultValue, string tip = "") : base(managerId, id, defaultValue, tip)
        {
            this.tip +=  " [float]";
            fieldValue = value;
            ValueToRawValue(value, out fieldValueStr);
        }

        protected override void GuiHeader()
        {
            GUILayout.Label(value.ToString());
        }

        protected override void GuiEdit(bool isSlider)
        {
            if (!isSlider)
                fieldValueStr = GUILayout.TextField(fieldValueStr);
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(min.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.Label(max.ToString());
                GUILayout.EndHorizontal();
                value = GUILayout.HorizontalSlider(value, min, max);
                fieldValueStr = value.ToString();
            }
        }

        protected override void ValueToRawValue(float source, out string rawValue)
        {
            rawValue = source.ToString();
        }
        protected override bool RawValueToValue(string rawValue)
        {
            float result;
            if (float.TryParse(rawValue, out result))
                value = result;
            else
                return false;

            return true;
        }

        protected override bool IsFiledValid()
        {
            return float.TryParse(fieldValueStr, out fieldValue);
        }

        protected override float GetValueFromField()
        {
            return fieldValue;
        }

        protected override bool IsSame(float v, float i)
        {
            return v == i;
        }
        protected override void UpdateField(float v)
        {
            ValueToRawValue(v, out fieldValueStr);
        }

        public override string ToString()
        {
            return Get().ToString();
        }
    }
}