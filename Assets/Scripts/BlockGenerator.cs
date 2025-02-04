using Cinemachine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private CinemachineVirtualCamera cam;
    public List<Block> blockReferences;
    private List<Block> choosedBlocks = new List<Block>();
    
    public Board board;

    [Header("Row Count")]
    [SerializeField] private int m;
    [Header("Column Count")]
    [SerializeField] private int n;
    [Header("A Threshold")]
    [SerializeField] private int a;
    [Header("B Threshold")]
    [SerializeField] private int b;
    [Header("C Threshold")]
    [SerializeField] private int c;
    [Header("Number Of Colors")]
    [SerializeField] private int k;

    private float currentOrthoSize = 10f;
    private float defaultOrthoSize = 16f;
    private float camYOffset = 0.5f;
    private float orthoYOffset = 3f;

    public int M { get { return m; } }
    public int N { get { return n; } }

    public int K { get { return k; } }

    public List<Block> ChoosedBlocks { get { return choosedBlocks; } }
    
    private void Start()
    {
        ChooseColors();
        cam.m_Lens.OrthographicSize = defaultOrthoSize;
        board = GenerateBlocks();  
    }

    private Board GenerateBlocks()
    {
        
        Block[,] blocks = new Block[m, n];
        GameObject[,] placeHolders = new GameObject[m, n];
        float posX = spawnPoint.transform.position.x;
        float posY = spawnPoint.transform.position.y;
        Block tempBlock = Instantiate(choosedBlocks[0]);
        float sizeX = tempBlock.gameObject.GetComponent<Collider2D>().bounds.size.x;
        float sizeY = tempBlock.gameObject.GetComponent<Collider2D>().bounds.size.y;
        Destroy(tempBlock.gameObject);
        int layer = m;

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int random = Random.Range(0, choosedBlocks.Count);
                Block block = Instantiate(choosedBlocks[random], new Vector2(posX, posY), Quaternion.identity, this.transform);
                GameObject placeholder = new GameObject($"PlaceHolder{i}-{j}");
                placeholder.transform.position = new Vector2(posX, posY);
                placeHolders[i, j] = placeholder;
                block.Init(i, j, layer);
                posX += sizeX;
                blocks[i, j] = block;
            }
            posX = spawnPoint.transform.position.x;
            posY -= sizeY;
            layer--;
        }

        SetCamDistance(sizeX, sizeY);
        SetCenterPointOfBoard(sizeX, sizeY);

        return new Board(blocks, placeHolders, a, b, c);

    }

    private void SetCamDistance(float sizeX, float sizeY)
    {
        float multFactorX = (sizeX * n) / cam.m_Lens.OrthographicSize;
        float multFactorY = (sizeY * m) / cam.m_Lens.OrthographicSize;

        if (m >= n)
        {
            currentOrthoSize *= multFactorY;
            cam.m_Lens.OrthographicSize = currentOrthoSize;
        }
        else
        {
            currentOrthoSize *= multFactorX;
            cam.m_Lens.OrthographicSize = currentOrthoSize - orthoYOffset;
        }
    }

    private void SetCenterPointOfBoard(float sizeX, float sizeY)
    {         
        float y = (spawnPoint.transform.position.y - ((sizeY * m) - (sizeY / 2)) / 2) + camYOffset;
        float x = (spawnPoint.transform.position.x + ((sizeX * n) - (sizeX / 2)) / 2);
        Vector3 boardCenter = new Vector3(x, y, -1);
        cam.transform.position = boardCenter;
    }

    private void OnValidate()
    {
        m = Mathf.Clamp(M, 2, 10);
        n = Mathf.Clamp(N, 2, 10);
        k = M < 3 || N < 3 ? 2 : Mathf.Clamp(K, 1, 6);
    }

    private void ChooseColors()
    {
        List<Block> referenceCopy = new List<Block>(blockReferences);
        for (int i = 0; i < K; i++)
        {
            int random = Random.Range(0, referenceCopy.Count());
            Block reference = referenceCopy[random];
            choosedBlocks.Add(reference);
            referenceCopy.Remove(reference);
        }

        Debug.Log("Selected Colors Count:" + choosedBlocks.Count());

    }
}
