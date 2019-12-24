using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaPrefs.Hidden;

namespace NipaPrefs
{
    public class NipaVector : NipaValue<Vector4>
    {
        static string[] fieldLabels = new string[] { "X", "Y", "Z", "W" };
        string[] fieldValues = new string[4];

        public NipaVector(string managerId, string id, Vector4 defaultValue, string tip = "") : base(managerId, id, defaultValue, tip)
        {
            this.tip += " [vector(x,y,z,w). e.g. 5, 2.5, 10 e.g. 11, 5]";
            UpdateField(value);
        }
        protected override void GuiHeader()
        {
            GUILayout.Label(value.ToString());
        }
        protected override void GuiEdit(bool isSlider)
        {
            if (!isSlider)
            {
                for (int i = 0; i < 4; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(fieldLabels[i], GUILayout.ExpandWidth(false));
                    fieldValues[i] = GUILayout.TextField(fieldValues[i], GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(min.ToString());
                GUILayout.FlexibleSpace();
                GUILayout.Label(max.ToString());
                GUILayout.EndHorizontal();
                value.x = GUILayout.HorizontalSlider(value.x, min, max);
                value.y = GUILayout.HorizontalSlider(value.y, min, max);
                value.z = GUILayout.HorizontalSlider(value.z, min, max);
                value.w = GUILayout.HorizontalSlider(value.w, min, max);

                UpdateField(value);
            }
        }

        protected override void ValueToRawValue(Vector4 source, out string rawValue)
        {
            rawValue = string.Format("{0},{1},{2},{3}", source.x, source.y, source.z, source.w);
        }
        protected override bool RawValueToValue(string rawValue)
        {
            var result = Vector4.zero;
            var raws = rawValue.Split(',').Select(v => v.Trim()).ToList();
            if (raws.Count < 2)
                return false;

            for (int i = 0; i < raws.Count; i++)
            {
                float temp;
                if (!float.TryParse(raws[i], out temp))
                    return false;
                if (i == 0)
                    result.x = temp;
                else if (i == 1)
                    result.y = temp;
                else if (i == 2)
                    result.z = temp;
                else if (i == 3)
                    result.w = temp;
            }
            value = result;
            return true;
        }

        protected override bool IsFiledValid()
        {
            foreach (var item in fieldValues)
            {
                float temp;
                if (!float.TryParse(item, out temp))
                    return false;
            }
            return true;
        }

        protected override Vector4 GetValueFromField()
        {
            var result = Vector4.zero;
            for (int i = 0; i < fieldValues.Length; i++)
            {
                float temp;
                if (!float.TryParse(fieldValues[i], out temp))
                    return result;
                if (i == 0)
                    result.x = temp;
                else if (i == 1)
                    result.y = temp;
                else if (i == 2)
                    result.z = temp;
                else if (i == 3)
                    result.w = temp;
            }
            return result;
        }

        protected override bool IsSame(Vector4 v, Vector4 i)
        {
            return v == i;
        }
        protected override void UpdateField(Vector4 v)
        {
            fieldValues[0] = v.x.ToString();
            fieldValues[1] = v.y.ToString();
            fieldValues[2] = v.z.ToString();
            fieldValues[3] = v.w.ToString();
        }

        public float x { get { return Get().x; } }
        public float y { get { return Get().y; } }
        public float z { get { return Get().z; } }
        public float w { get { return Get().w; } }

        public override string ToString()
        {
            return Get().ToString();
        }

        public static implicit operator Vector3(NipaVector me)
        {
            return me.Get();
        }
        public static implicit operator Vector2(NipaVector me)
        {
            return me.Get();
        }
        public static implicit operator Quaternion(NipaVector me)
        {
            var v = me.Get();
            return new Quaternion(v.x, v.y, v.z, v.w);
        }
        public static implicit operator Vector2Int(NipaVector me)
        {
            var v = me.Get();
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }
        public static implicit operator Vector3Int(NipaVector me)
        {
            var v = me.Get();
            return new Vector3Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y), Mathf.RoundToInt(v.z));
        }
    }
}