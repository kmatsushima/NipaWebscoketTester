using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaPrefs.Hidden;

namespace NipaPrefs
{
    public class NipaInt : NipaValue<int>
    {
        int fieldValue;
        string fieldValueStr;

        public NipaInt(string managerId, string id, int defaultValue, string tip = "") : base(managerId, id, defaultValue, tip)
        {
            this.tip += " [int]";
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
                value =Mathf.RoundToInt( GUILayout.HorizontalSlider(value, min, max));
                fieldValueStr = value.ToString();
            }
        }

        protected override void ValueToRawValue(int source, out string rawValue)
        {
            rawValue = source.ToString();
        }
        protected override bool RawValueToValue(string rawValue)
        {
            int result;
            if (int.TryParse(rawValue, out result))
                value = result;
            else
                return false;

            return true;
        }

        protected override bool IsFiledValid()
        {
            return int.TryParse(fieldValueStr, out fieldValue);
        }

        protected override int GetValueFromField()
        {
            return fieldValue;
        }

        protected override bool IsSame(int v, int i)
        {
            return v == i;
        }
        protected override void UpdateField(int v)
        {
            ValueToRawValue(v, out fieldValueStr);
        }

        public override string ToString()
        {
            return Get().ToString();
        }
    }
}