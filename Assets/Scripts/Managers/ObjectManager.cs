using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance;

    //for all intents and purposes, p1 is left, p2 is right
    public GameObject baseP1;
    public GameObject baseP2;

    public GameObject spawnAreaP1;
    public GameObject spawnAreaP2;

    



    private void Awake()
    {
        instance = this;
    }

    

}
