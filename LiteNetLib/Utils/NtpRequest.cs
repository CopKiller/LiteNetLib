﻿using System.Net;
using System.Net.Sockets;

namespace LiteNetLib.Utils;

internal sealed class NtpRequest
{
    private const int ResendTimer = 1000;
    private const int KillTimer = 10000;
    public const int DefaultPort = 123;
    private readonly IPEndPoint _ntpEndPoint;
    private float _resendTime = ResendTimer;
    private float _killTime = 0;

    public NtpRequest(IPEndPoint endPoint)
    {
        _ntpEndPoint = endPoint;
    }

    public bool NeedToKill => _killTime >= KillTimer;

    public bool Send(Socket socket, float time)
    {
        _resendTime += time;
        _killTime += time;
        if (_resendTime < ResendTimer) return false;
        var packet = new NtpPacket();
        try
        {
            var sendCount = socket.SendTo(packet.Bytes, 0, packet.Bytes.Length, SocketFlags.None, _ntpEndPoint);
            return sendCount == packet.Bytes.Length;
        }
        catch
        {
            return false;
        }
    }
}
