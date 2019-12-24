using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using NipaPrefs;

public class WebscoketReceiver : MonoBehaviour
{
    static NipaInt receivePort = new NipaInt(Param.PREFS, "receivePort", 8888);

    protected WebSocketServer ws = null;
    [SerializeField] int port;
    bool active;
    List<string> log = new List<string>();
    List<string> systemLog = new List<string>();
    void Open()
    {
        this.ws = new WebSocketServer(receivePort);
        this.ws.AddWebSocketService<ReveiveBehavior>("/");
        this.ws.Start();
        this.ws.Log.Level = LogLevel.Debug;
        this.ws.Log.Output += (data, l) => systemLog.Add(Param.prefix + data.Message);
        active = true;
    }

    private void Close()
    {
        this.ws?.Stop();
        this.ws = null;
        active = false;
    }

    private void Update()
    {
        while (ReveiveBehavior.log.Count > 0)
        {
            log.Add(Param.prefix + ReveiveBehavior.log.Dequeue());
        }
        while (log.Count > 10)
        {
            log.RemoveAt(0);
        }
        while (systemLog.Count > 10)
        {
            systemLog.RemoveAt(0);
        }
    }
    Vector2 guiScrollPosition;
    Vector2 guiScrollPosition2;
    public void Menu()
    {
        receivePort.OnGUI();


        if (!active && GUILayout.Button("Open"))
            Open();
        if (active && GUILayout.Button("Close"))
            Close();

        if (active)
            using (var v = new GUILayout.VerticalScope("box"))
            {
                GUILayout.Label("Received");
                guiScrollPosition = GUILayout.BeginScrollView(guiScrollPosition, GUILayout.MinHeight(200f));
                var l = "";
                foreach (var item in log)
                {
                    l += item + System.Environment.NewLine;
                }
                GUILayout.TextArea(l);
                GUILayout.EndScrollView();
                GUILayout.Label("log");
                guiScrollPosition2 = GUILayout.BeginScrollView(guiScrollPosition2, GUILayout.MinHeight(200f));
                l = "";
                foreach (var item in systemLog)
                {
                    l += item + System.Environment.NewLine;
                }
                GUILayout.TextArea(l);
                GUILayout.EndScrollView();
            }


    }
}
