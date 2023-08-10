using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceStationProgressionSetter : MonoBehaviour
{
    [SerializeField] private GameObject shipInteractor;
    [SerializeField] private GameObject outroInteractor;
    [SerializeField] private GameObject shopkeeperInteractor;
    [SerializeField] private Switch SecretSwitch0;
    [SerializeField] private Switch SecretSwitch1;
    [SerializeField] private GameObject RepairKit0;
    [SerializeField] private GameObject RepairKit1;

    private void SetRepairKit(GameObject repairKit)
    {
        repairKit.transform.GetChild(0).gameObject.SetActive(false);
        repairKit.transform.GetChild(1).gameObject.SetActive(true);
        repairKit.GetComponent<BoxCollider2D>().enabled = false;
    }

    // Start is called before the first frame update    
    void Start()
    {
        if (ProgressionManager.Instance.Flags.Contains(EProgressionFlag.ShopkeeperCutsceneFinished))
        {
            shipInteractor.SetActive(false);
            outroInteractor.SetActive(true);
            shopkeeperInteractor.SetActive(false);
        }
        if (ProgressionManager.Instance.ViewedDialogs.Contains("Shopkeeper 0"))
        {
            SecretSwitch0.Toggle();
        }
        if (ProgressionManager.Instance.ViewedDialogs.Contains("Shopkeeper 1"))
        {
            SecretSwitch1.Toggle();
        }
        if (ProgressionManager.Instance.Flags.Contains(EProgressionFlag.RepairKit1))
        {
            SetRepairKit(RepairKit0);
        }
        if (ProgressionManager.Instance.Flags.Contains(EProgressionFlag.RepairKit2))
        {
            SetRepairKit(RepairKit1);
        }
    }
}
