using System;
using System.Collections;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;
using static Unity.Collections.AllocatorManager;
using static UnityEngine.CullingGroup;

public class Block : NetworkBehaviour, ITakeDamage
{
    //I'm personally not sold on this, but atm its the easiest method and its working, so go get em tiger
    public NetworkVariable<int> id;
    public NetworkVariable<int> health = new NetworkVariable<int>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);
    public float speed;
    public Vector2 direction; //this will always be normalized

    public NetworkVariable<int> blockLayer;

    public float drag = 0.005f;

    //there is a chance i may want to replicate all these values, and not rely on unity's network transform, the motion is smooth but not perfect

    public CollisionHandler sharedHandler = null;

    //objects
    [SerializeField] Rigidbody2D rb;
    [SerializeField] TextMeshProUGUI healthUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.OnValueChanged += OnHPChange; //health may need to be changed from a network variable
        gameObject.layer = blockLayer.Value;
    }

    //public override void OnNetworkDespawn()
    //{
    //    BlockManager.instance.RemoveBlock(this);
    //}

    void Awake()
    {
        if (health.Value <= 0) health.Value = 100;
        //speed = 0.0f;
        //direction = Vector2.zero;

        UpdateHPUI();

        BlockManager.instance.AddBlock(this);

        //NetworkBlockManager.instance.RequestSpawnBlock(gameObject);
        //NetworkBlockManager.instance.RequestSpawnBlockServerRpc(GetComponent<NetworkObject>());

        /*
         * the problem with this system is that while the host can easily replicate the blocks it creates, the client will never be able to 
         * send/request for the block to replicate.
         * RPCs can only be called after an object has bee replicated over the network
         * because the RPC or replication request comes from the block itself, its an infinite loop of
         * needing to replicate so it sends an RPC, but to send an RPC it needs to replicate
         * 
         * I may need to do a proxy kind of replication where the block requests a replicated class to replicate the block for it
         
         */
    }

    private void Start()
    {
        BlockManager.instance.ReplaceSpacer(id.Value);
        
    }

    

    public override void OnDestroy()
    {
        base.OnDestroy();
        BlockManager.instance.RemoveBlock(this);
    }

    //public void OnDestroy()
    //{
    //    BlockManager.instance.RemoveBlock(this);
    //}

    void Update()
    {
        rb.linearVelocity = direction * speed;


        speed *= 1 - (drag * Time.deltaTime);

        if (speed <= 0.05f) speed = 0;
    }

    public void TakeDamage(int damage)
    {
        health.Value -= Mathf.Abs(damage);
        UpdateHPUI();
        if (health.Value <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnHPChange(int previousValue, int newValue)
    {
        UpdateHPUI();
    }

    public void UpdateHPUI()
    {
        healthUI.text = health.Value.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (sharedHandler != null) return;

        //allied
        if (collision.gameObject.layer == gameObject.layer)
        {
            Block otherBlock = collision.gameObject.GetComponent<Block>();
            if (otherBlock != null)
            {
                sharedHandler = Instantiate(CollisionManager.instance.collisionHandler).GetComponent<CollisionHandler>();
                otherBlock.sharedHandler = sharedHandler;
                sharedHandler.A = this;
                sharedHandler.B = otherBlock;
                sharedHandler.ProcessCollision(1);
            }
            else
            {
                //its probably an obstacle of some kind, bounce
                direction = Vector2.Reflect(direction, collision.GetContact(0).normal);
                speed *= 0.9f;
            }
            return;
        }

        //opposing
        ITakeDamage itd = collision.gameObject.GetComponent<ITakeDamage>();
        if (itd != null)
        {
            //its a block
            Block otherBlock = collision.gameObject.GetComponent<Block>();
            if (otherBlock != null)
            {
                sharedHandler = Instantiate(CollisionManager.instance.collisionHandler).GetComponent<CollisionHandler>();
                otherBlock.sharedHandler = sharedHandler;
                sharedHandler.A = this;
                sharedHandler.B = otherBlock;
                sharedHandler.ProcessCollision(0);
            }
            //its a wall
            else
            {
                itd.TakeDamage(health.Value);
                TakeDamage(health.Value);
            }
            return;
        }

        //its probably an obstacle of some kind, bounce
        direction = Vector2.Reflect(direction, collision.GetContact(0).normal);
        speed *= 0.9f;
    }
}
