using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecisionTree
{
    private int distanceWeight = 1;
    private int healthWeight = 1;
    private int persistWeigth = 1;

    public C_Health ChooseTarget(List<C_Health> targets, Entity selfEntity)
    {
        List<C_Health> enemyTargets = targets
            .Where(target => target.Entity.GetPlayer() != selfEntity.GetPlayer() && selfEntity.GetPlayer() != null)
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

    public C_Health ChooseTarget(List<Entity> entities, Entity selfEntity, C_Health oldTarget)
    {
        // get enemy targets (ingore friendlies)
        // get distances between selfEntity and target entities
        // get health values
        // combine health and distance -> rank this value
        // return target with highest score

        List<Entity> enemyTargets = entities
         .Where(entity => entity.GetPlayer() != selfEntity.GetPlayer() && selfEntity.GetPlayer() != null && entity.CHealth != null)
         .ToList();

        // float distance = Vector3.Distance(entities[0].transform.localPosition, selfEntity.transform.localPosition);
        List<double> distances = new List<double>();
        foreach (Entity target in enemyTargets)
        {
            // get distances between self and enemys
        }

        // calc score with healths

        enemyTargets.Sort((a, b) => a.CHealth.CurrentHP.CompareTo(b.CHealth.CurrentHP));
        return enemyTargets.Count > 0 ? enemyTargets[0].CHealth : null;
    }
}