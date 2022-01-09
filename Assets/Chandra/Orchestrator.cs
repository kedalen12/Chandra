using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Chandra;
using Chandra.Interfaces;
using UnityEditor;
using UnityEngine;

public static class Orchestrator
{
    public static Dictionary<string,IChandraThread> Threads = new Dictionary<string, IChandraThread>();
    private static bool isFirstCall = true;
    public static void AddToOrCreateThread(IServer server, byte ticksPerSecond = 10)
    {
        if (isFirstCall)
        {
            Application.quitting += ApplicationQuit;
            isFirstCall = false;
        }
        if (Threads.ContainsKey(server.Tag))
        {
            if (Threads[server.Tag].IsAlive)
            {
                Threads[server.Tag].AddServer(server);
                return;
            }
            Threads[server.Tag].Start(server);
            return;
        }
        
        ticksPerSecond = ticksPerSecond > 100 ? (byte) 100 : ticksPerSecond;
        Threads.Add(server.Tag, new ChandraThread(ticksPerSecond)); 
        Threads[server.Tag].Start(server);
    }
    private static void ApplicationQuit()
    {
        var values = Threads.Values.ToList();
        for (var i = 0; i < values.Count; i++)
        {
            values[i].Stop();
        }
    }
    
    public static bool TryGetServer(string tag, int serverId, out IServer server)
    {
        if (Threads.ContainsKey(tag))
        {
            server = Threads[tag].GetServer(serverId);
            return true;
        }

        server = null;
        return false;
    }

    public static IServer GetBestServer(string tag)
    {
        return Threads.ContainsKey(tag) ? Threads[tag].GetBestServer() : null;
    }

    public static IServer CloneServer(IServer server)
    {
        var gameObject = server.GetUnityObject.gameObject;
        var serverAsComponent = server as Component;
        var components = gameObject.GetComponents(serverAsComponent!.GetType()).First();
        gameObject.AddComponent(components.GetType());
        
        return components as IServer;
    }
}