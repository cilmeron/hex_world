using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class controller : MonoBehaviour
{
    private Vector3 start;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            start = Hex.Utils.GetMousePosition();
        }
        if (Input.GetMouseButtonUp(0))
        {
            Debug.Log(Hex.Utils.GetMousePosition() + " " + start);
            Collider[] colliderArray = Physics.OverlapBox(Hex.Utils.Center(start, Hex.Utils.GetMousePosition()), Hex.Utils.GetMousePosition()-start);
            foreach(Collider collider in colliderArray)
            {
                Debug.Log(collider);
            }
        }

    }
}
