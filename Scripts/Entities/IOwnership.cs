using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IOwnership : ISelectable {
    void SetPlayer(Player p);
    Player GetPlayer();
    void SetMaterialsAndShaders();

    bool IsOfType(Type type) {
        return type.IsInstanceOfType(this);
    }
}
