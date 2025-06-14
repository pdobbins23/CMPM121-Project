using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spell
{
    private float lastCast = 0f;
    private RawSpell rawSpell;

    private SpellCaster owner;

    public Spell(RawSpell rawSpell, SpellCaster owner)
    {
        this.rawSpell = rawSpell;
        this.owner = owner;
    }

    public bool IsReady()
    {
        return Time.time > lastCast + owner.Evaluate(rawSpell.Cooldown);
    }

    public float LastCast() => lastCast;

    public RawSpell GetRaw() => rawSpell;
    public string GetName() => rawSpell.Name;
    public int GetIcon() => rawSpell.Icon;

    public float GetDelay()
    {
        return owner.Evaluate(rawSpell.Mod.Delay ?? "0");
    }

    public float GetAngle()
    {
        return owner.Evaluate(rawSpell.Mod.Angle ?? "0");
    }

    public int GetCount()
    {
        return owner.EvaluateInt(rawSpell.Count ?? "1");
    }

    public float GetSpray()
    {
        return owner.Evaluate(rawSpell.Spray ?? "0");
    }

    public float GetBaseLifetime()
    {
        return owner.Evaluate(rawSpell.Projectile?.Lifetime ?? "100");
    }

    public float GetSecondaryLifetime()
    {
        return owner.Evaluate(rawSpell.SecondaryProjectile?.Lifetime ?? "100");
    }

    public int GetManaCost()
    {
        float manaCost = owner.Evaluate(rawSpell.ManaCost);
        float manaAdder = owner.Evaluate(rawSpell.Mod.ManaAdder ?? "0");
        float manaMultiplier = owner.Evaluate(rawSpell.Mod.ManaMultiplier ?? "1");

        return Mathf.RoundToInt((manaCost + manaAdder) * manaMultiplier);
    }

    public float GetBaseDamage()
    {
        float damage = owner.Evaluate(rawSpell.Damage?.Amount ?? "0");
        float damageAdder = owner.Evaluate(rawSpell.Mod.DamageAdder ?? "0");
        float damageMultiplier= owner.Evaluate(rawSpell.Mod.DamageMultiplier ?? "1");

        return (damage + damageAdder) * damageMultiplier;
    }

    public float GetSecondaryDamage()
    {
        return owner.Evaluate(rawSpell.SecondaryDamage ?? "0");
    }

    public float GetCooldown()
    {
        float cooldown = owner.Evaluate(rawSpell.Cooldown ?? "0");
        float cooldownAdder = owner.Evaluate(rawSpell.Mod.CooldownAdder ?? "0");
        float cooldownMultiplier = owner.Evaluate(rawSpell.Mod.CooldownMultiplier ?? "1");

        return (cooldown + cooldownAdder) * cooldownMultiplier;
    }

    public float GetBaseProjectileSpeed()
    {
        float speed = owner.Evaluate(rawSpell.Projectile?.Speed ?? "0");
        float speedAdder = owner.Evaluate(rawSpell.Mod.SpeedAdder ?? "0");
        float speedMultiplier = owner.Evaluate(rawSpell.Mod.SpeedMultiplier ?? "1");

        return (speed + speedAdder) * speedMultiplier;
    }

    public float GetSecondaryProjectileSpeed()
    {
        float speed = owner.Evaluate(rawSpell.SecondaryProjectile?.Speed ?? "0");
        float speedAdder = owner.Evaluate(rawSpell.Mod.SpeedAdder ?? "0");
        float speedMultiplier = owner.Evaluate(rawSpell.Mod.SpeedMultiplier ?? "1");

        return (speed + speedAdder) * speedMultiplier;
    }

    private void FireProjectile(Vector3 where, Vector3 dir)
    {
        string baseTrajectory = rawSpell.Mod.ProjectileTrajectory ?? rawSpell.Projectile?.Trajectory;

        GameManager.Instance.projectileManager.CreateProjectile(
            rawSpell.Icon, baseTrajectory, where, dir, GetBaseProjectileSpeed(), OnHit, GetBaseLifetime());
    }

    private void FireBaseProjectiles(Vector3 where, Vector3 dir)
    {
        int count = GetCount();
        float spray = GetSpray();

        var rnd = new System.Random();

        for (int i = 0; i < count; i++)
        {
            Vector3 finalDir = dir;

            if (spray != 0.0)
            {
                Quaternion rot = Quaternion.Euler(0, 0, rnd.Next(-180, 180) * spray);
                finalDir = rot * finalDir;
            }

            if (rawSpell.Mod.SplitProjectile == true)
            {
                float angleOffset = GetAngle();
                Quaternion rotation1 = Quaternion.Euler(0, 0, angleOffset);
                Quaternion rotation2 = Quaternion.Euler(0, 0, -angleOffset);

                Vector3 dir1 = rotation1 * finalDir;
                Vector3 dir2 = rotation2 * finalDir;

                FireProjectile(where, dir1);
                FireProjectile(where, dir2);
            }
            else
                FireProjectile(where, finalDir);
        }

    }

    public void FireSecondaryProjectile(Vector3 where, Vector3 dir)
    {
        GameManager.Instance.projectileManager.CreateProjectile(
            rawSpell.Icon, rawSpell.SecondaryProjectile?.Trajectory, where, dir, GetSecondaryProjectileSpeed(), OnSecondaryHit, GetSecondaryLifetime());
    }

    public IEnumerator Cast(Vector3 where, Vector3 target, Hittable.Team team)
    {
        lastCast = Time.time;
        Vector3 dir = (target - where).normalized;

        FireBaseProjectiles(where, dir);

        if (rawSpell.Mod.DoubleProjectile == true)
        {
            yield return new WaitForSeconds(GetDelay());

            FireBaseProjectiles(where, dir);
        }

        yield return null;
    }

    public void OnHit(Hittable other, Vector3 impact)
    {
        if (other.team != owner.team)
        {
            other.Damage(new Damage(Mathf.RoundToInt(GetBaseDamage()), Damage.TypeFromString(rawSpell.Damage?.Type)));

            if (rawSpell.SecondaryProjectile != null)
            {
                var rnd = new System.Random();

                int count = rnd.Next(5, 10);
                Vector3 dir = (other.owner.transform.position - impact).normalized;

                for (int i = 0; i < count; i++)
                {
                    Quaternion rot = Quaternion.Euler(0, 0, rnd.Next(-180, 180));
                    Vector3 randomDir = rot * dir;

                    FireSecondaryProjectile(impact, randomDir);
                }
            }
        }
    }

    public void OnSecondaryHit(Hittable other, Vector3 impact)
    {
        if (other.team != owner.team)
        {
            other.Damage(new Damage(Mathf.RoundToInt(GetSecondaryDamage()), Damage.TypeFromString(rawSpell.Damage?.Type)));
        }
    }
}
