using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance { get; private set; }

    [SerializeField] private TowerData[] towerDatas;

    private Dictionary<int, TowerData> towerDataDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeTowerDataDictionary();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeTowerDataDictionary()
    {
        towerDataDictionary = new Dictionary<int, TowerData>
        {
            { 2, towerDatas[0] },
            { 4, towerDatas[1] },
            { 8, towerDatas[2] },
            { 16, towerDatas[3] },
            { 32, towerDatas[4] },
            { 64, towerDatas[5] },
            { 128, towerDatas[6] },
            { 256, towerDatas[7] },
            { 512, towerDatas[8] },
            { 1024, towerDatas[9] },
            { 2048, towerDatas[10] }
        };
    }

    public TowerData GetTowerData(int value)
    {
        return towerDataDictionary.TryGetValue(value, out var towerData) ? towerData : null;
    }

    // Initializes the block with the appropriate tower data based on its value
    public void InitializeBlockWithTower(Block block)
    {
        TowerData towerData = GetTowerData(block.Value);
        if (towerData != null)
        {
            block.SetTowerAttributes(towerData);
        }
        else
        {
            Debug.LogError($"No TowerData found for block value: {block.Value}");
        }
    }

    // Updates the tower on the block after merging
    public void UpdateBlockTower(Block block)
    {
        TowerData towerData = GetTowerData(block.Value);
        if (towerData != null)
        {
            block.SetTowerAttributes(towerData);
        }
        else
        {
            Debug.LogError($"No TowerData found for block value: {block.Value}");
        }
    }
}
