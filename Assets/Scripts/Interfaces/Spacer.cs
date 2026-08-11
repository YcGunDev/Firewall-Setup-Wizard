using UnityEngine;

public class Spacer : MonoBehaviour
{
    public int id = 0;
    void Awake()
    {
        if (BlockManager.instance == null)
        {
            Invoke("AddSelf", 0.1f);
        }
        else
        {
            BlockManager.instance.AddSpacer(this);
        }
        
    }

    private void OnDestroy()
    {
        BlockManager.instance.RemoveSpacer(this);
    }

    private void AddSelf()
    {
        BlockManager.instance.AddSpacer(this);
    }
}
