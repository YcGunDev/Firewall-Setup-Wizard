using Unity.Netcode;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class NetworkProxy : NetworkBehaviour
{
    //rn we're getting away with this because there can only be one instance, so the other proxy that spawns isnt used, i might want to spawn in the
    //proxy the same way the game manager is being spawned in
    public static NetworkProxy instance;

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
        Block b = EntityManager.instance.FindBlock(id);
    
        b.speed = speed;
        b.direction = direction;
        if (IsHost)
        {
            NetworkHealthComponent h = b.GetComponent<NetworkHealthComponent>();
            if (h != null)
                RequestDamageEntity(id, h.health.Value - health);
        }
    }

    public void RequestDamageEntity(uint id, int damage)
    {
        RequestDamageHealthComponentServerRpc(id, damage);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestDamageHealthComponentServerRpc(uint id, int damage)
    {
        Debug.Log("Damage entity: " + damage);
        // This code executes strictly on the Server
        NetworkHealthComponent a = EntityManager.instance.FindEntity(id);
        if (a != null) a.TakeDamage(damage);
    }
    //i think the damage is getting replicated fairly well, i think the only entity that isnt getting damage replicated is the base walls,
    //i might want to rework damage so its more seamless like having a shared base class

    //this one is special and will use the network id
    public void RequestSetEntityID(ulong netID, uint newID)
    {
        RequestSetEntityIDServerRpc(netID, newID);
    }

    [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
    private void RequestSetEntityIDServerRpc(ulong netID, uint newID)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(netID, out NetworkObject networkObject))
        {
            GameObject targetGo = networkObject.gameObject;
            Block b = targetGo.GetComponent<Block>();
            
            if (b != null)
                b.id.Value = newID;
            else
            {
                BaseWall bw = targetGo.GetComponent<BaseWall>();
                if (b != null)
                    bw.id.Value = newID;
            }

            Debug.Log($"Found object: {targetGo.name}, setting id to " + newID);
        }
        else
        {
            Debug.LogWarning($"No network object found with ID: {netID}");
        }
    }



    //theres a good chance i will need to do a bit of snapshot replication just to make sure things dont get too desynced
    //but maybe later
}
