using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using NipaPrefs;
using System;
public class WebsocketSender : MonoBehaviour
{
    NipaString ip = new NipaString(Param.PREFS, "ipAddress", "localhost");
    static NipaInt port = new NipaInt(Param.PREFS, "port", 6666);
    protected WebSocket client = null;
    bool active;
    List<string> log = new List<string>();
    string content;

    void Open()
    {
        this.Close();
        var url = string.Format("ws://{0}:{1}/", ip, port);
        active = true;
        try
        {
            this.client = new WebSocket(url);
            this.client.OnOpen += (sender, args)
                => log.Add(Param.prefix + "WebSocket Client OnOpen");
            this.client.OnClose += (sender, args)
                            => log.Add(Param.prefix + "WebSocket Client OnClose (" + args.Code + ", " + args.Reason + ")");
            this.client.OnError += (sender, args)
                => log.Add(Param.prefix + "WebSocket Client OnError (" + args.Exception + ", " + args.Message + ")");

            this.client.OnOpen += (sender, args)
                => log.Add(Param.prefix + "Opend");
            this.client.OnClose += (sender, args)
                => log.Add(Param.prefix + "Closed");
            this.client.OnError += (sender, args)
                => log.Add(Param.prefix + "Error");

            this.client.ConnectAsync();
        }
        catch (Exception e)
        {
            log.Add(Param.prefix + e.ToString());
            this.Close();
        }
    }
    private void Update()
    {
        while (log.Count > 10)
        {
            log.RemoveAt(0);
        }
    }
    void Close()
    {
        this.client?.Close();
        this.client = null;
        active = false;
    }
    Vector2 guiScrollPosition;
    public void Menu()
    {
        ip.OnGUI();
        port.OnGUI();

        if (!active && GUILayout.Button("Open"))
            Open();
        if (active && GUILayout.Button("Close"))
            Close();

        if (active)
        {
            using (var v = new GUILayout.VerticalScope("box"))
            {
                GUILayout.Label("string to send");
                content = GUILayout.TextField(content);
                if (GUILayout.Button("Send"))
                    this.client.SendAsync(content, null);
            }
        }


        if (active)
            using (var v = new GUILayout.VerticalScope("box"))
            {
                GUILayout.Label("log");
                guiScrollPosition = GUILayout.BeginScrollView(guiScrollPosition, GUILayout.MinHeight(200f));
                var l = "";
                foreach (var item in log)
                {
                    l += item + System.Environment.NewLine;
                }
                GUILayout.TextArea(l);

                GUILayout.EndScrollView();
            }



    }
}
