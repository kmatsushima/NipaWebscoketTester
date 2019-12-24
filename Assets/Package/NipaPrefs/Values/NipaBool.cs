using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaPrefs.Hidden;

namespace NipaPrefs
{
    public class NipaBool : NipaValue<bool>
    {
        public NipaBool(string managerId, string id, bool defaultValue, string tip = "") : base(managerId, id, defaultValue, tip)
        {
            this.tip += " [boolean. e.g. true e.g. false]";
        }

        protected override void GuiHeader()
        {
            GUILayout.Label(value ? "✓" : "-");
        }

        protected override void GuiEdit(bool isSlider)
        {
            value = GUILayout.Toggle(value, "");
        }

        protected override void ValueToRawValue(bool source, out string rawValue)
        {
            rawValue = source ? "true" : "false";
        }
        protected override bool RawValueToValue(string rawValue)
        {
            value = rawValue.Contains("true") ;
            return true;
        }

        protected override bool IsFiledValid()
        {
            return true;
        }

        protected override bool GetValueFromField()
        {
            return value;
        }

        protected override bool IsSame(bool v, bool i)
        {
            return v == i;
        }
        protected override void UpdateField(bool v)
        {
        }

        public override string ToString()
        {
            return value.ToString();
        }

        public static implicit operator bool(NipaBool me)
        {
            return me.Get();
        }
    }
}