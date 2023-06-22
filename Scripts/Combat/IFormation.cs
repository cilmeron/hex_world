using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IFormation {
    void SetLeader(C_Formation fe);
    C_Formation GetLeader();
    void SetFormationElements(List<C_Formation> fes);
    bool AddFormationElement(C_Formation fe);
    bool AddFormationElementAt(C_Formation fe,Vector3 relativePosition);
    void RemoveFormationElement(C_Formation fe);
    HashSet<C_Formation> GetFormationElements();
    void SetFormation();
    Formation GetFormation();
    void SetSpacing(int spacing);
    void SetTargetPosition(Vector3 pos);

    void CalculateCapacity();

    void CalculateRelativePositions();
}
