using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMoveable{
    public void EnableNavMesh(bool active);
    
    void SetMoveToPosition(Vector3 moveToPosition);
}
