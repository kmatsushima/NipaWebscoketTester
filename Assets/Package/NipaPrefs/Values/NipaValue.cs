using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NipaPrefs.Hidden
{
    public abstract class NipaValue<T>
    {
        protected string id;
        protected T value;
        protected T defaultValue;
        protected string tip = "";
        protected float min;
        protected float max;

        string managerId;
        NipaPrefsManager manager;
        bool isEditMode;
        bool isTipShown;
        bool initialized;
        T savedValue;
        Color guiColor = Color.white;

        public NipaValue(string managerId, string id, T defaultValue, string tip = "")
        {
            this.id = id;
            this.value = this.defaultValue = defaultValue;
            this.savedValue = this.value;
            this.tip = tip;
            this.managerId = managerId;
            if (NipaPrefsManagerInterface.GetManager(managerId, out manager))
                Initialize();
            else
                NipaPrefsManagerInterface.OnManagerReady += LazyInitialize; 
        }

        public void OnGUISlider(float min, float max, string title = "")
        {
            this.min = min;
            this.max = max;
            GuiContent(true, title);
        }

        public void OnGUI(string title = "")
        {
            GuiContent(false, title);
        }

        void GuiContent(bool isSlider, string title)
        {
            guiColor = GUI.color;
            GUILayout.BeginVertical();

            var isSavedValueLatest = IsSame(value, savedValue);
            var isDefault = IsSame(value, defaultValue);
            GUI.color = isSavedValueLatest ? Color.white : Color.yellow;

            GUILayout.BeginHorizontal();
            GUILayout.Label(isDefault ? string.Format("[ {0} ]", title.Length == 0 ? id : title) : string.Format("[ {0} ]*", title.Length == 0 ? id : title));
            GUILayout.FlexibleSpace();
            GuiHeader();
            if (GUILayout.Button(isEditMode ? "▲" : "▼", GUILayout.ExpandWidth(false)))
                isEditMode = !isEditMode;
            GUILayout.EndHorizontal();

            if (isEditMode)
            {
                using (var v = new GUILayout.VerticalScope("box"))
                {
                    GuiEdit(isSlider);

                    GUI.color = Color.white;

                    var validValueInField = IsFiledValid();
                    guiColor = validValueInField ? Color.white : Color.red;

                    if (validValueInField)
                        value = GetValueFromField();

                    if (isTipShown)
                        GUILayout.Label(tip, GUILayout.ExpandWidth(false));

                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Mgr", GUILayout.ExpandWidth(false)))
                        manager.ToggleMenu(true);
                    if (GUILayout.Button("Tip", GUILayout.ExpandWidth(false)))
                        isTipShown = !isTipShown;

                    GUILayout.FlexibleSpace();

                    if ((!validValueInField || !isDefault) && GUILayout.Button("Def", GUILayout.ExpandWidth(false)))
                    {
                        value = defaultValue;
                        UpdateField(value);
                    }
                    if ((!validValueInField || !isSavedValueLatest) && GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
                    {
                        value = savedValue;
                        UpdateField(value);
                    }
                    GUI.color = Color.yellow;
                    if (validValueInField && !isSavedValueLatest && GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
                    {
                        string raw;
                        ValueToRawValue(value, out raw);
                        manager.SaveRawValue(id, raw);
                        savedValue = value;
                        if (manager.enableManualUpdate)
                            manager.ManualUpdate();
                    }
                    GUI.color = Color.white;
                    if (!isSavedValueLatest && manager.enableManualUpdate && GUILayout.Button("Apply"))
                        manager.ManualUpdate();
                    GUILayout.EndHorizontal();
                }
            }

            GUILayout.EndVertical();
            GUI.color = guiColor;
        }

        public void Set(T value)
        {
            this.value = value;
        }

        public T Get()
        {
            if (!initialized) // this can be happen, when Get() is called BEFORE the manager gets ready and LazyInitialize is called.
            {
                var mgr = NipaPrefsManagerInterface.GetManager(managerId);
                if (mgr != null)
                    LazyInitialize(mgr); //call LazyInitialize to unsubscribe event
                else
                    Debug.LogErrorFormat("[NipaPrefs] value id {0} failed to find its manager {1}", id, managerId);
            }

            return value;
        }

        public static implicit operator T(NipaValue<T> me)
        {
            return me.Get();
        }

        void LazyInitialize(NipaPrefsManager mgr)
        {
            if (initialized)
                NipaPrefsManagerInterface.OnManagerReady -= LazyInitialize;
            else if (mgr.id == managerId)
            {
                NipaPrefsManagerInterface.OnManagerReady -= LazyInitialize;
                manager = mgr;
                Initialize();
            }
        }

        void Initialize()
        {
            string raw;
            if (manager.GetValue(id, out raw))
                RawValueToValue(raw);
            else
                value = defaultValue;
            savedValue = value;
            UpdateField(value);
            manager.RegisterEditGui(id, () => GuiContent(false, ""));
            manager.OnSave += () =>
            {
                if (IsFiledValid())
                {
                    string r;
                    ValueToRawValue(value, out r);
                    manager.SaveRawValue(id, r);
                    savedValue = value;
                }
            };
            manager.OnWriteTips += () =>
            {
                manager.WriteTip(id, tip);
            };
            if (manager.enableManualLoad)
                manager.OnLoad += () =>
                {
                    if (manager.GetValue(id, out raw))
                        RawValueToValue(raw);
                };
            manager.OnReset += () =>
            {
                value = defaultValue;
                UpdateField(value);
            };
            initialized = true;
        }

        protected abstract void GuiHeader();
        protected abstract void GuiEdit(bool isSlider);
        protected abstract bool IsFiledValid();
        protected abstract void ValueToRawValue(T source, out string rawValue);
        protected abstract bool RawValueToValue(string rawValue);

        protected abstract T GetValueFromField();

        protected abstract bool IsSame(T v, T i);

        protected abstract void UpdateField(T v);
    }
}