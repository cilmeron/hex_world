using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface DetectorNotification{
    public void DetectorNotification(Component detectedComponent, Detector.DetectionManagement direction);
}
