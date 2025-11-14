using UnityEngine;
using UnityEngine.InputSystem;

public class GrowBlock : MonoBehaviour
{
    public enum GrowthStage
    {
        barren,     // 贫瘠阶段
        ploughed,   // 犁地阶段
        planted,    // 种植阶段
        growing1,   // 成长阶段1
        growing2,   // 成长阶段2
        ripe        // 成熟阶段
    }

    public GrowthStage currentStage;
    public SpriteRenderer theSR;
    public Sprite soilTilled, soilWatered;

    public SpriteRenderer cropSR; // 用于显示作物的 SpriteRenderer
    public Sprite cropPlanted, cropGrowing1, cropGrowing2, cropRipe; // 作物的不同生长阶段的精灵s

    public bool isWatered;

    public bool preverntUse; // 防止重复使用

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // AdvanceStage();
    }

    // Update is called once per frame
    void Update()
    {
        // // For testing purposes, press the "1" key to advance the growth stage
        // if (Keyboard.current.digit1Key.wasPressedThisFrame)
        // {
        //     AdvanceStage();
        //     SetSoilSprite();
        // }
        // 这里添加一个测试按键，用于推进作物生长阶段
#if UNITY_EDITOR
        // 按钮 N 用于测试推进作物生长阶段
        if (Keyboard.current.nKey.wasPressedThisFrame)
        {
            Debug.Log("Advancing crop growth stage for testing.");
            AdvanceCrop();
        }
#endif
    }

    public void AdvanceStage()
    {
        currentStage = currentStage + 1;
        if ((int)currentStage >= 6)
        {
            currentStage = GrowthStage.barren;
        }
    }

    public void SetSoilSprite()
    {
        if (currentStage == GrowthStage.barren)
        {
            theSR.sprite = null;
        }
        else
        {
            if (isWatered == true)
            {
                theSR.sprite = soilWatered;
            }
            else
            {
                theSR.sprite = soilTilled;
            }
        }
    }

    public void PloughSoil()
    {
        Debug.Log("currentStage: " + currentStage);
        if (currentStage == GrowthStage.barren && preverntUse == false)
        {
            // AdvanceStage();
            currentStage = GrowthStage.ploughed;
            SetSoilSprite();
        }
    }

    public void WaterSoil()
    {
        if(preverntUse == false)
        {
            isWatered = true;
            SetSoilSprite();
        }

    }

    public void PlantCrop()
    {
        Debug.Log("currentStage: " + currentStage);
        if (currentStage == GrowthStage.ploughed && isWatered == true && preverntUse == false)
        {
            currentStage = GrowthStage.planted;
            UpdateCropSprite();
        }
    }

    void UpdateCropSprite()
    {
        switch (currentStage)
        {
            case GrowthStage.planted:
                cropSR.sprite = cropPlanted;
                break;
            case GrowthStage.growing1:
                cropSR.sprite = cropGrowing1;
                break;
            case GrowthStage.growing2:
                cropSR.sprite = cropGrowing2;
                break;
            case GrowthStage.ripe:
                cropSR.sprite = cropRipe;
                break;
            default:
                cropSR.sprite = null; // 其他阶段不显示作物
                break;
        }
    }

    public void AdvanceCrop()
    {
        if (isWatered == true && preverntUse == false )
        {
            if (currentStage == GrowthStage.planted || currentStage == GrowthStage.growing1 || currentStage == GrowthStage.growing2)
            {
                currentStage++; // 进入下一个生长阶段
                isWatered = false; // 重置浇水状态
                SetSoilSprite(); // 更新土壤的精灵显示
                UpdateCropSprite(); // 更新作物的精灵显示     
            }
        }

    }
    
    public void HarvestCrop()
    {
        if (currentStage == GrowthStage.ripe && preverntUse == false)
        {
            // 收获作物的逻辑
            Debug.Log("Harvesting crop.");
            currentStage = GrowthStage.planted; // 收获后变为贫瘠状态
            cropSR.sprite = null; // 隐藏作物精灵
            SetSoilSprite(); // 更新土壤的精灵显示
        }
    }
}
