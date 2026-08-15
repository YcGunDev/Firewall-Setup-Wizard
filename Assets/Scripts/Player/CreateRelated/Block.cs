using TMPro;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

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

    [SerializeField] private float drag = 0.5f;

    //there is a chance i may want to replicate all these values, and not rely on unity's network transform, the motion is smooth but not perfect

    public CollisionHandler sharedHandler = null;

    //objects
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private TextMeshProUGUI healthUI;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject spawnParticles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.OnValueChanged += OnHPChange; //health may need to be changed from a network variable //or ig not
        gameObject.layer = blockLayer.Value;

        if (gameObject.layer == 6)
            sprite.color = new Color(1.0f, 0.67f, 0.67f, 1.0f);

        else if (gameObject.layer == 7)
            sprite.color = new Color(0.67f, 0.67f, 1.0f, 1.0f);

        else sprite.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
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
        if (!rb)
            rb = GetComponentInChildren<Rigidbody2D>();

        if (!healthUI)
            healthUI = GetComponentInChildren<TextMeshProUGUI>();
        UpdateHPUI();

        if (!sprite)
            sprite = GetComponentInChildren<SpriteRenderer>();


        BlockManager.instance.AddBlock(this);

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

    public void SpawnParticles()
    {
        spawnParticles.SetActive(true);
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
            //its a wall, like a player's base wall
            else
            {
                //this will need to be changed to use the RPC, may need to rework the block manager to include these walls
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
