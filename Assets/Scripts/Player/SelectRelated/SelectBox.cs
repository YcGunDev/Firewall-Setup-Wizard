using System.Collections.Generic;
using UnityEngine;

public class SelectBox : MonoBehaviour
{
    public List<Block> blockList;
    public Select selectParent;
    public int playerLayer;

    private void Awake()
    {
        playerLayer = ToolManager.instance.player.gameObject.layer;
    }

    public void SendBlockList()
    {
        selectParent.selectedBlocks = new List<Block>(blockList);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Block block = collision.gameObject.GetComponent<Block>();
        if (block != null)
        {
            if (collision.gameObject.layer == playerLayer)
            {
                if (!blockList.Contains(block))
                {
                    blockList.Add(block);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Block block = collision.gameObject.GetComponent<Block>();
        if (block != null)
        {
            if (collision.gameObject.layer == playerLayer)
            {

                if (blockList.Contains(block))
                {
                    blockList.Remove(block);
                }
            }
        }
    }
}
