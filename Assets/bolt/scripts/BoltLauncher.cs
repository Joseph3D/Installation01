using UnityEngine;
using UdpKit;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

public static class BoltLauncher {
  public static void StartServer() {
    StartServer(UdpEndPoint.Any);
  }

  public static void StartServer(BoltConfig config) {
    StartServer(UdpEndPoint.Any, config);
  }

  public static void StartServer(UdpEndPoint endpoint) {
    StartServer(endpoint, BoltRuntimeSettings.instance.GetConfigCopy());
  }

  public static void StartServer(UdpEndPoint endpoint, BoltConfig config) {
    Initialize(BoltNetworkModes.Server, endpoint, config);
  }

  public static void StartClient() {
    StartClient(UdpEndPoint.Any);
  }

  public static void StartClient(BoltConfig config) {
    StartClient(UdpEndPoint.Any, config);
  }

  public static void StartClient(UdpEndPoint endpoint) {
    StartClient(endpoint, BoltRuntimeSettings.instance.GetConfigCopy());
  }

  public static void StartClient(UdpEndPoint endpoint, BoltConfig config) {
    Initialize(BoltNetworkModes.Client, endpoint, config);
  }

  public static void Shutdown() {
    BoltNetworkInternal.__Shutdown();
  }

  static void Initialize(BoltNetworkModes modes, UdpEndPoint endpoint, BoltConfig config) {
    BoltNetworkInternal.DebugDrawer = new BoltInternal.UnityDebugDrawer();

#if UNITY_PRO_LICENSE
    BoltNetworkInternal.UsingUnityPro = true;
#else
    BoltNetworkInternal.UsingUnityPro = false;
#endif

#if BOLT_UPNP_SUPPORT
    BoltNetworkInternal.NatCommunicator = new BoltInternal.StandaloneNatCommunicator();
#endif

    BoltNetworkInternal.CreateUdpPlatform = BoltNetworkUtils.CreateUdpPlatform;
    BoltNetworkInternal.GetBroadcastAddress = BoltNetworkUtils.FindBroadcastAddress;
    BoltNetworkInternal.GetSceneName = GetSceneName;
    BoltNetworkInternal.GetSceneIndex = GetSceneIndex;
    BoltNetworkInternal.GetGlobalBehaviourTypes = GetGlobalBehaviourTypes;
    BoltNetworkInternal.EnvironmentSetup = BoltInternal.BoltNetworkInternal_User.EnvironmentSetup;
    BoltNetworkInternal.EnvironmentReset = BoltInternal.BoltNetworkInternal_User.EnvironmentReset;
    BoltNetworkInternal.__Initialize(modes, endpoint, config);
  }

  static int GetSceneIndex(string name) {
    return BoltInternal.BoltScenes_Internal.GetSceneIndex(name);
  }

  static string GetSceneName(int index) {
    return BoltInternal.BoltScenes_Internal.GetSceneName(index);
  }

  static public List<STuple<BoltGlobalBehaviourAttribute, Type>> GetGlobalBehaviourTypes() {
    Assembly asm = Assembly.GetExecutingAssembly();
    List<STuple<BoltGlobalBehaviourAttribute, Type>> result = new List<STuple<BoltGlobalBehaviourAttribute, Type>>();

    try {
      foreach (Type type in asm.GetTypes()) {
        if (typeof(MonoBehaviour).IsAssignableFrom(type)) {
          var attrs = (BoltGlobalBehaviourAttribute[])type.GetCustomAttributes(typeof(BoltGlobalBehaviourAttribute), false);
          if (attrs.Length == 1) {
            result.Add(new STuple<BoltGlobalBehaviourAttribute, Type>(attrs[0], type));
          }
        }
      }
    }
    catch {
      // just eat this, a bit dangerous but meh.
    }

    return result;
  }
}
