using UnityEngine;
using System;

public class EventBus 
{
    private static EventBus theInstance;
    public static EventBus Instance
    {
        get
        {
            if (theInstance == null)
                theInstance = new EventBus();
            return theInstance;
        }
    }

    public event Action<Hittable> OnKill;
    public void DoKill()
{
    OnKill?.Invoke();
    AudioManager.Instance.PlaySound(AudioManager.Instance.enemyDeathSound);
}


    public event Action OnCast;
    public void DoCast()
{
    OnCast?.Invoke();
    AudioManager.Instance.PlaySound(AudioManager.Instance.spellCastSound);
}


    public event Action<float> OnMove; // provide distance moved
    public void DoMove(float distance) => OnMove?.Invoke(distance);

    public event Action<Vector3, Damage, Hittable> OnDamage;
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
        AudioManager.Instance.PlaySound(AudioManager.Instance.damageSound);
    }
}
