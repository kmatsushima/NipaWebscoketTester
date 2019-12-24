using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using System;
using NipaPrefs.Hidden;

namespace NipaPrefs
{
    public class NipaPrefsManager : MonoBehaviour
    {
        public event Action OnSave;
        public event Action OnWriteTips;
        public event Action OnPrefsUpdated;
        public event Action OnReset;
        public event Action OnLoad;
        public string id;

        [SerializeField, HeaderAttribute("Editor Path")]
        PathProvider.EditorRootDirectroy rootDirEditor;
        ///<summary> path of parent dir of file from root dir, can be empty. e.g. Main/Prefs </summary>
        [SerializeField] string parentDirPathFromRootDirEditor = "";
        ///<summary> e.g. MySetting.xml </summary>
        [SerializeField] string fileNameWithExtensionEditor;

        [SerializeField, HeaderAttribute("Standalone Path")]
        PathProvider.StandaloneRootDirectroy rootDirStandalone;
        ///<summary> path of parent dir of file from root dir, can be empty. e.g. Prefs </summary>
        [SerializeField] string parentDirPathFromRootDirStandalone = "";
        ///<summary> e.g. MySetting.xml </summary>
        [SerializeField] string fileNameWithExtensionEditorStandalone;

        ///<summary> enable invoking event OnPrefsUpdated, so  listeners update its nornaml value according to NipaValue </summary>
        public bool enableManualUpdate = false;
        ///<summary> enable load values from setting file manualy. otherwise, value is loaded when it is accessed </summary>
        public bool enableManualLoad = false;
        public int guiLayoutWindowId;
        public Rect windowRect = new Rect(0, 0, 500, 500);
        public float valueEditorHeight = 300f;


        string path = "";
        string dirPath;
        bool firstLoadDone;
        bool rootExist;
        XDocument xml;
        XElement root;
        bool isMenuActive;
        bool isEditMode;
        bool isFileExists;
        Dictionary<string, Action> valueIdAndGuis = new Dictionary<string, Action>();

        #region < PUBLIC >=========================================================================================================================< PUBLIC >

        #region ===============================================  Nipa value management


        public void RegisterEditGui(string id, Action gui)
        {
            if (!valueIdAndGuis.ContainsKey(id))
                valueIdAndGuis.Add(id, gui);
            else
                valueIdAndGuis[id] = gui; //constractor of nipa value with initializer  in monobehavior can be called several times 
        }

        public bool GetValue(string id, out string rawValue)
        {
            if (!firstLoadDone)
            {
                GeneratePath();
                Load();
                firstLoadDone = true;
            }
            if (rootExist && root.HasElements && root.Elements().Any(v => v.Name == id))
            {
                rawValue = root.Element(id).Value;
                return true;
            }
            rawValue = "";
            rawValue = "";
            return false;
        }

        public void SaveRawValue(string id, string rawValue)
        {
            if (!firstLoadDone)
                Load();

            if (!rootExist)
            {
                xml = new XDocument();
                xml.Add(new XElement(this.id, new XElement(id)));
                root = xml.Element(this.id);
                rootExist = true;
            }
            else if (!root.Elements().Any(v => v.Name == id))
                root.Add(new XElement(id));

            root.Element(id).Value = rawValue;

            if (!isFileExists)
            {
                dirPath = Path.GetDirectoryName(path);
                System.IO.Directory.CreateDirectory(dirPath);
                var stream = System.IO.File.Create(path);
                stream.Dispose();
                isFileExists = true;
            }

            xml.Save(path);
        }

        #endregion
        #region ===============================================  Manager action

        public void Load()
        {
            isFileExists = System.IO.File.Exists(path);
            if (isFileExists)
            {
                var content = System.IO.File.ReadAllText(path);
                rootExist = content.Contains(this.id);
                if (rootExist)
                {
                    xml = XDocument.Parse(content);
                    root = xml.Element(this.id);
                }
            }
            else
            {
                xml = new XDocument();
                rootExist = false;
            }
        }

