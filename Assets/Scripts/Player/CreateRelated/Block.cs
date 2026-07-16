using TMPro;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class Block : MonoBehaviour, ITakeDamage
{
    public int health;
    public float speed;
    public float drag = 0.01f;
    public Vector2 direction; //this will always be normalized

    public CollisionHandler sharedHandler = null;

    //objects
    [SerializeField] Rigidbody2D rb;
    [SerializeField] TextMeshProUGUI healthUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    //public override void OnNetworkSpawn()
    //{
    //    if (health <= 0) health = 100;
    //    //speed = 0.0f;
    //    //direction = Vector2.zero;

    //    UpdateHPUI();

    //    BlockManager.instance.AddBlock(this);
    //}

    //public override void OnNetworkDespawn()
    //{
    //    BlockManager.instance.RemoveBlock(this);
    //}

    void Awake()
    {
        if (health <= 0) health = 100;
        //speed = 0.0f;
        //direction = Vector2.zero;

        UpdateHPUI();

        BlockManager.instance.AddBlock(this);
    }

    //public override void OnDestroy()
    //{
    //    base.OnDestroy();
    //    BlockManager.instance.RemoveBlock(this);
    //}

    public void OnDestroy()
    {
        BlockManager.instance.RemoveBlock(this);
    }

    void Update()
    {
        if (direction != Vector2.zero)
            rb.linearVelocity = direction * speed;

        speed *= 1 - (drag * Time.deltaTime);
        if (speed <= 0.05f) speed = 0;
    }

    public void TakeDamage(int damage)
    {
        health -= Mathf.Abs(damage);
        UpdateHPUI();
        if (health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void UpdateHPUI()
    {
        healthUI.text = health.ToString();
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
                itd.TakeDamage(health);
                TakeDamage(health);
            }
            return;
        }

        //its probably an obstacle of some kind, bounce
        direction = Vector2.Reflect(direction, collision.GetContact(0).normal);
        speed *= 0.9f;
    }
}
