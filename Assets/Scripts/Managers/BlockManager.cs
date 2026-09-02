using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<Block> Blocks = new List<Block>();
    public List<Spacer> Spacers = new List<Spacer>();

    public bool RemoveBlock(Block targetBlock)
    {
        if (Blocks.Count > 0)
            return Blocks.Remove(targetBlock);

        return false;
    }

    public void AddBlock(Block targetBlock)
    {
        Blocks.Add(targetBlock);
    }

    public bool RemoveSpacer(Spacer targetSpacer)
    {
        if (Spacers.Count > 0)
            return Spacers.Remove(targetSpacer);

        return false;
    }

    public void AddSpacer(Spacer targetSpacer)
    {
        Spacers.Add(targetSpacer);
    }

    public void ReplaceSpacer(uint id)
    {
        Spacer space = Spacers.Find(spacer => spacer.id == id);
        if (space != null)
        {
            Debug.Log("Spacer found, id: " + id);
            Destroy(space.gameObject);
        }
        else
        {
            Debug.Log("Spacer not found, id: " + id);
        }
        
    }

    public Block FindBlock(uint id, List<Block> blackList = null)
    {
        Block b = null;
        if (blackList != null)
            b = Blocks.Find(block => block.id.Value == id && isBlockInList(block, blackList));

        else
            b = Blocks.Find(block => block.id.Value == id);

        if (b != null)
        {
            Debug.Log("Block found, id: " + id);
        }
        else
        {
            Debug.Log("Block not found, id: " + id);
        }

        return b;
    }

    bool isBlockInList(Block b, List<Block> bl)
    {
        foreach (Block block in bl)
        {
            if (b == block)
                return true;
        }
        return false;
    }
}