        public void WriteTip(string id, string tip)
        {
            TipWriter.WriteTipOnFile(path, id, tip);
        }

        public void ToggleMenu(bool active)
        {
            isMenuActive = active;
        }

        public void ManualUpdate()
        {
            OnPrefsUpdated?.Invoke();
        }

        public void SaveAll()
        {
            OnSave?.Invoke();
        }

        public void WriteTip()
        {
            OnWriteTips?.Invoke();
            xml = XDocument.Load(path);
            root = xml.Element(this.id);
        }

        public void ResetAll()
        {
            OnReset?.Invoke();
        }

        #endregion
        #region ===============================================  Path 

        public string GetFullFilePath()
        {
            return path;
        }

        public void SetFullFilePath(string path)
        {
            this.path = path;
            isFileExists = System.IO.File.Exists(path);
        }

        public string parentDirPathFromRootDir
        {
            get
            {

#if UNITY_EDITOR
                return parentDirPathFromRootDirEditor;
#else
                return parentDirPathFromRootDirStandalone;
#endif
            }
            set
            {

#if UNITY_EDITOR
                parentDirPathFromRootDirEditor = value;
                GeneratePath();
#else
                parentDirPathFromRootDirStandalone = value;
               GeneratePath();
#endif
            }
        }

        public string fileNameWithExtension
        {
            get
            {

#if UNITY_EDITOR
                return fileNameWithExtensionEditor;
#else
                return fileNameWithExtensionEditorStandalone;
#endif
            }
            set
            {

#if UNITY_EDITOR
                fileNameWithExtensionEditor = value;
                GeneratePath();
#else
                fileNameWithExtensionEditorStandalone = value;
               GeneratePath();
#endif
            }
        }


        #endregion

        #endregion
        #region < private >=========================================================================================================================< private >

        private void Awake()
        {
            NipaPrefsManagerInterface.RegisterManger(this);
        }

        void GeneratePath()
        {
#if UNITY_EDITOR
            path = PathProvider.GetPath(rootDirEditor, parentDirPathFromRootDirEditor, fileNameWithExtensionEditor);
#else
                   path = PathProvider.GetPath(rootDirStandalone, parentDirPathFromRootDirStandalone, fileNameWithExtensionEditorStandalone);
#endif
            isFileExists = System.IO.File.Exists(path);
        }

        Vector2 scroll;

        #region ===============================================  GUI

        void OnGUI()
        {
            if (isMenuActive)
                windowRect = GUILayout.Window(guiLayoutWindowId, windowRect, DebugWindow, string.Format("NipaPrefsMgr : {0}", id), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        }


        public void DebugWindow(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("close"))
                isMenuActive = false;
            GUILayout.EndHorizontal();

            GUILayout.BeginVertical();
            GUILayout.Label("file path : " + path);

            GUILayout.BeginHorizontal();
            if (isFileExists && GUILayout.Button("Open XML file"))
                System.Diagnostics.Process.Start(path);
            if (isFileExists && GUILayout.Button("Open directory"))
                System.Diagnostics.Process.Start(dirPath);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (enableManualUpdate && GUILayout.Button("Apply all values"))
                ManualUpdate();
            if (enableManualLoad && GUILayout.Button("Load"))
                OnLoad?.Invoke();
            if (GUILayout.Button("Save all"))
                SaveAll();
            if (isFileExists && rootExist && GUILayout.Button("Write tips"))
                WriteTip();
            if (GUILayout.Button("Reset all"))
                ResetAll();
            if (isFileExists && GUILayout.Button("Delete file"))
            {
                File.Delete(path);
                firstLoadDone = false;
            }
            GUILayout.EndHorizontal();

            isEditMode = GUILayout.Toggle(isEditMode, "Show all values");



            if (isEditMode)
            {
                scroll = GUILayout.BeginScrollView(scroll, GUILayout.MinHeight(valueEditorHeight));
                foreach (var item in valueIdAndGuis)
                {
                    item.Value();
                }
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
            GUI.DragWindow();
        }
        #endregion

        #endregion
    }
}