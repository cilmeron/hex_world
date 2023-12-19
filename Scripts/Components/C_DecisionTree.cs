using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DecisionTree
{
    private float distanceWeight = 1;
    private float healthWeight = 1;
    private float persistWeigth = 1;
    private float rangedUnitThreshold = 10;

    public C_Health ChooseTarget(List<C_Health> targets, Entity selfEntity)
    {
        List<C_Health> enemyTargets = targets
            .Where(target => target.Entity.GetNation() != selfEntity.GetNation() && selfEntity.GetNation() != null)
            .ToList();

        enemyTargets.Sort((a, b) => a.CurrentHP.CompareTo(b.CurrentHP));

        return enemyTargets.Count > 0 ? enemyTargets[0] : null;
    }
    /*
    public C_Health ChooseTarget(List<Entity> entities, Entity selfEntity)
    {
        List<Entity> enemyTargets = entities
         .Where(entity => entity.GetPlayer() != selfEntity.GetPlayer() && selfEntity.GetPlayer() != null && entity.CHealth != null)
         .ToList();

        enemyTargets.Sort((a, b) => a.CHealth.CurrentHP.CompareTo(b.CHealth.CurrentHP));
        return enemyTargets.Count > 0 ? enemyTargets[0].CHealth : null;
    }
    */

    public C_Health ChooseTarget(List<Entity> entities, Entity selfEntity, C_Attack attack)
    {
        // get enemy targets (ingore friendlies)
        // get distances between selfEntity and target entities
        // get health values
        // combine health, swap and distance -> rank this value
        // return target with highest score

        // select enemies
        float rangedWeight = 1.0f;
        List<Entity> enemyTargets = entities
         .Where(entity => entity.GetNation() != selfEntity.GetNation() && selfEntity.GetNation() != null && entity.CHealth != null)
         .ToList();

        if (enemyTargets.Count == 0) return null;
        // calc distances to enemies
        var distances = enemyTargets.Select(entity => Vector3.Distance(selfEntity.transform.localPosition, entity.transform.localPosition)).ToArray();
        float maxDistance = distances.Max();

        var normalizedDistances = distances.Select(distance => distance / maxDistance).ToArray(); // maximu distance will be 1

        int attackRange = attack.weapon.AttackRange;
        if (attackRange >= rangedUnitThreshold) // just an additional idea to weigh in the unit type. If it is a ranged unit -> weigh the distance less
        {
            rangedWeight = 0.5f;
        }

        int i = 0;
        int swap = 0;
        float relativeHP;
        float reversedDistance;
        float targetValue = 0f;
        List<float> costList = new List<float>();

        foreach (Entity target in enemyTargets)
        {
            // three metrics are calculated which are used to estimat the target value. They are scaled from 0 (worst) to 1 (best)
            swap = (target == attack.target || attack.target != null) ? 1 : 0;
            relativeHP = 1 - (target.CHealth.CurrentHP / target.CHealth.GetMaxHp());
            reversedDistance = 1 - normalizedDistances[i];

            targetValue = (swap * persistWeigth + relativeHP * healthWeight + reversedDistance * (distanceWeight * rangedWeight)) /
                (persistWeigth + healthWeight + (distanceWeight * rangedWeight));
            // ensure that the targetValue again ranges from 0 to 1. The weights determine the weight of the individual metrics
            costList.Add(targetValue);
            i++;
        }

        if (costList.Count > 0)
        {
            int bestTarget = costList.IndexOf(costList.Max());
            return enemyTargets[bestTarget].CHealth;
        }
        else
        {
            return null;
        }
    }
}