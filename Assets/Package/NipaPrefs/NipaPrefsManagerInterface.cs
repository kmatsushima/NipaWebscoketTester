using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Linq;

namespace NipaPrefs
{
    public static class NipaPrefsManagerInterface
    {
        public static event System.Action<NipaPrefsManager> OnManagerReady;
        static Dictionary<string, NipaPrefsManager> managers = new Dictionary<string, NipaPrefsManager>();

        public static bool GetManager(string managerId, out NipaPrefsManager manager)
        {
            if (!managers.ContainsKey(managerId))
            {
                manager = null;
                return false;
            }
            else
            {
                manager = managers[managerId];
                return true;
            }
        }

        ///<summary>  must be called from Awake or later, because of FindObjectsOfTypeAll </summary>
        public static NipaPrefsManager GetManager(string managerId)
        {
            if (!managers.ContainsKey(managerId))
            {
                var allDatabases = Resources.FindObjectsOfTypeAll<MonoBehaviour>()
                        .Select(b => b.GetComponent<NipaPrefsManager>())
                        .Where(o => o != null);

                foreach (var item in allDatabases)
                    RegisterManger(item);
            }

            if (managers.ContainsKey(managerId))
                return managers[managerId];
            else
                return null;
        }

        public static void RegisterManger(NipaPrefsManager manager)
        {
            if (managers.ContainsKey(manager.id))
                return;
            managers.Add(manager.id, manager);
            OnManagerReady?.Invoke(manager);
        }
    }
}