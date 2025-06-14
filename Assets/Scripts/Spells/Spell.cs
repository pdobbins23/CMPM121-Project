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
        return Time.time > lastCast + RPN.eval(rawSpell.Cooldown, owner.GetContext().ToDictionary());
    }

    public float LastCast() => lastCast;

    public string GetName() => rawSpell.Name;
    public int GetIcon() => rawSpell.Icon;

    public float GetDelay()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.eval(rawSpell.Delay ?? "0", ctx);
    }

    public float GetAngle()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.eval(rawSpell.Angle ?? "0", ctx);
    }

    public int GetCount()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.EvalInt(rawSpell.Count ?? "1", ctx);
    }

    public float GetSpray()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.eval(rawSpell.Spray ?? "0", ctx);
    }

    public float GetBaseLifetime()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.eval(rawSpell.Projectile?.Lifetime ?? "100", ctx);
    }

    public float GetSecondaryLifetime()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.eval(rawSpell.SecondaryProjectile?.Lifetime ?? "100", ctx);
    }

    public int GetManaCost()
    {
        var ctx = owner.GetContext().ToDictionary();

        float manaCost = RPN.eval(rawSpell.ManaCost, ctx);
        float manaAdder = RPN.eval(rawSpell.ManaAdder ?? "0", ctx);
        float manaMultiplier = RPN.eval(rawSpell.ManaMultiplier ?? "1", ctx);

        return Mathf.RoundToInt((manaCost + manaAdder) * manaMultiplier);
    }

    public float GetBaseDamage()
    {
        var ctx = owner.GetContext().ToDictionary();

        float damage = RPN.eval(rawSpell.Damage?.Amount ?? "0", ctx);
        float damageAdder = RPN.eval(rawSpell.DamageAdder ?? "0", ctx);
        float damageMultiplier= RPN.eval(rawSpell.DamageMultiplier ?? "1", ctx);

        return (damage + damageAdder) * damageMultiplier;
    }

    public float GetSecondaryDamage()
    {
        var ctx = owner.GetContext().ToDictionary();

        return RPN.eval(rawSpell.SecondaryDamage ?? "0", ctx);
    }

    public float GetCooldown()
    {
        var ctx = owner.GetContext().ToDictionary();

        float cooldown = RPN.eval(rawSpell.Cooldown ?? "0", ctx);
        float cooldownAdder = RPN.eval(rawSpell.CooldownAdder ?? "0", ctx);
        float cooldownMultiplier = RPN.eval(rawSpell.CooldownMultiplier ?? "1", ctx);

        return (cooldown + cooldownAdder) * cooldownMultiplier;
    }

    public float GetBaseProjectileSpeed()
    {
        var ctx = owner.GetContext().ToDictionary();

        float speed = RPN.eval(rawSpell.Projectile?.Speed ?? "0", ctx);
        float speedAdder = RPN.eval(rawSpell.SpeedAdder ?? "0", ctx);
        float speedMultiplier = RPN.eval(rawSpell.SpeedMultiplier ?? "1", ctx);

        return (speed + speedAdder) * speedMultiplier;
    }

    public float GetSecondaryProjectileSpeed()
    {
        var ctx = owner.GetContext().ToDictionary();

        float speed = RPN.eval(rawSpell.SecondaryProjectile?.Speed ?? "0", ctx);
        float speedAdder = RPN.eval(rawSpell.SpeedAdder ?? "0", ctx);
        float speedMultiplier = RPN.eval(rawSpell.SpeedMultiplier ?? "1", ctx);

        return (speed + speedAdder) * speedMultiplier;
    }

    private void FireProjectile(Vector3 where, Vector3 dir)
    {
        string baseTrajectory = rawSpell.ProjectileTrajectory ?? rawSpell.Projectile?.Trajectory;

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

            if (rawSpell.SplitProjectile == true)
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
        var ctx = owner.GetContext().ToDictionary();

        Vector3 dir = (target - where).normalized;

        FireBaseProjectiles(where, dir);

        if (rawSpell.DoubleProjectile == true)
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
