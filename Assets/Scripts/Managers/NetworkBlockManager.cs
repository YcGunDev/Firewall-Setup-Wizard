using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NetworkBlockManager : NetworkBehaviour
{
    public static NetworkBlockManager instance;

    public GameObject block;
    public GameObject alliedSpawnArea;
    [SerializeField] GameObject NGM;
    private void Awake()
    {
        instance = this;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsHost) //host
        {
            gameObject.layer = LayerMask.NameToLayer("Player1");
            alliedSpawnArea = ObjectManager.instance.spawnAreaP1;

            if (IsOwnedByServer)
            {
                //1. spawn it
                GameObject ngm = Instantiate(NGM, Vector3.zero, Quaternion.identity);

                // 2. Get the NetworkObject component
                NetworkObject _ngm = ngm.GetComponent<NetworkObject>();

                // 3. Spawn across the network to all clients
                _ngm.Spawn();
            }
            
        }
        else //client
        {
            gameObject.layer = LayerMask.NameToLayer("Player2");
            alliedSpawnArea = ObjectManager.instance.spawnAreaP2;
        }


        //if (NetworkGameManager.instance == null)
        //{
            
        //}
    }



    public void RequestSpawnBlock(Vector3 spawnPos, Quaternion spawnRot, uint id)
    {
        RequestSpawnBlockServerRpc(spawnPos, spawnRot, id, gameObject.layer);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestSpawnBlockServerRpc(Vector3 spawnPos, Quaternion spawnRot, uint id, int blockOwner)
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

    public void RequestMoveBlock(float speed, Vector3 direction, int health, uint id)
    {
        RequestMoveBlockMulticastRpc(speed, direction, health, id);
    }
    
    [Rpc(SendTo.ClientsAndHost, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestMoveBlockMulticastRpc(float speed, Vector3 direction, int health, uint id)
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

    public void RequestDamageBlock(uint id, int damage)
    {
        RequestDamageBlockMulticastRpc(id, damage);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestDamageBlockMulticastRpc(uint id, int damage)
    {
        Debug.Log("Damage block: " + damage);
        // This code executes strictly on the Server
        Block a = BlockManager.instance.FindBlock(id);
        a.TakeDamage(damage);
        a.UpdateHPUI();
    }

    //this one is special and will use the network id
    public void RequestSetBlockID(ulong netID, uint newID)
    {
        RequestSetBlockIDServerRpc(netID, newID);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestSetBlockIDServerRpc(ulong netID, uint newID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(netID, out NetworkObject networkObject))
        {
            GameObject targetGo = networkObject.gameObject;
            targetGo.GetComponent<Block>().id.Value = newID;
            Debug.Log($"Found object: {targetGo.name}, setting id to " + newID);
        }
        else
        {
            Debug.LogWarning($"No network object found with ID: {netID}");
        }
    }
}
