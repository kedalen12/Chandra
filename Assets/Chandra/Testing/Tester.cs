#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using Chandra;
using UnityEditor;
using UnityEngine;

public class Tester : MonoBehaviour
{
    [MenuItem("Tests/LoginTest")]
    public static void GetBestServerForTagLogin()
    {
      var server =    Orchestrator.GetBestServer("login");
      Debug.Log(server.ServerIdentifier);
    }
    [MenuItem("Tests/DefaultTest")]
    public static void GetBestServerForDefault()
    {
      var server =  Orchestrator.GetBestServer("default");
      Debug.Log(server.ServerIdentifier);

    }
    [MenuItem("Tests/InvalidTest")]
    public static void GetBestServerForInvalidTag()
    {
      var server =   Orchestrator.GetBestServer("invalid");
      Debug.Log(server is null);
    }
}
#endif