using System.Collections.Generic;
using UnityEngine;

public interface IFormation {
    void SetLeader(Unit u);
    Unit GetLeader();
    void SetUnitsInFormation(List<Unit> units);
    bool AddUnitToFormation(Unit u);
    void RemoveUnitFromFormation(Unit u);
    List<Unit> GetUnitsInFormation();
    void SetFormation();
    void SetSpacing(int spacing);
    void SetTargetPosition(Vector3 pos);

    void CalculateCapacity();

    void CalculateRelativePositions();
}
