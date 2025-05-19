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

    public event Action<Vector3, Damage, Hittable> OnDamage;
    public event Action OnKill;
    public void DoKill() => OnKill?.Invoke();

    public event Action OnCast;
    public void DoCast() => OnCast?.Invoke();


    public event Action OnCast;

public event Action<float> OnMove; // provide distance moved
public void DoMove(float distance) => OnMove?.Invoke(distance);

public void DoMove() => OnMove?.Invoke();

    
    public void DoDamage(Vector3 where, Damage dmg, Hittable target)
    {
        OnDamage?.Invoke(where, dmg, target);
    }

}
