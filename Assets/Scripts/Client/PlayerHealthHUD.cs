using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealthHUD : MonoBehaviour
{
    [SerializeField] Slider healthBar;

    public void UpdateBarValue(float value)
    {
        healthBar.value = value;
    }
}
