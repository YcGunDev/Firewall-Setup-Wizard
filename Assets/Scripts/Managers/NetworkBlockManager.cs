using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NetworkBlockManager : NetworkBehaviour
{
    public static NetworkBlockManager instance;

    public GameObject block;
    private void Awake()
    {
        instance = this;
        Debug.Log("Network Block Manager");
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost) //host
        {
            gameObject.layer = LayerMask.NameToLayer("Player1");
        }
        else //client
        {
            gameObject.layer = LayerMask.NameToLayer("Player2");
        }
    }

    //public void RequestSpawnBlock(GameObject block, Vector3 spawnPos, Quaternion spawnRot)
    //{
    //    Debug.Log("IsHost: " + IsHost);
    //    Debug.Log("IsClient: " + IsClient);
    //    if (IsHost) //host
    //    {
    //        block.GetComponent<NetworkObject>().Spawn();
    //    }
    //    else //client
    //    {
    //        RequestSpawnBlockServerRpc(spawnPos, spawnRot);

    //        //if (block.TryGetComponent<NetworkObject>(out var netObject))
    //        //{
    //        //    // Implicitly converts GameObject/NetworkObject to NetworkObjectReference
    //        //    RequestSpawnBlockServerRpc(netObject);
    //        //}
    //    }
    //}

    public void RequestSpawnBlock(Vector3 spawnPos, Quaternion spawnRot, int id)
    {
        RequestSpawnBlockServerRpc(spawnPos, spawnRot, id, gameObject.layer);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestSpawnBlockServerRpc(Vector3 spawnPos, Quaternion spawnRot, int id, int blockOwner)
    {
        Debug.Log("Spawn block");
        // This code executes strictly on the Server
        GameObject currentBlock = Instantiate(block, spawnPos, spawnRot);
        Block b = currentBlock.GetComponent<Block>();
        b.id.Value = id;
        b.blockLayer.Value = blockOwner;


        /*
         * OKAY so if i comment out the lines below, the spawning IS replicated, but the movement is not.
         * If I uncomment the lines below, the spawning is duplicated but the movement IS replicated.
         * FOR CLIENT
         * host works completely fine
         
         */

        // 2. Get the NetworkObject component
        NetworkObject networkBlock = currentBlock.GetComponent<NetworkObject>();

        // 3. Spawn across the network to all clients
        networkBlock.Spawn();
    }

    public void RequestMoveBlock(float speed, Vector3 direction, int health, int id)
    {
        RequestMoveBlockMulticastRpc(speed, direction, health, id);
    }
    
    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestMoveBlockMulticastRpc(float speed, Vector3 direction, int health, int id)
    {
        Debug.Log("Move block");
        // This code executes strictly on the Server
        Block b = BlockManager.instance.FindBlock(id);
    
        b.speed = speed;
        b.direction = direction;
        if (IsHost)
        {
            //b.health.Value = health;
            RequestDamageBlock(id, b.health.Value - health);
        }
    }

    public void RequestDamageBlock(int id, int damage)
    {
        RequestDamageBlockMulticastrRpc(id, damage);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestDamageBlockMulticastrRpc(int id, int damage)
    {
        Debug.Log("Damage block");
        // This code executes strictly on the Server
        Block a = BlockManager.instance.FindBlock(id);
        a.TakeDamage(damage);
        a.UpdateHPUI();
    }
}
