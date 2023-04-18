using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleFormation : Formation
{
    private int numUnits = 8;
    private float radius = 5f;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void CalculateCapacity()
    {
        maxCapacity = numUnits;
    }

    public override void CalculateRelativePositions()
    {
        CalculateCapacity();

        float angleBetweenUnits = 360f / (numUnits - 1);

        for (int i = 0; i < numUnits; i++)
        {
            float angle = i == 0 ? 0 : angleBetweenUnits * (i - 1);
            Vector3 relativePosition = new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * angle),
                0,
                Mathf.Sin(Mathf.Deg2Rad * angle)
            ) * radius;

            relativePositions[relativePosition] = null;
        }
    }


}