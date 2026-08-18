using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Block : NetworkBehaviour, ITakeDamage
{
    //I'm personally not sold on this, but atm its the easiest method and its working, so go get em tiger
    [Header("Attributes")]
    public NetworkVariable<int> id;
    public NetworkVariable<int> health = new NetworkVariable<int>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner);
    public float speed;
    public Vector2 direction; //this will always be normalized

    public NetworkVariable<int> blockLayer;

    [SerializeField] private float drag = 0.5f;

    //there is a chance i may want to replicate all these values, and not rely on unity's network transform, the motion is smooth but not perfect


    [Header("Components")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] private TextMeshProUGUI healthUI;
    [SerializeField] private SpriteRenderer sprite;
    public CollisionHandler sharedHandler = null;

    [Header("Effects")]
    [SerializeField] private GameObject spawnParticles;
    [SerializeField] float shakeMagnitude = 1f;
    [SerializeField] float effectSpeed = 0.1f;
    private Color baseColour;
    private Vector3 basePos;
    private bool isEffect = false;
    private float shakeStrength = 1.0f;
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

        baseColour = sprite.color;
        basePos = sprite.transform.localPosition;
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
        if (health.Value <= 0) Destroy(this.gameObject);
        else TryDamageEffect(damage);
    }

    private void OnHPChange(int previousValue, int newValue)
    {
        if (health.Value <= 0) Destroy(this.gameObject);
        else
        {
            TryDamageEffect(previousValue - newValue);
            UpdateHPUI();
        }
    }

    public void UpdateHPUI()
    {
        healthUI.text = health.Value.ToString();
    }

    public void SpawnParticles()
    {
        spawnParticles.SetActive(true);
    }

    void TryDamageEffect(int damage)
    {
        if (isEffect)
        {
            //dont start a new coroutine
            sprite.color = Color.white;
            shakeStrength = 1.0f;
        }
        else
        {
            isEffect = true;
            StartCoroutine(DamageBaseColourEffect());
            StartCoroutine(DamageShakeEffect(damage));
            //I tried add colour flashing to the text aswell but its not percievable ngl so ill leave it out
        }
    }

    IEnumerator DamageBaseColourEffect()
    {
        sprite.color = Color.white;
        while (sprite.color != baseColour)
        {
            sprite.color = Color.Lerp(sprite.color, baseColour, effectSpeed);
            yield return null;
        }

        isEffect = false;
        yield return null;
    }

    IEnumerator DamageShakeEffect(int damage)
    {
        shakeStrength = 1.0f;
        while (isEffect)
        {
            sprite.transform.localPosition = basePos + (Vector3)Random.insideUnitCircle.normalized * shakeMagnitude * shakeStrength * ((float)damage / 1000f);
            Mathf.Lerp(shakeStrength, 0.0f, effectSpeed);
            yield return null;
        }
        sprite.transform.localPosition = basePos;
        yield return null;
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
