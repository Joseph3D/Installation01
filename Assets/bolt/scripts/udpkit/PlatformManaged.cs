#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_PSM
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

#if !UNITY_WEBPLAYER
using System.Net.NetworkInformation;
#endif

namespace UdpKit {
  class UdpPrecisionTimer {
    static readonly long start = Stopwatch.GetTimestamp();
    static readonly double freq = 1.0 / (double) Stopwatch.Frequency;

    internal static uint GetCurrentTime () {
      long diff = Stopwatch.GetTimestamp() - start;
      double seconds = (double) diff * freq;
      return (uint) (seconds * 1000.0);
    }
  }

  public sealed class UdpPlatformManaged : UdpPlatform {
    Socket socket;
    EndPoint recvEndPoint;
    SocketError socketError;

    Socket broadcastSocket;
    IPEndPoint broadcastAddress;

    public UdpPlatformManaged () {
      socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      socket.Blocking = false;

      SetConnReset(socket);

      recvEndPoint = new IPEndPoint(IPAddress.Any, 0);
    }

    public override bool SupportsBroadcast {
      get { return true; }
    }

    public override UdpEndPoint EndPoint {
      get { return ConvertEndPoint((IPEndPoint) socket.LocalEndPoint); }
    }

    public override string PlatformErrorString {
      get { return socketError.ToString(); }
    }

    public override uint PlatformPrecisionTime {
      get { return UdpPrecisionTimer.GetCurrentTime(); }
    }

    public override bool IsBroadcasting {
      get { return broadcastSocket != null && broadcastSocket.IsBound; }
    }

    public override bool Close () {
      try {
        socket.Close();
        return true;
      } catch (SocketException exn) {
        socketError = exn.SocketErrorCode;
        return false;
      }
    }

    public override void DisableBroadcast () {
      try {
        broadcastSocket.Shutdown(SocketShutdown.Both);
      } catch { }

      try {
        broadcastSocket.Close();
      } catch { }

      broadcastSocket = null;
    }

    public override void EnableBroadcast (UdpEndPoint broadcast) {
      try {
        if (broadcastSocket != null) {
          DisableBroadcast();
        }

        // setup broadcast address
        broadcastAddress = ConvertEndPoint(broadcast);
        UdpLog.Info("setting broadcast address to {0}", broadcastAddress);

        // setup broadcast endpoint
        IPEndPoint broadcastEndPoint = new IPEndPoint(IPAddress.Any, broadcast.Port);
        UdpLog.Info("binding broadcast socket to {0}", broadcastEndPoint);

        // setup broadcast socket
        broadcastSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        broadcastSocket.Blocking = false;
        broadcastSocket.EnableBroadcast = true;
        broadcastSocket.Bind(broadcastEndPoint);

      } catch (Exception exn) {
        UdpLog.Error("could not enable broadcast, exception thrown: {0}", exn.ToString());
      }
    }

    public override void SendBroadcastData (byte[] buffer, int length) {
      if (broadcastSocket == null) {
        UdpLog.Error("broadcast socket not created");
        return;
      }

      broadcastSocket.SendTo(buffer, 0, length, SocketFlags.None, broadcastAddress);
    }

    public override bool RecvBroadcastData (byte[] buffer, out UdpEndPoint sender, out int bytes) {
      bytes = 0;
      sender = UdpEndPoint.Any;

      if (broadcastSocket == null) {
        UdpLog.Error("broadcast socket not created");
        return false;
      }

      if (broadcastSocket.Available > 0) {
        EndPoint remote = new IPEndPoint(IPAddress.Any, 0);
        bytes = broadcastSocket.ReceiveFrom(buffer, ref remote);
        sender = ConvertEndPoint((IPEndPoint) remote);
        return true;
      }

      return false;
    }

    public override bool Bind (UdpEndPoint endpoint) {
      try {
        socket.Bind(ConvertEndPoint(endpoint));
        return true;
      } catch (SocketException exn) {
#if UNITY_PSM
        // because sockets on PSVita are weird.
        if (exn.SocketErrorCode == SocketError.Fault) {
          return true;
        }
#endif

        socketError = exn.SocketErrorCode;
        return false;
      }
    }

    public override bool RecvPoll (int timeoutInMs) {
      try {
        return socket.Poll(timeoutInMs * 1000, SelectMode.SelectRead);
      } catch (SocketException exn) {
        socketError = exn.SocketErrorCode;
        return false;
      }
    }

