using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DecisionTree
{
    public C_Health ChooseTarget(List<C_Health> targets, Entity selfEntity)
    {
        // Filter out targets belonging to the same player as selfEntity
        List<C_Health> enemyTargets = targets
            .Where(target => target.Entity.GetPlayer() != selfEntity.GetPlayer() && selfEntity.GetPlayer() != null)
            .ToList();

        // Implement your decision logic on the filtered enemyTargets list
        // For example, you can sort the filtered list based on health and choose the first one.
        enemyTargets.Sort((a, b) => a.CurrentHP.CompareTo(b.CurrentHP));
        //foreach (C_Health target in enemyTargets)
        //{
        //    //Debug.Log(target.CurrentHP);
        //}


        return enemyTargets.Count > 0 ? enemyTargets[0] : null;
    }
}
