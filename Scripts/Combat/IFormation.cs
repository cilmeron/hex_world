using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IFormation {
    void SetLeader(IFormationElement fe);
    IFormationElement GetLeader();
    void SetFormationElements(List<IFormationElement> fes);
    bool AddFormationElement(IFormationElement fe);
    bool AddFormationElementAt(IFormationElement fe,Vector3 relativePosition);
    void RemoveFormationElement(IFormationElement fe);
    List<IFormationElement> GetFormationElements();
    void SetFormation();
    Formation GetFormation();
    void SetSpacing(int spacing);
    void SetTargetPosition(Vector3 pos);

    void CalculateCapacity();

    void CalculateRelativePositions();
}
