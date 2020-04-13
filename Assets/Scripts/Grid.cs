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
    public float reward;
    public float value;
    public bool invalMove;
    public GameObject direction;

    public GridBlock()
    {
        x = y = 0;
        obj = direction = null;
        value = reward = -1;
        invalMove = false;
    }

    public GridBlock(int x, int y, GameObject obj, float value, GameObject direction, float reward, bool invalMove)
    {
        this.x = x;
        this.y = y;
        this.obj = obj;
        this.value = value;
        this.direction = direction;
        this.reward = reward;
        this.invalMove = invalMove;
    }
}

public class Grid : MonoBehaviour
{
    // Grid Blocks
    public GameObject redBlock;
    public GameObject greenBlock;
    public GameObject plainBlock;

    // Block Arrows
    public GameObject left;
    public GameObject up;
    public GameObject down;
    public GameObject right;

    public List<List<GridBlock>> grid = new List<List<GridBlock>>();

    public bool mapSet = false;

    //private bool rewardSet = false;
    private float tileSize = .9f;
    private float t;
    // Start is called before the first frame update
    void Start()
    {
        t = 0;
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
                    block.value = 0;
                    block.reward = 100;
                    block.invalMove = true;
                }
                else if (el == '1')
                {
                    block.obj = Instantiate(redBlock, this.transform);
                    block.value = -0;
                    block.reward = -100;
                    block.invalMove = true;
                }
                else
                {
                    block.obj = Instantiate(plainBlock, this.transform);
                    block.value = -1;
                    block.reward = 0;
                    block.invalMove = false;
                }
                // placement
                block.obj.GetComponent<RectTransform>().transform.position = new Vector3(worldStart.x + tileSize / 2 + (tileSize * j),
                                                              worldStart.y - tileSize / 2 - (tileSize * i),
                                                              0);
                Debug.Log(block.obj.GetComponent<RectTransform>().transform.position);
                block.x = i;
                block.y = j;
                gridline.Add(block);
                ++j;
            }
            grid.Add(gridline);
        }
        mapSet = true;        
    }

    void ValueIteration(float theta, float discount)
    {
        bool convergence = false;
        // one iteration through entire grid
        for (int i=0; i<10; i++) {
            for (int j=0; j<10; j++) {
                float oldVal = grid[i][j].value;
                float value;
                bool placed = false;
                // for up
                if (i > 0 && !grid[i][j].invalMove) {
                    value = grid[i - 1][j].reward + (discount * grid[i - 1][j].value);
                    if (value > grid[i][j].value) {
                        grid[i][j].value = value;
                        if (grid[i][j].direction != null)
                            Destroy(grid[i][j].direction);
                        grid[i][j].direction = Instantiate(up, grid[i][j].obj.GetComponent<RectTransform>().transform);
                        placed = true;
                    }
                }
                // for right
                if (j < 9 && !grid[i][j].invalMove) {
                    value = grid[i][j + 1].reward + (discount * grid[i][j + 1].value);
                    if (value > grid[i][j].value) {
                        grid[i][j].value = value;
                        if (grid[i][j].direction != null)
                            Destroy(grid[i][j].direction);
                        grid[i][j].direction = Instantiate(right, grid[i][j].obj.GetComponent<RectTransform>().transform);
                        placed = true;
                    }
                }
                // for left 
                if (j > 0 && !grid[i][j].invalMove) {
                    value = grid[i][j - 1].reward + (discount * grid[i][j - 1].value);
                    if (value > grid[i][j].value) {
                        grid[i][j].value = value;
                        if (grid[i][j].direction != null)
                            Destroy(grid[i][j].direction);
                        grid[i][j].direction = Instantiate(left, grid[i][j].obj.GetComponent<RectTransform>().transform);
                        placed = true;
                    }
                }
                // for down
                if (i < 9 && !grid[i][j].invalMove) {
                    value = grid[i + 1][j].reward + (discount * grid[i+1][j].value);
                    if (value > grid[i][j].value) {
                        grid[i][j].value = value;
                        if (grid[i][j].direction != null)
                            Destroy(grid[i][j].direction);
                        grid[i][j].direction = Instantiate(down, grid[i][j].obj.GetComponent<RectTransform>().transform);
                        placed = true;
                    }
                }
                if (placed) {
                    grid[i][j].direction.GetComponent<RectTransform>().transform.position = new Vector3(grid[i][j].obj.GetComponent<RectTransform>().transform.position.x,
                                                                                                grid[i][j].obj.GetComponent<RectTransform>().transform.position.y,
                                                                                                -1);
                    Debug.Log("placed");
                }
                if (Mathf.Abs(oldVal - grid[i][j].value) <= theta) { 
                    convergence = true;
                    //break;
                }
            }
            /*if (convergence)
                break;*/
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (t % 10 == 0) { 
            ValueIteration(0.01f, 0.9f);
            Debug.Log("one run through value iter done");
        }
        t++;
    }
}
