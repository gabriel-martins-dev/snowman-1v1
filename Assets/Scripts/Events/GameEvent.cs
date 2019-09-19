using UnityEngine;

public class GameEvent
{

}

public class WaitingForPlayersEvent : GameEvent
{

}

public class RoundStartingEvent : GameEvent
{
    public int Time { get; private set; }


    public RoundStartingEvent(int t)
    {
        Time = t;
    }
}

public class RoundStartedEvent : GameEvent
{

}

public class TriggerEvent : GameEvent
{
    public Collider2D Collider { get; private set; }

    public TriggerEvent(Collider2D coll)
    {
        Collider = coll;
    }
}

public class BulletTriggerEvent : TriggerEvent
{
    public BulletComponent Bullet { get; private set; }

    public BulletTriggerEvent(Collider2D coll, BulletComponent bullet) : base(coll)
    {
        Bullet = bullet;
    }
}

public class AmmoTriggerEvent : TriggerEvent
{
    public AmmoPickup Ammo { get; private set; }

    public AmmoTriggerEvent(Collider2D coll, AmmoPickup ammo) : base(coll)
    {
        Ammo = ammo;
    }
}

public class DeathEvent : GameEvent
{
    public ulong ClientId { get; private set; }

    public DeathEvent(ulong clientId)
    {
        ClientId = clientId;
    }
}