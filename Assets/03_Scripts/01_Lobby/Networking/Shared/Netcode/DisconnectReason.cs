using System;

using UnityEngine;

public class DisconnectReason
{
    public ConnectStatus Reason { get; private set; } = ConnectStatus.Undefined;

    public void SetDisconnectReason(ConnectStatus reason)
    {
        Debug.Assert(condition: reason != ConnectStatus.Success);
        Reason = reason;
    }

    public void Clear() { Reason = ConnectStatus.Undefined; }

    public Boolean HasTransitionReason => Reason != ConnectStatus.Undefined;
}