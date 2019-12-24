using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{

    [SerializeField] GUISkin skin;
    [SerializeField] WebsocketSender sender;
    [SerializeField] WebscoketReceiver reciever;


    Rect dubugWindow = new Rect(0, 0, Screen.width, Screen.height);

    void OnGUI()
    {
        GUI.skin = skin;
        dubugWindow = GUILayout.Window(-100, dubugWindow, DebugWindow, "DebugWindow", GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true));
    }

    void DebugWindow(int id)
    {
        GUILayout.BeginVertical();

        GUILayout.Label("Sender");
        using (var v = new GUILayout.VerticalScope("box"))
        {
            sender.Menu();
        }
        GUILayout.Label("Receiver");
        using (var v = new GUILayout.VerticalScope("box"))
        {
            reciever.Menu();
        }


        GUILayout.EndVertical();
        GUI.DragWindow();
    }
}
