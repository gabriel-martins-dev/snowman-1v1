using UnityEngine;
using System.Collections;
using MLAPI;
using MLAPI.NetworkedVar;

public class PlayerHealthComponent : NetworkedBehaviour
{
    [SerializeField] PlayerHealthHUD hud;
    public static float MaxHealth = 5f;
    public NetworkedVar<float> Health = new NetworkedVar<float>(MaxHealth);
}
