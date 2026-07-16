using TMPro;
using UnityEngine;

public class BaseWall : MonoBehaviour, ITakeDamage
{
    public int health = 500;

    [SerializeField] TextMeshProUGUI healthUI;

    private void Awake()
    {
        UpdateHPUI();
    }

    public void UpdateHPUI()
    {
        healthUI.text = health.ToString();
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Clamp(health - Mathf.Abs(damage), 0, 9999);

        UpdateHPUI();

        if (health <= 0)
        {
            OnDeath();
        }
    }

    public virtual void OnDeath() 
    { 
        switch (gameObject.layer)
        {
            case 6: //player 1
                Debug.Log("Player 1 has Died, Player 2 Wins");
                break;
            case 7: //player 2
                Debug.Log("Player 2 has Died, Player 1 Wins");
                break;
            case 8: //NPC
                Debug.Log("NPC has Died, Player Wins");
                break;

        }
    }
}
