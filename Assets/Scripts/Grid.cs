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

    //public List<List<GridBlock>> grid = new List<List<GridBlock>>();
    public List<GridBlock> grid = new List<GridBlock>();
    public bool mapSet;
    public bool run;
    public bool converged;
    public float discountRate = 0.9f;

    //private bool rewardSet = false;
    private float tileSize = .6f;
    private float t;
    private Vector3 worldStart;

    // sample grids
    private List<List<string>> sampleGrids = new List<List<string>>();
    
    // Start is called before the first frame update
    void Start()
    {
        // add three grids
        sampleGrids.Add(new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "0", "1", "1", "1", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "2" });
        sampleGrids.Add(new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "0", "0", "1", "1", "1", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "1", "1", "1", "2" });
        sampleGrids.Add(new List<string> { "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "2", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "1", "1", "1", "1", "1", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0", "0" });

        mapSet = false;
        run = false;
        t = 0;
        worldStart = GetComponent<RectTransform>().transform.position;
        discountRate = 0.9f;
        Debug.Log(discountRate);
        //CreateGrid("1");
    }

    public void CreateGrid(int textfile)
    {
        converged = false;
        //int i = 0;
        List<string> lines = sampleGrids[textfile];
        for (int i=0; i<10; i++)
        //foreach (string line in File.ReadLines(@"Assets/Gridworlds/"+textfile+".txt", Encoding.UTF8))
        {
            //int j = 0;
            for (int j=0; j<10; j++)
            //foreach (char el in line)
            {
                GridBlock block = new GridBlock();
                if (lines[(i*10)+j] == "2")
                {
                    block.obj = Instantiate(greenBlock, this.transform);
                    block.value = 0;
                    block.reward = 100;
                    block.invalMove = true;
                }
                else if (lines[(i * 10) + j] == "1")
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
                block.x = i;
                block.y = j;
                grid.Add(block);
                //++j;
            }
            //i++;
        }
        mapSet = true;
    }

    void ValueIteration(float theta, float discount)
    {
        int convergence = 0;
        // one iteration through entire grid
        for (int i=0; i<10; i++) {
            for (int j=0; j<10; j++) {
                float oldVal = grid[(i * 10) + j].value;
                float value;
                bool placed = false;
                // for up
                if (i > 0 && !grid[(i * 10) + j].invalMove) {
                    value = grid[((i - 1)*10)+j].reward + (discount * grid[((i - 1) * 10)+ j].value);
                    if (value > grid[(i * 10) + j].value) {
                        grid[(i * 10) + j].value = value;
                        if (grid[(i * 10) + j].direction != null)
                            Destroy(grid[(i * 10) + j].direction);
                        grid[(i * 10) + j].direction = Instantiate(up, grid[(i * 10) + j].obj.GetComponent<RectTransform>().transform);
                        placed = true;
                    }
                }
                // for right
                if (j < 9 && !grid[(i * 10) + j].invalMove) {
                    value = grid[(i * 10) + (j + 1)].reward + (discount * grid[(i * 10) + (j + 1)].value);
                    if (value > grid[(i * 10) + j].value) {
                        grid[(i * 10) + j].value = value;
                        if (grid[(i * 10) + j].direction != null)
                            Destroy(grid[(i * 10) + j].direction);
                        grid[(i * 10) + j].direction = Instantiate(right, grid[(i * 10) + j].obj.GetComponent<RectTransform>().transform);
                        placed = true;
                    }
                }
                // for left 
                if (j > 0 && !grid[(i * 10) + j].invalMove) {
                    value = grid[(i * 10) + (j - 1)].reward + (discount * grid[(i * 10) + (j - 1)].value);
                    if (value > grid[(i * 10) + j].value) {
                        grid[(i * 10) + j].value = value;
                        if (grid[(i * 10) + j].direction != null)
                            Destroy(grid[(i * 10) + j].direction);
                        grid[(i * 10) + j].direction = Instantiate(left, grid[(i * 10) + j].obj.GetComponent<RectTransform>().transform);
                        placed = true;
                    }
                }
                // for down
                if (i < 9 && !grid[(i * 10) + j].invalMove) {
                    value = grid[((i + 1)*10)+j].reward + (discount * grid[((i + 1) * 10)+ j].value);
                    if (value > grid[(i * 10) + j].value) {
                        grid[(i * 10) + j].value = value;
                        if (grid[(i * 10) + j].direction != null)
                            Destroy(grid[(i * 10) + j].direction);
                        grid[(i * 10) + j].direction = Instantiate(down, grid[(i * 10) + j].obj.GetComponent<RectTransform>().transform);
                        placed = true;
                    }
                }
                if (placed) {
                    grid[(i * 10) + j].direction.GetComponent<RectTransform>().transform.position = new Vector3(grid[(i * 10) + j].obj.GetComponent<RectTransform>().transform.position.x,
                                                                                                grid[(i * 10) + j].obj.GetComponent<RectTransform>().transform.position.y,
                                                                                                -1);
                }
                if (Mathf.Abs(oldVal - grid[(i * 10) + j].value) <= theta) 
                    convergence++;
            }
        }
        if (convergence == 100)
            converged = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (run)
        {
            if (t % 30 == 0 && mapSet && !converged)
            {
                Debug.Log(discountRate);
                ValueIteration(0.01f, discountRate);
                if (converged) { 
                    run = false;
                    Debug.Log("converged!");
                }
            }
            t++;
        }
    }
    
    public void Clear()
    {
        for (int i=0; i<100; i++)
        {
            if (!grid[i].invalMove)
                Destroy(grid[i].direction);
            Destroy(grid[i].obj);
        }
        grid = new List<GridBlock>();
    }

    public void Rerun()
    {
        Debug.Log("rerunning");
        converged = false;
        for (int i=0; i<100; i++)
        {
            if (!grid[i].invalMove) { 
                Destroy(grid[i].direction);
                grid[i].value = -1;
            }
        }
        run = true;
    }
}
