using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStationProgressionSetter : MonoBehaviour
{
    [SerializeField] private GameObject shipInteractor;
    [SerializeField] private GameObject outroInteractor;
    [SerializeField] private Switch SecretSwitch0;
    [SerializeField] private Switch SecretSwitch1;
    // Start is called before the first frame update
    
    void Start()
    {
        if (ProgressionManager.Instance.Flags.Contains(EProgressionFlag.ShopkeeperCutsceneFinished))
        {
            shipInteractor.SetActive(false);
            outroInteractor.SetActive(true);
        }
        if (ProgressionManager.Instance.ViewedDialogs.Contains("Shopkeeper 0"))
        {
            SecretSwitch0.Toggle();
        }
        if (ProgressionManager.Instance.ViewedDialogs.Contains("Shopkeeper 1"))
        {
            SecretSwitch1.Toggle();
        }
    }
}
