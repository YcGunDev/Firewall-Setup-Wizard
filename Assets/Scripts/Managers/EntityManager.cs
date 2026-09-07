using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance;

    private void Awake()
    {
        instance = this;
    }

    //public List<Block> Blocks = new List<Block>();
    public List<NetworkHealthComponent> Entities = new List<NetworkHealthComponent>();
    public List<Spacer> Spacers = new List<Spacer>();

    //public bool RemoveBlock(Block targetBlock)
    //{
    //    if (Blocks.Count > 0)
    //        return Blocks.Remove(targetBlock);
    //
    //    return false;
    //}

    //public void AddBlock(Block targetBlock)
    //{
    //    Blocks.Add(targetBlock);
    //}

    public bool RemoveEntity(NetworkHealthComponent targetEntity)
    {
        if (Entities.Count > 0)
            return Entities.Remove(targetEntity);
    
        return false;
    }

    public void AddEntity(NetworkHealthComponent targetEntity)
    {
        Entities.Add(targetEntity);
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

    //public Block FindBlock(uint id, List<Block> blackList = null)
    //{
    //    Block b = null;
    //    if (blackList != null)
    //        b = Blocks.Find(block => block.id.Value == id && !isBlockInList(block, blackList));
    //
    //    else
    //        b = Blocks.Find(block => block.id.Value == id);
    //
    //    if (b != null)
    //    {
    //        Debug.Log("Block found, id: " + id);
    //    }
    //    else
    //    {
    //        Debug.Log("Block not found, id: " + id);
    //    }
    //
    //    return b;
    //}

    uint GetID(NetworkHealthComponent c)
    {
        Block b = c.GetComponent<Block>();
        if (b != null)
        {
            return b.id.Value;
        }
        BaseWall bw = c.GetComponent<BaseWall>();
        if (bw != null)
        {
            return bw.id.Value;
        }

        return 0;
    }
    
    Block GetBlock(NetworkHealthComponent c)
    {
        return c.GetComponent<Block>();
    }

    public Block FindBlock(uint id, List<Block> blackList = null)
    {
        NetworkHealthComponent e = null;

        if (blackList != null)
            e = Entities.Find(entity => GetID(entity) == id && !isBlockInList(GetBlock(entity), blackList));

        else
            e = Entities.Find(entity => GetID(entity) == id);
    
        
        if (e != null)
        {
            Block b = e.GetComponent<Block>();
            Debug.Log("Block found, id: " + id);
            return b;
        }
        else
        {
            Debug.Log("Block not found, id: " + id);
        }
        return null;
    }

    public NetworkHealthComponent FindEntity(uint id, List<NetworkHealthComponent> blackList = null)
    {
        NetworkHealthComponent e = null;
        if (blackList != null)
            e = Entities.Find(entity => GetID(entity) == id && !isEntityInList(entity, blackList));

        else
            e = Entities.Find(entity => GetID(entity) == id);

        if (e != null)
        {
            Debug.Log("Entity found, id: " + id);
        }
        else
        {
            Debug.Log("Entity not found, id: " + id);
        }

        return e;
    }

    bool isBlockInList(Block b, List<Block> bl)
    {
        if (b == null) return false;
        foreach (Block block in bl)
        {
            if (b == block)
                return true;
        }
        return false;
    }

    bool isEntityInList(NetworkHealthComponent e, List<NetworkHealthComponent> bl)
    {
        if (e == null) return false;
        foreach (NetworkHealthComponent entity in bl)
        {
            if (e == entity)
                return true;
        }
        return false;
    }
}
