using System.Collections.Generic;
using System.Linq;
using static UnityEngine.GraphicsBuffer;

public class DecisionTree
{
    public C_Health ChooseTarget(List<C_Health> targets, Entity selfEntity)
    {
        List<C_Health> enemyTargets = targets
            .Where(target => target.Entity.GetPlayer() != selfEntity.GetPlayer() && selfEntity.GetPlayer() != null)
            .ToList();

        enemyTargets.Sort((a, b) => a.CurrentHP.CompareTo(b.CurrentHP));

        return enemyTargets.Count > 0 ? enemyTargets[0] : null;
    }

    public C_Health ChooseTarget(List<Entity> entities, Entity selfEntity)
    {
        List<Entity> enemyTargets = entities
         .Where(entity => entity.GetPlayer() != selfEntity.GetPlayer() && selfEntity.GetPlayer() != null && entity.CHealth != null)
         .ToList();

        enemyTargets.Sort((a, b) => a.CHealth.CurrentHP.CompareTo(b.CHealth.CurrentHP));
        return enemyTargets.Count > 0 ? enemyTargets[0].CHealth : null;
    }
}