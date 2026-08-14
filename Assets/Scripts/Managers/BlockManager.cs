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
    public GameObject spawnArea1;
    public GameObject spawnArea2;

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

    public void ReplaceSpacer(int id)
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

    public Block FindBlock(int id)
    {
        Block b = Blocks.Find(block => block.id.Value == id);
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
}