    public override bool RecvFrom (byte[] buffer, int bufferSize, ref int bytesReceived, ref UdpEndPoint remoteEndpoint) {
      try {
        bytesReceived = socket.ReceiveFrom(buffer, 0, bufferSize, SocketFlags.None, ref recvEndPoint);

        if (bytesReceived > 0) {
          remoteEndpoint = ConvertEndPoint((IPEndPoint) recvEndPoint);
          return true;
        } else {
          return false;
        }
      } catch (SocketException exn) {
        socketError = exn.SocketErrorCode;
        return false;
      }
    }

    public override bool SendTo (byte[] buffer, int bytesToSend, UdpEndPoint endpoint, ref int bytesSent) {
      try {
        bytesSent = socket.SendTo(buffer, 0, bytesToSend, SocketFlags.None, ConvertEndPoint(endpoint));
        return bytesSent == bytesToSend;
      } catch (SocketException exn) {
        socketError = exn.SocketErrorCode;
        return false;
      }
    }

#pragma warning disable 618
    UdpEndPoint ConvertEndPoint (IPEndPoint endpoint) {
      return new UdpEndPoint(new UdpIPv4Address(endpoint.Address.Address), (ushort) endpoint.Port);
    }

    IPEndPoint ConvertEndPoint (UdpEndPoint endpoint) {
      return new IPEndPoint(new IPAddress(new byte[] { endpoint.Address.Byte3, endpoint.Address.Byte2, endpoint.Address.Byte1, endpoint.Address.Byte0 }), endpoint.Port);
    }
#pragma warning restore 618

    void SetConnReset (Socket s) {
      try {
        const uint IOC_IN      = 0x80000000;
        const uint IOC_VENDOR  = 0x18000000;
        uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
        s.IOControl((int) SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
      } catch { }
    }

#if !UNITY_WEBPLAYER
    static bool IsValidInterface (NetworkInterface nic, IPInterfaceProperties p) {
      foreach (var addr in p.GatewayAddresses) {
        byte[] bytes = addr.Address.GetAddressBytes();

        if (bytes.Length == 4 && bytes[0] != 0 && bytes[1] != 0 && bytes[2] != 0 && bytes[3] != 0) {
          return true;
        }
      }

      return false;
    }

    static IPAddress FindBroadcastAddress (bool strict) {
      NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

      foreach (NetworkInterface nic in nics) {
        switch (nic.NetworkInterfaceType) {
          case NetworkInterfaceType.Ethernet:
          case NetworkInterfaceType.Ethernet3Megabit:
          case NetworkInterfaceType.FastEthernetFx:
          case NetworkInterfaceType.FastEthernetT:
          case NetworkInterfaceType.Wireless80211:
          case NetworkInterfaceType.GigabitEthernet:

            if (nic.OperationalStatus == OperationalStatus.Up || nic.OperationalStatus == OperationalStatus.Unknown) {
              IPInterfaceProperties p = nic.GetIPProperties();

              if ((strict == false) || IsValidInterface(nic, p)) {
                foreach (UnicastIPAddressInformation address in p.UnicastAddresses) {
                  if (address.Address.AddressFamily == AddressFamily.InterNetwork) {
                    if ((p.DhcpServerAddresses.Count == 0) && (strict == false)) {
                      byte[] bytes = address.Address.GetAddressBytes();
                      bytes[3] = 255;
                      return new IPAddress(bytes);

                    } else {
                      byte[] dhcp = p.DhcpServerAddresses[0].GetAddressBytes();
                      byte[] mask = address.IPv4Mask.GetAddressBytes();
                      byte[] addr = new byte[4];

                      for (int i = 0; i < dhcp.Length; ++i) {
                        addr[i] = (byte) ((dhcp[i] & mask[i]) | ~mask[i]);
                      }
                      return new IPAddress(addr);
                    }

                  }
                }
              }
            }
            break;
        }
      }

      if (strict) {
        return FindBroadcastAddress(false);
      }

      return IPAddress.Any;
    }
#endif

#pragma warning disable 618
    static public UdpIPv4Address FindBroadcastAddress () {
#if UNITY_WEBPLAYER
      return new UdpIPv4Address(255, 255, 255, 255);
#else
      return new UdpIPv4Address(FindBroadcastAddress(true).Address);
#endif
    }
#pragma warning restore 618
  }
}
#endif