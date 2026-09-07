using UnityEngine;

public class Spacer : MonoBehaviour
{
    public uint id = 0;
    void Awake()
    {
        if (EntityManager.instance == null)
        {
            Invoke("AddSelf", 0.1f);
        }
        else
        {
            EntityManager.instance.AddSpacer(this);
        }
        
    }

    private void OnDestroy()
    {
        EntityManager.instance.RemoveSpacer(this);
    }

    private void AddSelf()
    {
        EntityManager.instance.AddSpacer(this);
    }
}
