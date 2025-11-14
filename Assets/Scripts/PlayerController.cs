using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // 使用 Unity 新输入系统（Input System）
#endif

// 确保物体上一定存在 Rigidbody2D 组件（若缺失会在挂载脚本时自动添加）
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // 角色刚体：用于基于物理的移动
    public Rigidbody2D theRB;

    // 角色移动速度（单位/秒）：用于控制移动快慢
    public float moveSpeed;

    // 输入动作引用：应在 Inspector 中绑定到“移动”二维向量输入（如 WASD/摇杆）
    public InputActionReference moveInput, actionInput;

    // 动画控制器：根据速度大小切换动画（Idle/Walk/Run 等）
    public Animator anim;
    // 定义可用工具类型
    public enum ToolType
    {
        plough, // 犁地
        wateringCan, // 浇水
        seeds, // 种子
        basket // 篮子  
    }

    public float toolWaitTime = 0.5f; // 使用工具的等待时间（秒）
    private float toolWaitCounter; // 工具等待计时器
    public float toolRange = 3f; // 工具使用范围
    // 当前使用的工具
    public ToolType currentTool;

    // 工具指示器
    public Transform toolIndicator; 
     
    void Start()
    {
        UIController.instance.SwitchTool((int)currentTool);
    }
    // 每帧调用：读取输入 -> 计算速度 -> 设置朝向 -> 更新动画参数
    void Update()
    {
        if (toolWaitCounter > 0)
        {
            toolWaitCounter -= Time.deltaTime;
            theRB.linearVelocity = Vector2.zero;
        }
        else
        {
            // 1) 读取二维输入（x=水平，y=垂直）
            // 2) normalized 保持斜向与直线同等速度
            // 3) 乘以移动速度，得到最终线性速度
            theRB.linearVelocity = moveInput.action.ReadValue<Vector2>().normalized * moveSpeed;

            // 根据水平速度方向翻转角色的朝向（通过缩放 x 实现左右朝向切换）
            if (theRB.linearVelocity.x < 0f)
            {
                // 向左移动：x 取 -1
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else if (theRB.linearVelocity.x > 0f)
            {
                // 向右移动：恢复为 (1,1,1)
                transform.localScale = Vector3.one;
            }
        }
         
    

        bool hasSwitchedTool = false;
        // 测试切换工具
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            // 切换当前工具
            currentTool++;
            if ((int)currentTool >= Enum.GetNames(typeof(ToolType)).Length)
            {
                currentTool = ToolType.plough;
            }

            hasSwitchedTool = true;
        }
        // 数字键 1-4 直接选择工具
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            currentTool = ToolType.plough;
            hasSwitchedTool = true;
        }
        else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            currentTool = ToolType.wateringCan;
            hasSwitchedTool = true;
        }
        else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            currentTool = ToolType.seeds;
            hasSwitchedTool = true;
        }
        else if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            currentTool = ToolType.basket;
            hasSwitchedTool = true;
        }

        // 如果切换了工具，更新 UI 显示
        if (hasSwitchedTool == true)
        {
            // FindFirstObjectByType<UIController>().SwitchTool((int)currentTool);
            UIController.instance.SwitchTool((int)currentTool);

        }

        if (actionInput.action.WasPressedThisFrame())
        {
            Debug.Log("Use Tool");
            UseTool();
        }

        // 将当前速度大小传入 Animator 参数 "speed"，用于驱动状态机过渡
        anim.SetFloat("speed", theRB.linearVelocity.magnitude);

        // 设置工具指示器位置和朝向
        toolIndicator.position = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        toolIndicator.position = new Vector3(toolIndicator.position.x, toolIndicator.position.y, 0f);
        // toolIndicator.up = Camera.main.transform.up;

        if (Vector3.Distance(toolIndicator.position, transform.position) > toolRange)
        {
            Vector2 direction = toolIndicator.position - transform.position;
            direction = direction.normalized * toolRange;
            toolIndicator.position = transform.position + new Vector3(direction.x, direction.y, 0f);
        }

        toolIndicator.position = new Vector3(Mathf.FloorToInt(toolIndicator.position.x)+ 0.5f, Mathf.FloorToInt(toolIndicator.position.y)+ 0.5f, 0f); 

    }

    void  UseTool()
    {
        // 使用工具的逻辑
        GrowBlock block = null;
        // block = FindFirstObjectByType<GrowBlock>();
        block = GridController.instance.GetBlock(toolIndicator.position.x, toolIndicator.position.y);
        toolWaitCounter = toolWaitTime;

        if (block != null)
        {
            switch (currentTool)
            {
                case ToolType.plough:
                    block.PloughSoil();
                    anim.SetTrigger("usePlough");
                    break;
                case ToolType.wateringCan:
                    // 浇水逻辑
                    block.WaterSoil();
                    anim.SetTrigger("useWateringCan");
                    break;
                case ToolType.seeds:
                    // 种植逻辑
                    block.PlantCrop();
                    Debug.Log("Planting seeds.");
                    break;
                case ToolType.basket:
                    // 收获逻辑
                    block.HarvestCrop();
                    Debug.Log("Harvesting crops.");
                    break;
                default:
                    Debug.Log("Unknown tool.");
                    break;
            }
        }
    } 
}
