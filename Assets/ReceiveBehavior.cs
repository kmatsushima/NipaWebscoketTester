using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class ReveiveBehavior : WebSocketBehavior
{
    public static Queue<string> log = new Queue<string>();
    protected override void OnMessage(MessageEventArgs e)
    {
            var d = e.Data;
        log.Enqueue(d);
        while (log.Count > 10)
        {
            log.Dequeue();
        }
    }

    protected override void OnOpen()
    {
   //     UnityLogger.Log("WebSocketServer OnOpen" + this.GetUrlStr());
        base.OnOpen();
    }

    protected override void OnClose(CloseEventArgs e)
    {
     //   UnityLogger.Warning("WebSocketServer OnClose (" + e.Code + ", " + e.Reason + ")" + this.GetUrlStr());
        base.OnClose(e);
    }

    protected override void OnError(ErrorEventArgs e)
    {
      //  UnityLogger.Error("WebSocketServer OnError (" + e.Exception + ", " + e.Message + ")" + this.GetUrlStr());
        base.OnError(e);
    }
}