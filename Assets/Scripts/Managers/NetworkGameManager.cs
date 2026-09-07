using Unity.Netcode;
using UnityEngine;

public class NetworkGameManager : NetworkBehaviour
{
    //this manager will handle networked fields, like score, and id
    public static NetworkGameManager instance;

    public NetworkVariable<uint> currentID;

    private void Awake()
    {
        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //currentID.Value = 0;
    }

    public uint ClaimID()
    {
        uint oldID = currentID.Value;
        //currentID.Value++; //probably need an rpc
        IncrementIDServerRpc();
        return oldID;
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void IncrementIDServerRpc()
    {
        Debug.Log("claim id:" + currentID.Value);
        currentID.Value++;
    }




}
