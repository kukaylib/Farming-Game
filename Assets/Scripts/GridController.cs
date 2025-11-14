using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public static GridController instance;

    private void Awake()
    {
        instance = this;
    }

    public Transform minPoint, maxPoint; // 网格的最小和最大点（用于定义网格范围）
    public GrowBlock baseGridBlock; // 基础网格块预制件

    public Vector2Int gridSize;    // 网格尺寸（行数和列数）

    public List<BlockRow> blockRows = new List<BlockRow>(); // 网格数据结构，包含所有行

    public LayerMask gridBlockers; // 用于检测网格块阻挡的图层

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateGrid()
    {
        // 将最小和最大点的位置四舍五入到整数值，确保网格对齐
        minPoint.position = new Vector3(Mathf.Round(minPoint.position.x), Mathf.Round(minPoint.position.y), 0f);
        maxPoint.position = new Vector3(Mathf.Round(maxPoint.position.x), Mathf.Round(maxPoint.position.y), 0f);

        Vector3 startPoint = minPoint.position + new Vector3(0.5f, 0.5f, 0f);

        // 计算网格尺寸
        gridSize = new Vector2Int(Mathf.RoundToInt(maxPoint.position.x - minPoint.position.x), Mathf.RoundToInt(maxPoint.position.y - minPoint.position.y));

        for (int x = 0; x < gridSize.x; x++)
        {
            blockRows.Add(new BlockRow());
            for (int y = 0; y < gridSize.y; y++)
            {
                // GrowBlock newBlock = Instantiate(baseGridBlock, startPoint + new Vector3(x, y, 0f), Quaternion.identity);
                GrowBlock newBlock = Instantiate(baseGridBlock, startPoint + new Vector3(x, y, 0f), Quaternion.identity);
                newBlock.transform.SetParent(transform);
                newBlock.theSR.sprite = null; // 设置为空精灵，隐藏网格块

                blockRows[x].blocks.Add(newBlock);
                // 检测该位置是否有阻挡物
                if (Physics2D.OverlapBox(newBlock.transform.position, new Vector2(0.9f, 0.9f), 0f, gridBlockers))
                {
                    // newBlock.isBlocked = true;
                    // newBlock.GetComponent<SpriteRenderer>().color = Color.red;
                    newBlock.theSR.sprite = null; // 设置为空精灵，隐藏网格块
                    newBlock.preverntUse = true; // 设置为不可用，防止重复使用
                }
                else
                {
                    // newBlock.isBlocked = false;
                    // newBlock.GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }

        baseGridBlock.gameObject.SetActive(false);

    }

    public GrowBlock GetBlock(float x, float y)
    {
        x = Mathf.RoundToInt(x - 0.5f);
        y = Mathf.RoundToInt(y - 0.5f);

        x -=minPoint.position.x;
        y -= minPoint.position.y;

        int intX = Mathf.RoundToInt(x);
        int intY = Mathf.RoundToInt(y);
        if (intX < gridSize.x && intY >= 0 && intY < gridSize.y)
        {
            return blockRows[intX].blocks[intY];
        }
        return null;
    }
}

[System.Serializable]
public class BlockRow 
{
    public List<GrowBlock> blocks = new List<GrowBlock>();

}
