using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NipaPrefs.Hidden;

namespace NipaPrefs
{
    public class NipaColor : NipaValue<Color>
    {
        static string[] fieldLabelsRGBA = new string[] { "R", "G", "B", "A" };
        static string[] fieldLabelsHSVA = new string[] { "H", "S", "V", "A" };
        float[] fieldValues = new float[4];
        bool isHSV;

        public NipaColor(string managerId, string id, Color defaultValue, string tip = "") : base(managerId, id, defaultValue, tip)
        {
            this.tip += " [color. RGBA e.g. 240,180,180,0.5 (RGB:0~255, A:0~1) or HTML code e.g. #FF5733 e.g. #FF42429B or HTML code and alpha e.g. #FF5733, 0.5]";
            UpdateField(value);
        }
        protected override void GuiHeader()
        {
            var guiColor = GUI.color;
            GUILayout.Label(value.ToString());
            GUI.color = value;
            GUILayout.Label("■■■■");
            GUI.color = guiColor;
        }
        protected override void GuiEdit(bool isSlider)
        {
            var temp = GUILayout.Toggle(isHSV, "HSV+alpha");
            if (temp != isHSV)
            {
                if (!isHSV)
                {
                    isHSV = false;
                    var c = GetValueFromField();
                    isHSV = true;
                    UpdateField(c);
                }
                else
                {
                    isHSV = true;
                    var c = GetValueFromField();
                    isHSV = false;
                    UpdateField(c);
                }
                isHSV = temp;
            }

            if (!isHSV)
                for (int i = 0; i < 4; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(fieldLabelsRGBA[i]);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(string.Format("{0} ({1} / 255)", fieldValues[i], Mathf.RoundToInt(fieldValues[i] * 255f)));
                    GUILayout.EndHorizontal();

                    fieldValues[i] = GUILayout.HorizontalSlider(fieldValues[i], 0f, 1f);
                }
            else
                for (int i = 0; i < 4; i++)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(fieldLabelsHSVA[i]);
                    GUILayout.FlexibleSpace();
                    GUILayout.Label(fieldValues[i].ToString());
                    GUILayout.EndHorizontal();

                    fieldValues[i] = GUILayout.HorizontalSlider(fieldValues[i], 0f, 1f);
                }
        }

        protected override void ValueToRawValue(Color source, out string rawValue)
        {
            rawValue = string.Format("{0},{1},{2},{3}", source.r * 255f, source.g * 255f, source.b * 255f, source.a);
        }
        protected override bool RawValueToValue(string rawValue)
        {
            var result = Color.clear;
            var raws = rawValue.Split(',').Select(v => v.Trim()).ToList();
            if (raws.Count == 1)
            {
                if (ColorUtility.TryParseHtmlString(raws[0], out result))
                {
                    value = result;
                    return true;
                }
                else
                    return false;
            }
            else if (raws.Count == 2)
            {
                if (ColorUtility.TryParseHtmlString(raws[0], out result))
                {
                    float alpha;
                    if (float.TryParse(raws[1], out alpha))
                    {
                        value = result;
                        value.a = alpha;
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            else if (raws.Count < 4)
                return false;


            for (int i = 0; i < 4; i++)
            {
                float temp;
                if (!float.TryParse(raws[i], out temp))
                    return false;
                if (i == 0)
                    result.r = temp / 255f;
                else if (i == 1)
                    result.g = temp / 255f;
                else if (i == 2)
                    result.b = temp / 255f;
                else if (i == 3)
                    result.a = temp;
            }
            value = result;
            return true;
        }

        protected override bool IsFiledValid()
        {
            return true;
        }

        protected override Color GetValueFromField()
        {
            if (!isHSV)
                return new Color(fieldValues[0], fieldValues[1], fieldValues[2], fieldValues[3]);
            else
            {
                var result = Color.HSVToRGB(fieldValues[0], fieldValues[1], fieldValues[2]);
                result.a = fieldValues[3];
                return result;
            }
        }

        protected override bool IsSame(Color v, Color i)
        {
            return v == i;
        }
        protected override void UpdateField(Color v)
        {
            if (!isHSV)
            {
                fieldValues[0] = v.r;
                fieldValues[1] = v.g;
                fieldValues[2] = v.b;
                fieldValues[3] = v.a;
            }
            else
            {
                Color.RGBToHSV(v, out fieldValues[0], out fieldValues[1], out fieldValues[2]);
                fieldValues[3] = v.a;
            }
        }
        public float r { get { return Get().r; } }
        public float g { get { return Get().g; } }
        public float b { get { return Get().b; } }
        public float a { get { return Get().a; } }

        public override string ToString()
        {
            return Get().ToString();
        }
    }
}