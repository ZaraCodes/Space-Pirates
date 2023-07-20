using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionSetter : MonoBehaviour
{
    [SerializeField] private EProgressionFlag flag;

    public void SetFlag()
    {
        if (!ProgressionManager.Instance.Flags.Contains(flag))
            ProgressionManager.Instance.Flags.Add(flag);
    }
}
