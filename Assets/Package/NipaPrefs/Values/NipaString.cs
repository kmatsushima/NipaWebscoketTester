using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaPrefs.Hidden;

namespace NipaPrefs
{
    public class NipaString : NipaValue<string>
    {
        string fieldValue;

        public NipaString(string managerId, string id, string defaultValue, string tip = "") : base(managerId, id, defaultValue, tip)
        {
            this.tip += " [string]";
        }

        protected override void GuiHeader()
        {
            GUILayout.Label(value.ToString());
        }

        protected override void GuiEdit(bool isSlider)
        {
            fieldValue = GUILayout.TextField(fieldValue);
        }

        protected override void ValueToRawValue(string source, out string rawValue)
        {
            rawValue = source;
        }
        protected override bool RawValueToValue(string rawValue)
        {
                value = rawValue;
            return true;
        }

        protected override bool IsFiledValid()
        {
            return true;
        }

        protected override string GetValueFromField()
        {
            return fieldValue;
        }

        protected override bool IsSame(string v, string i)
        {
            return v == i;
        }
        protected override void UpdateField(string v)
        {
            ValueToRawValue(v, out fieldValue);
        }

        public override string ToString()
        {
            return Get().ToString();
        }
    }
}