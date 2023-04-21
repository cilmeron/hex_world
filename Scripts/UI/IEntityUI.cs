using System;
using System.Collections.Generic;
using UnityEngine;

public interface IEntityUI{
    float GetHpPercentage();
    Sprite GetSprite();
    String GetStats();
}