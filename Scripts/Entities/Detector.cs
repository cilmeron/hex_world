using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private Entity owner;
    public GameObject visualisation;
    private SphereCollider col;

    [SerializeField] private string _detactables;

    public List<Component> detectedObjects;

    private void OnTriggerEnter(Collider other)
    {
        Type type = Type.GetType(_detactables);
        Component triggeredComponent = other.gameObject.GetComponent(type);
        if (triggeredComponent != null)
        {
            if (triggeredComponent is Detectable)
            {
                try
                {
                    detectedObjects.Add(triggeredComponent);
                    EventManager.Instance.componentDetected.Invoke(this, triggeredComponent, DetectionManagement.Enter);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Debug.Log("Error occurs here");
                    throw;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Type type = Type.GetType(_detactables);
        Component triggeredComponent = other.gameObject.GetComponent(type);
        if (triggeredComponent != null)
        {
            if (triggeredComponent is Detectable)
            {
                detectedObjects.Remove(triggeredComponent);
                EventManager.Instance.componentDetected.Invoke(this, triggeredComponent, DetectionManagement.Exit);
            }
        }
    }

    private List<Component> GetDetectedObjects()
    {
        Type type = Type.GetType(_detactables);
        return detectedObjects
            .Where(component => type.IsInstanceOfType(component))
            .ToList();
    }

    public void SetOwner(Entity entity)
    {
        owner = entity;
    }

    public enum DetectionManagement
    {
        Enter,
        Exit
    }

    public void SetRadius(int r)
    {
        col = GetComponent<SphereCollider>();
        col.radius = r;
        var t = visualisation.transform;
        Vector3 ls = t.localScale;
        t.localScale = new Vector3(r, ls.y, r);
    }

    public void EnableVisualisation(bool b)
    {
        visualisation.SetActive(false);
    }
}
