using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;

public class BaseWall : MonoBehaviour, ITakeDamage
{
    public int health = 500;

    [SerializeField] TextMeshProUGUI healthUI;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] float shakeMagnitude = 2f;
    [SerializeField] float effectSpeed = 0.1f;
    private Color baseColour;
    private Vector3 basePos;
    private bool isEffect = false;
    private float shakeStrength = 1.0f;

    private void Awake()
    {
        UpdateHPUI();
        baseColour = sprite.color;
        basePos = sprite.transform.localPosition;
    }

    public void UpdateHPUI()
    {
        healthUI.text = health.ToString();
    }

    public void TakeDamage(int damage)
    {
        health = Mathf.Clamp(health - Mathf.Abs(damage), 0, 9999);

        UpdateHPUI();
        
        if (health <= 0) OnDeath();
        else TryDamageEffect(damage);
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

    void TryDamageEffect(int damage)
    {
        if (isEffect)
        {
            //dont start a new coroutine
            sprite.color = Color.white;
            shakeStrength = 1.0f;
        }
        else
        {
            isEffect = true;
            StartCoroutine(DamageBaseColourEffect());
            StartCoroutine(DamageShakeEffect(damage));
            //I tried add colour flashing to the text aswell but its not percievable ngl so ill leave it out
        }
    }

    IEnumerator DamageBaseColourEffect()
    {
        sprite.color = Color.white;
        while (sprite.color != baseColour)
        {
            sprite.color = Color.Lerp(sprite.color, baseColour, effectSpeed);
            yield return null;
        }

        isEffect = false;
        yield return null;
    }

    IEnumerator DamageShakeEffect(int damage)
    {
        shakeStrength = 1.0f;
        while (isEffect)
        {
            sprite.transform.localPosition = basePos + (Vector3)Random.insideUnitCircle.normalized * shakeMagnitude * shakeStrength * ((float)damage / 1000f);
            Mathf.Lerp(shakeStrength, 0.0f, effectSpeed);
            yield return null;
        }
        sprite.transform.localPosition = basePos;
        yield return null;
    }
}
