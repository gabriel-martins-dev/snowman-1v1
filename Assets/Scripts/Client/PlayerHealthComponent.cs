using UnityEngine;
using MLAPI;
using MLAPI.NetworkedVar;

public class PlayerHealthComponent : NetworkedBehaviour
{
    private static float maxHealth = 5f;

    [SerializeField] PlayerHealthHUD hud;
    private NetworkedVar<float> Health = new NetworkedVar<float>(maxHealth);

    private void Awake()
    {
        Health.OnValueChanged += HealthValueChangeHandler;
    }

    private void HealthValueChangeHandler(float pvalue, float nvalue)
    {
        hud.UpdateBarValue(nvalue / maxHealth);
    }

    public void Damage(float damage)
    {
        Health.Value -= damage;

        if(Health.Value <= 0) EventManager.Trigger(new DeathEvent(OwnerClientId));
    }

    public void Heal()
    {
        Health.Value = maxHealth;
    }
}
