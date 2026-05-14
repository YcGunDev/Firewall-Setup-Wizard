using TMPro;
using UnityEngine;

public class Block : MonoBehaviour, ITakeDamage
{
    public int health;
    public float speed;
    public Vector2 direction; //this will always be normalized

    public CollisionHandler sharedHandler = null;

    //objects
    [SerializeField] Rigidbody2D rb;
    [SerializeField] TextMeshProUGUI healthUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (health <= 0) health = 100;
        //speed = 0.0f;
        //direction = Vector2.zero;

        healthUI.text = health.ToString();

        BlockManager.instance.AddBlock(this);
    }

    private void OnDestroy()
    {
        BlockManager.instance.RemoveBlock(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (direction != Vector2.zero)
            rb.linearVelocity = direction * speed;
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
        if (collision.gameObject.layer == gameObject.layer) return;
        if (sharedHandler != null) return;
        ITakeDamage itd = collision.gameObject.GetComponent<ITakeDamage>();
        if (itd != null)
        {
            Block otherBlock = collision.gameObject.GetComponent<Block>();
            if (otherBlock != null)
            {
                sharedHandler = Instantiate(CollisionManager.instance.collisionHandler).GetComponent<CollisionHandler>();
                otherBlock.sharedHandler = sharedHandler;
                sharedHandler.A = this;
                sharedHandler.B = otherBlock;
                sharedHandler.ProcessCollision();
            }  
        }
    }
}
