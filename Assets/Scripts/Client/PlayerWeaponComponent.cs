using UnityEngine;
using System.Collections;
using MLAPI;
using MLAPI.NetworkedVar;
using MLAPI.Spawning;

public class PlayerWeaponComponent : NetworkedBehaviour
{
    [SerializeField] PlayerInputComponent input;

    private NetworkedVar<float> ammoAmount = new NetworkedVar<float>(0);

    private void Update()
    {
        if (input.Fire) Fire();
    }

    private void Fire()
    {
        if(ammoAmount.Value > 0)
        {
            Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float angleRad = Mathf.Atan2(direction.y, direction.x);
            Vector3 rotation = new Vector3(0,0, angleRad * Mathf.Rad2Deg);

            GameSpawnManager.SpawnBullet(
                this,
                transform.position + (direction.normalized * 2), rotation);
        }
    }

    public void AddAmmo(int amount)
    {
        ammoAmount.Value += amount;
    }

    public bool HasAmmo()
    {
        return ammoAmount.Value > 0;
    }

    public void SpentAmmo()
    {
        ammoAmount.Value--;
    }

    public void ResetAmmo()
    {
        ammoAmount.Value = 0f;
    }
}
