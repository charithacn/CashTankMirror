using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    [SerializeField] int target = 60;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = target;
    }
}