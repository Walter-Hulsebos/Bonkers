using System;

[Serializable]
public class ClientData
{
    public UInt64 clientId;
    public Int32  characterId = -1;

    public ClientData(UInt64 clientId) => this.clientId = clientId;
}