using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct ScoreModel : INetworkSerializable,IEquatable<ScoreModel>
{

    public FixedString32Bytes Name;
    public int CurrencyVal;
    public int Health;
    public int KillCount;
    public ulong ClientId;


    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref CurrencyVal);
        serializer.SerializeValue(ref Health);
        serializer.SerializeValue(ref KillCount);
        serializer.SerializeValue(ref ClientId);
    }

    public bool Equals(ScoreModel other)
    {
        if(Name.Equals(other.Name) && CurrencyVal == other.CurrencyVal && other.ClientId==ClientId) return true;

        return false;
    }




}
