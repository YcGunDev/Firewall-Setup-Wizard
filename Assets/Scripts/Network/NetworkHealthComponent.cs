using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkHealthComponent : NetworkBehaviour, ITakeDamage
{
    //the point of this component is to centralize the damage system between objects, which should also be replicated accross the network
    //aka the damage functions from the blocks and walls will be put here instead
    //this is so that the network manager, which should get renamed to the network proxy parses this component instead of the blocks themselves

    //this problem is easily solvable by adding a second function but oh well

    public NetworkVariable<int> health = new NetworkVariable<int>(0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    [SerializeField] private TextMeshProUGUI healthUI;

    public UnityEvent<int> OnDamagetaken;

    void Awake()
    {
        if (health.Value <= 0) health.Value = 100;

        if (!healthUI)
            healthUI = GetComponentInChildren<TextMeshProUGUI>();
        UpdateHPUI();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.OnValueChanged += OnHPChange;
    }

    public void TakeDamage(int damage)
    {
        health.Value -= Mathf.Abs(damage);
        UpdateHPUI();
        if (health.Value <= 0) Destroy(this.gameObject);
        OnDamagetaken.Invoke(damage);
    }

    private void OnHPChange(int previousValue, int newValue)
    {
        if (health.Value <= 0) Destroy(this.gameObject);
        else
        {
            OnDamagetaken.Invoke(previousValue - newValue);
            UpdateHPUI();
            Debug.Log("huh");
        }
    }

    public void UpdateHPUI()
    {
        healthUI.text = health.Value.ToString();
    }

}
