#if (UNITY_ANDROID || UNITY_IPHONE || UNITY_WP8) && !UNITY_EDITOR
using System;
using System.Security;
using System.Runtime.InteropServices;

namespace UdpKit {
  public sealed class UdpPlatformMobile : UdpPlatform {
    public delegate Int32 Udp_RecvFrom (IntPtr socket, byte[] data, int size, out UdpEndPoint ep);
    public delegate Int32 Udp_GetEndPoint (IntPtr socket, out UdpEndPoint ep);
	
#if UNITY_ANDROID
    public const string DLL_NAME = "udpkit_android";
#elif UNITY_IPHONE
    public const string DLL_NAME = "__Internal";
#endif

	  public const int UDPKIT_SOCKET_OK = 0;
	  public const int UDPKIT_SOCKET_ERROR = -1;
	  public const int UDPKIT_SOCKET_NOTVALID = -2;
	  public const int UDPKIT_SOCKET_NODATA = -3;

#if UNITY_WP8
    public static System.Func<IntPtr> udpCreate;
	  public static System.Func<IntPtr, UdpEndPoint, Int32> udpBind;
    public static System.Func<IntPtr, Int32> udpEnableBroadcast;
	  public static System.Func<IntPtr, byte[], int, UdpEndPoint, Int32> udpSendTo;
	  public static Udp_RecvFrom udpRecvFrom;
    public static System.Func<IntPtr, Int32, Int32> udpRecvPoll;
    public static System.Func<IntPtr, Int32> udpLastError;
    public static Udp_GetEndPoint udpGetEndPoint;
	  public static System.Func<IntPtr, Int32> udpClose;
    public static System.Func<string> udpGetPlatform;
    public static System.Func<string> udpErrorString;
    public static System.Func<UInt32> udpGetHighPrecisionTime;
    public static System.Func<UInt32> udpFindBroadcastAddress;
#else
	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern IntPtr udpCreate ();

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern Int32 udpBind (IntPtr socket, UdpEndPoint addr);

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern Int32 udpEnableBroadcast (IntPtr socket);

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern Int32 udpSendTo (IntPtr socket, [Out] byte[] buffer, int size, UdpEndPoint addr);

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern Int32 udpRecvFrom (IntPtr socket, [Out] byte[] buffer, int size, [Out] out UdpEndPoint addr);

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern Int32 udpRecvPoll (IntPtr socket, int timeout);

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern Int32 udpLastError (IntPtr socket);

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
    public static extern Int32 udpGetEndPoint (IntPtr socket, [Out] out UdpEndPoint addr);

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern Int32 udpClose (IntPtr socket);

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  static extern IntPtr udpPlatform ();
	  public static string udpGetPlatform () { return Marshal.PtrToStringAnsi(udpPlatform()); }

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  static extern IntPtr udpPlatformError ();
	  public static string udpErrorString () { return Marshal.PtrToStringAnsi(udpPlatformError()); }

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern UInt32 udpGetHighPrecisionTime ();

	  [DllImport(DLL_NAME)]
	  [SuppressUnmanagedCodeSecurity]
	  public static extern UInt32 udpFindBroadcastAddress ();
#endif

    IntPtr sptr;
    IntPtr bcptr;
    UdpEndPoint bcendpoint;

    public override UdpEndPoint EndPoint {
      get {
        UdpEndPoint ep = default(UdpEndPoint);

        if (udpGetEndPoint(sptr, out ep) == UDPKIT_SOCKET_OK) {
          return ep;
        }

        return UdpEndPoint.Any;
      }
    }

    public override bool SupportsBroadcast {
      get { return true; }
    }

    public override string PlatformErrorString {
      get { return udpErrorString(); }
    }

    public override uint PlatformPrecisionTime {
      get { return udpGetHighPrecisionTime(); }
    }

    public UdpPlatformMobile () {
      sptr = udpCreate();
      bcptr = IntPtr.Zero;
    }

    public override bool IsBroadcasting {
      get { return bcptr != IntPtr.Zero; }
    }

    public override void DisableBroadcast () {
      if (bcptr != IntPtr.Zero) {
        udpClose(bcptr);
        bcptr = IntPtr.Zero;
      }
    }

    public override void EnableBroadcast (UdpEndPoint address) {
      if (bcptr != IntPtr.Zero) {
        DisableBroadcast();
      }

      bcendpoint = address;
      bcptr = udpCreate();
      UdpLog.Info("setting broadcast address to {0}", bcendpoint);

      UdpEndPoint bcSocketEndPoint = new UdpEndPoint(UdpIPv4Address.Any, address.Port);
      udpEnableBroadcast(bcptr);
      udpBind(bcptr, bcSocketEndPoint);
      UdpLog.Info("bound broadcast socket to {0}", bcSocketEndPoint);
    }

    public override void SendBroadcastData (byte[] buffer, int bytes) {
      int b = 0;
      SendTo(bcptr, buffer, bytes, bcendpoint, ref b);
    }

    public override bool RecvBroadcastData (byte[] buffer, out UdpEndPoint sender, out int bytes) {
      sender = new UdpEndPoint();
      bytes = 0;

      if (udpRecvPoll(bcptr, 1) != UDPKIT_SOCKET_OK) {
        return false;
      }

      return RecvFrom(bcptr, buffer, buffer.Length, ref bytes, ref sender);
    }

    public override bool Close () {
      return udpClose(sptr) == UDPKIT_SOCKET_OK;
    }

    public override bool Bind (UdpEndPoint endpoint) {
      return udpBind(sptr, endpoint) == UDPKIT_SOCKET_OK;
    }

    public override bool RecvPoll (int timeoutInMs) {
      return udpRecvPoll(sptr, timeoutInMs) == UDPKIT_SOCKET_OK;
    }

    public override bool RecvFrom (byte[] buffer, int bufferSize, ref int bytesReceived, ref UdpEndPoint remoteEndpoint) {
      return RecvFrom(sptr, buffer, bufferSize, ref bytesReceived, ref remoteEndpoint);
    }

    bool RecvFrom (IntPtr ptr, byte[] buffer, int bufferSize, ref int bytesReceived, ref UdpEndPoint remoteEndpoint) {
      UdpEndPoint nativeEndpoint = default(UdpEndPoint);
      bytesReceived = udpRecvFrom(ptr, buffer, bufferSize, out nativeEndpoint);

      if (bytesReceived > 0) {
        remoteEndpoint = nativeEndpoint;
        return true;
      }

      return false;
    }

    public override bool SendTo (byte[] buffer, int bytesToSend, UdpEndPoint endpoint, ref int bytesSent) {
      return SendTo(sptr, buffer, bytesToSend, endpoint, ref bytesSent);
    }

#if UNITY_IPHONE
	public static UdpIPv4Address FindBroadcastAddress_IOS() {
      uint addr = udpFindBroadcastAddress();
      return new UdpIPv4Address ((byte) (addr >> 0), (byte) (addr >> 8), (byte) (addr >> 16), (byte) (addr >> 24));
    }
#endif

    bool SendTo (IntPtr ptr, byte[] buffer, int bytesToSend, UdpEndPoint endpoint, ref int bytesSent) {
	  if (bytesToSend == (bytesSent = udpSendTo(ptr, buffer, bytesToSend, endpoint))) {
	    return true;
	  } else {
	    UdpLog.Info(udpErrorString());
	    return false;
	  }
    }
  }
}
#endif