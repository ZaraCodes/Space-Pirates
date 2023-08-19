using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the progression parameters for the space station scene
/// </summary>
public class SpaceStationProgressionSetter : MonoBehaviour
{
    #region Fields
    /// <summary>Reference to ship interactor</summary>
    [SerializeField] private GameObject shipInteractor;
    /// <summary>Reference to the outro interactor</summary>
    [SerializeField] private GameObject outroInteractor;
    /// <summary>Reference to the shopkeeper interactor</summary>
    [SerializeField] private GameObject shopkeeperInteractor;
    /// <summary>Reference to the switch that controls the door to the upper area</summary>
    [SerializeField] private Switch SecretSwitch0;
    /// <summary>Reference to the switch that controls the door to the lower area</summary>
    [SerializeField] private Switch SecretSwitch1;
    /// <summary>Reference to the object for the first repair kit</summary>
    [SerializeField] private GameObject RepairKit0;
    /// <summary>Reference to the object for the second repair kit</summary>
    [SerializeField] private GameObject RepairKit1;
    /// <summary>Reference to the landing texbox trigger</summary>
    [SerializeField] private TextboxTrigger landingDialog;
    #endregion

    #region Methods
    /// <summary>Sets the parameters for a repair kit</summary>
    /// <param name="repairKit">The repair kit in question</param>
    private void SetRepairKit(GameObject repairKit)
    {
        repairKit.transform.GetChild(0).gameObject.SetActive(false);
        repairKit.transform.GetChild(1).gameObject.SetActive(true);
        repairKit.GetComponent<BoxCollider2D>().enabled = false;
    }

    /// <summary>Coroutine that starts the landing dialog</summary>
    /// <returns></returns>
    private IEnumerator StartLandingDialog()
    {
        yield return new WaitForSeconds(2f);
        while (!GameManager.Instance.IsPlaying)
        {
            yield return null;
        }
        landingDialog.LoadDialog();
    }
    #endregion

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
        if (!ProgressionManager.Instance.Flags.Contains(EProgressionFlag.IntroFinished)) {
            StartCoroutine(StartLandingDialog());
        }
    }
}
