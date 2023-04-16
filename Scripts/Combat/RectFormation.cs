using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectFormation : Formation{
    private int rows = 3;
    private int columns = 4;
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
    }

    
    public override void CalculateCapacity(){
        maxCapacity = columns * rows;
    }

    public override void CalculateRelativePositions(){
        CalculateCapacity();

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (row == 0 && column == 0){
                    continue; // leader Position
                }
                Vector3 relativePosition = new Vector3(
                    row * spacingBetweenUnits,
                    0,
                    column * spacingBetweenUnits
                );
                relativePositions[relativePosition] = null;
            }
        }
    }
}
