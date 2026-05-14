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
}
