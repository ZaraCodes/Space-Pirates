using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class AllDialogs : MonoBehaviour
{
    #region Fields
    public static AllDialogs Instance;
    public TextboxSequence[] Sequences;
    #endregion

    #region Methods
    public TextboxSequence GetSequence(string sequenceName)
    {
        foreach (var sequence in Sequences)
        {
            if (sequence.ID == sequenceName) return sequence;
        }
        return null;
    }
    #endregion

    #region Unity Stuff
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (Instance == null) Instance = GetComponent<AllDialogs>();
    }
    #endregion
}
