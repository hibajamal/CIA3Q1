using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.EventSystems;
using System.IO;

public class GridBlock
{
    public int x;
    public int y;
    public GameObject obj;
    public float value;

    public GridBlock()
    {
        x = y = 0;
        obj = null;
        value = -1;
    }

    public GridBlock(int x, int y, GameObject obj, float value)
    {
        this.x = x;
        this.y = y;
        this.obj = obj;
        this.value = value;
    }
}

public class Grid : MonoBehaviour
{
    public GameObject redBlock;
    public GameObject greenBlock;
    public GameObject plainBlock;

    public List<List<GridBlock>> grid = new List<List<GridBlock>>();

    public bool mapSet = false;

    //private bool rewardSet = false;
    private float tileSize = .9f;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 worldStart = GetComponent<RectTransform>().transform.position;
        //for (int i=0; i<10; i++)
        int i = -1;
        foreach (string line in File.ReadLines(@"Assets/Gridworlds/1.txt", Encoding.UTF8))
        {
            ++i;
            List<GridBlock> gridline = new List<GridBlock>();
            GridBlock block = new GridBlock();
            //for (int j=0; j<10; j++)
            int j = 0;
            foreach (char el in line)
            {
                if (el == '2')
                {
                    block.obj = Instantiate(greenBlock, this.transform);
                    block.value = 100;
                }
                else if (el == '1')
                {
                    block.obj = Instantiate(redBlock, this.transform);
                    block.value = -100;
                }
                else
                {
                    block.obj = Instantiate(plainBlock, this.transform);
                    block.value = -1;
                }
                // placement
                block.obj.GetComponent<RectTransform>().transform.position = new Vector3(worldStart.x + tileSize / 2 + (tileSize * j),
                                                              worldStart.y - tileSize / 2 - (tileSize * i),
                                                              0);
                block.x = i;
                block.y = j;
                gridline.Add(block);
                ++j;
            }
            grid.Add(gridline);
        }
        mapSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
