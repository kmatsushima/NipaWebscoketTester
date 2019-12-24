using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaPrefs.Hidden
{
    public static class TipWriter
    {
        public static void WriteTipOnFile(string path, string valueId, string tip)
        {
            var content = System.IO.File.ReadAllText(path);
            var tag = string.Format("<{0}>", valueId);
            var tipTag = string.Format("_{0}_", valueId);
            if (!content.Contains(tag) || content.IndexOf(tipTag) != -1)
                return;

            var index = content.IndexOf(tag) - 1;
            content = content.Insert(index, System.Environment.NewLine + string.Format("<!--{0} {1}-->", tipTag, tip) + System.Environment.NewLine);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path))
            {
                file.WriteLine(content);
            }
        }
    }
}