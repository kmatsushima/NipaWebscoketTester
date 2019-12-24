using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace NipaPrefs
{
    public static class PathProvider
    {
        #region ===============================================  Define

        public enum EditorRootDirectroy
        {
            ///<summary> "Assets" directory </summary>
            Assets,
            ///<summary> Assets/StreamingAssets directory </summary>
            StreamingAssets,
            ///<summary> parent directory of "Assets" directory </summary>
            ParentOfAssets
        }

        public enum StandaloneRootDirectroy
        {
            ///<summary> YourApp_Data directory in same directory YourApp.exe in </summary>
            Data,
            ///<summary> StreamingAssets directory </summary>
            StreamingAssets,
            ///<summary> C:\Users\you\AppData\LocalLow </summary>
            PersistentData,
            ///<summary> directory YourApp.exe in </summary>
            DirectoryExeFileIn,
            ///<summary>parent directory of directory YourApp.exe in </summary>
            ParentDirOfDirExeFileIn,
            ///<summary> MyDocuments </summary>
            MyDocuments,
        }

        #endregion

        public static string GetPath(EditorRootDirectroy rootDir, string parentDirPathFromRootDir, string fileNameWithExtension)
        {
            var path = "";
            switch (rootDir)
            {
                case EditorRootDirectroy.Assets:
                    path = Path.Combine(Application.dataPath, parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                case EditorRootDirectroy.ParentOfAssets:
                    path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                case EditorRootDirectroy.StreamingAssets:
                    path = Path.Combine(Application.streamingAssetsPath, parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                default:
                    break;
            }
            return path;
        }

        public static string GetPath(StandaloneRootDirectroy rootDir, string parentDirPathFromRootDir, string fileNameWithExtension)
        {
            var path = "";
            switch (rootDir)
            {
                case StandaloneRootDirectroy.Data:
                    path = Path.Combine(Application.dataPath, parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                case StandaloneRootDirectroy.DirectoryExeFileIn:
                    path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                case StandaloneRootDirectroy.ParentDirOfDirExeFileIn:
                    path = Path.Combine(Directory.GetParent(Directory.GetParent(Application.dataPath).FullName).FullName, parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                case StandaloneRootDirectroy.MyDocuments:
                    path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                case StandaloneRootDirectroy.StreamingAssets:
                    path = Path.Combine(Application.streamingAssetsPath, parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                case StandaloneRootDirectroy.PersistentData:
                    path = Path.Combine(Application.persistentDataPath, parentDirPathFromRootDir, fileNameWithExtension);
                    break;
                default:
                    break;
            }
            return path;
        }
    }
}