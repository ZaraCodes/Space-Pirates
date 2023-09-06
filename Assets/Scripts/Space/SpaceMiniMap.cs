using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mini map that displays the positioning of the space objects and the space ship
/// </summary>
public class SpaceMiniMap : MonoBehaviour
{
    /// <summary>Number that scales down the position of objects on the mini map</summary>
    [SerializeField] private float divisionFactorForMap;
    [SerializeField] private float mapRadius;

    /// <summary>Refernce to the sun</summary>
    [Header("Space Objects"), SerializeField] private Transform sun;
    /// <summary>Reference to the gas giant</summary>
    [SerializeField] private Transform gasGiant;
    /// <summary>Reference to the island planet</summary>
    [SerializeField] private Transform islandPlanet;
    /// <summary>Reference to the city planet</summary>
    [SerializeField] private Transform cityPlanet;
    /// <summary>Reference to the space station</summary>
    [SerializeField] private Transform spaceStation;
    /// <summary>Reference to the hope ship</summary>
    [SerializeField] private Transform hopeShip;
    /// <summary>Reference to the moon ring that's used for landing</summary>
    [SerializeField] private GameObject moonRing;

    /// <summary>Reference to the sun on the mini map</summary>
    [Header("Mini Map Objects"), SerializeField] private RectTransform sunOnMap;
    /// <summary>Reference to the gas giant on the mini map</summary>
    [SerializeField] private RectTransform gasOnMap;
    /// <summary>Reference to the island planet on the mini map</summary>
    [SerializeField] private RectTransform islandOnMap;
    /// <summary>Reference to the city planet on the mini map</summary>
    [SerializeField] private RectTransform cityOnMap;
    /// <summary>Reference to the space station on the mini map</summary>
    [SerializeField] private RectTransform stationOnMap;
    /// <summary>Reference to the hope ship on the mini map</summary>
    [SerializeField] private RectTransform hopeOnMap;

    /// <summary>Reference to the quest marker for the island planet</summary>
    [Header("Quest Markers"), SerializeField] private GameObject islandQuestMarker;
    /// <summary>Reference to the quest marker for the city planet</summary>
    [SerializeField] private GameObject cityQuestMarker;
    [SerializeField] private GameObject moonQuestMarker;

    /// <summary>
    /// Sets the positions of the space objects on the map and sets up the correct progression state
    /// </summary>
    private void Start()
    {
        sunOnMap.localPosition = Vector3.zero;
        gasOnMap.localPosition = (gasGiant.position - sun.position) / divisionFactorForMap;
        islandOnMap.localPosition = (islandPlanet.position - sun.position) / divisionFactorForMap;
        cityOnMap.localPosition = (cityPlanet.position - sun.position) / divisionFactorForMap;
        stationOnMap.localPosition = (spaceStation.position - sun.position) / divisionFactorForMap;

        if (ProgressionManager.Instance.Flags.Contains(EProgressionFlag.IslandHint))
            islandQuestMarker.SetActive(false);
        if (ProgressionManager.Instance.Flags.Contains(EProgressionFlag.ShipSellerHint)) 
            cityQuestMarker.SetActive(false);
        if (ProgressionManager.Instance.Flags.Contains(EProgressionFlag.ShipSellerHint) &&
            ProgressionManager.Instance.Flags.Contains(EProgressionFlag.IslandHint))
        {
            moonQuestMarker.SetActive(true);
            moonRing.SetActive(true);
        }
    }

    /// <summary>
    /// Updates the position of the hope ship on the map
    /// </summary>
    private void Update()
    {
        hopeOnMap.localPosition = Vector3.ClampMagnitude(hopeShip.position - sun.position, mapRadius) / divisionFactorForMap;
    }
}
