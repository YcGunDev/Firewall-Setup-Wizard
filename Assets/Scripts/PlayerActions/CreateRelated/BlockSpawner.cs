using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    float colliderTimer = 0.03f;
    float colliderDelay = 0.03f;
    //bool canSpawn = true;
    [SerializeField] Block block;

    Vector2 tr_s = Vector2.zero;
    Vector2 bl_s = Vector2.zero;

    private void Update()
    {
        //check positions instead of a timer
        bool isOverlap = false;

        tr_s = new Vector2(transform.position.x + transform.localScale.x * 0.525f, transform.position.y + transform.localScale.y * 0.525f);
        bl_s = new Vector2(transform.position.x - transform.localScale.x * 0.525f, transform.position.y - transform.localScale.y * 0.525f);

        foreach (Block block in BlockManager.instance.Blocks)
        {
            Vector2 tr_b = new Vector2(block.transform.position.x + block.transform.localScale.x * 0.525f, block.transform.position.y + block.transform.localScale.y * 0.525f);
            Vector2 bl_b = new Vector2(block.transform.position.x - block.transform.localScale.x * 0.525f, block.transform.position.y - block.transform.localScale.y * 0.525f);

            //should try to use a karnough map to try and optimize this

            // # METHOD 1
            //top right point is fully enclosed
            if (tr_b.x <= tr_s.x && tr_b.x >= bl_s.x &&
                tr_b.y <= tr_s.y && tr_b.y >= bl_s.y)
            {
                isOverlap = true;
                break;
            }
            //bottom right is fully enclosed
            else if (bl_b.x <= tr_s.x && bl_b.x >= bl_s.x &&
                bl_b.y <= tr_s.y && bl_b.y >= bl_s.y)
            {
                isOverlap = true;
                break;
            }
            else if (tr_b.x <= tr_s.x && tr_b.x >= bl_s.x &&
                bl_b.y <= tr_s.y && bl_b.y >= bl_s.y)
            {
                isOverlap = true;
                break;
            }
            else if (bl_b.x <= tr_s.x && bl_b.x >= bl_s.x &&
                tr_b.y <= tr_s.y && tr_b.y >= bl_s.y)
            {
                isOverlap = true;
                break;
            }

            // METHOD 2
            //if (tr_b.x <= tr_s.x && tr_b.x >= bl_s.x)
            //{
            //    if (tr_b.y <= tr_s.y && tr_b.y >= bl_s.y)
            //    {
            //        isOverlap = true;
            //        break;
            //    }
            //    else if (bl_b.y <= tr_s.y && bl_b.y >= bl_s.y)
            //    {
            //        isOverlap = true;
            //        break;
            //    }
            //}
            //else if (bl_b.x <= tr_s.x && bl_b.x >= bl_s.x)
            //{
            //    if (bl_b.y <= tr_s.y && bl_b.y >= bl_s.y)
            //    {
            //        isOverlap = true;
            //        break;
            //    }
            //    else if (tr_b.y <= tr_s.y && tr_b.y >= bl_s.y)
            //    {
            //        isOverlap = true;
            //        break;
            //    }
            //}
        }

        if (!isOverlap)
        {
            if (colliderTimer <= 0)
            {
                Instantiate(block, transform.position, transform.rotation);
                colliderTimer = colliderDelay;
            }
        }
        if (colliderTimer > 0) colliderTimer = Mathf.Clamp(colliderTimer - Time.deltaTime, 0.0f, 5.0f);

        transform.position = transform.position + Random.insideUnitSphere * 0.001f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //colliderTimer = colliderDelay;
    }


}
