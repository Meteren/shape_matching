using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    public enum Types
    {  
        A,
        B,
        C,
        DefaultType,
    }

    public Types currentType;
    [SerializeField] private List<Sprite> types;
    int[,] directions;

    private int multiplyFactor;

    private float moveSpeed = 3f;
    public int X {  get; private set; }
    public int Y { get; private set; }

    public Board board => GameManager.instance.blockGenerator.board;

    [HideInInspector]
    public List<Block> adjacents = new List<Block>();
    [HideInInspector]
    public HashSet<Block> groupedBlocks = new HashSet<Block>();

    SpriteRenderer spriteRenderer;

    [Header("Conditions")]
    public bool isMoving;
    public bool isDestroyed;

    private void Start()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        UpdateAdjacents();

    }
    private void Update()
    {
      
        if (!GameManager.instance.cancelClick)
        {
                      
            ClearDestroyedGroupBlocks();

        }
        
    }

    public void SetMultiplyFactor()
    {
        multiplyFactor = currentType == Types.DefaultType ? 1 : Convert.ToInt16(currentType) + 2;
    }

    private void ClearDestroyedGroupBlocks()
    {
        groupedBlocks = groupedBlocks.Where(block => block).ToHashSet();
        UpdateAdjacents();    
        DrawPath(this);
        UpdateType();
    }

    public void Init(int X, int Y,int layer)
    {
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = layer;
        this.X = X;
        this.Y = Y;
        directions = new int[4, 2]
        {
            {1,0},{-1,0},{0,-1},{0,1}
        };
    }

    public void UpdateAdjacents()
    {
        ClearAdjacents();   
        for(int i = 0; i < directions.GetLength(0); i++)
        { 
            int newX = X + directions[i, 0];
            int newY = Y + directions[i, 1];

            if (newX >= 0 && newX < board.blocks.GetLength(0) &&
                newY >= 0 && newY < board.blocks.GetLength(1) )
            {
                adjacents.Add(board.blocks[newX, newY]);
            }
        }
    }

    public void ClearAdjacents()
    {
        adjacents.Clear();
    }

    public void UpdateType()
    {
        
        if(groupedBlocks.Count < board.A)
        {
            currentType = Types.DefaultType;

        }else if(groupedBlocks.Count >= board.A && groupedBlocks.Count < board.B)
        {
           currentType = Types.A;

        }
        else if (groupedBlocks.Count >= board.B && groupedBlocks.Count < board.C)
        {
            currentType = Types.B;

        }
        else if (groupedBlocks.Count >= board.C)
        {
            currentType = Types.C;

        }
        
        spriteRenderer.sprite = types[Convert.ToInt16(currentType)];
    }

    public void DrawPath(Block currentBlock, HashSet<Block> visitedBlocks = null, bool calledAfterFall = false)
    {

        if (visitedBlocks == null)
        {
            visitedBlocks = new HashSet<Block>();
        }

        if (visitedBlocks.Contains(this))
            return;

        visitedBlocks.Add(this);

        if (!currentBlock.groupedBlocks.Contains(this))
        {
            currentBlock.groupedBlocks.Add(this);
        }

        foreach (var adjacent in adjacents)
        {
            if(adjacent != null)
            {
                if (adjacent.GetType() == this.GetType())
                {
                    adjacent.DrawPath(currentBlock, visitedBlocks);
                }
            }
           
        }

    }
    public void MovePosition(Vector2 originPosition, Vector2 positionToMove,int X, int Y)
    {
        isMoving = true;
        GameManager.instance.movingBlocks.Add(this);
        StartCoroutine(Move(originPosition,positionToMove,X,Y));
    }

    private IEnumerator Move(Vector2 originPosition, Vector2 positionToMove,int newX, int newY)
    {
        for(float i = 0; i < 1; i+=Time.deltaTime * moveSpeed)
        {
            float posX = Mathf.Lerp(originPosition.x, positionToMove.x, i);
            float posY = Mathf.Lerp(originPosition.y,positionToMove.y,i);
            transform.position = new Vector2(posX, posY);
            yield return null;
        }
        transform.position = positionToMove;
        UpdatePlaceOnBoard(newX, newY);      
        groupedBlocks.Clear();
        GetComponent<SpriteRenderer>().sortingOrder = GameManager.instance.blockGenerator.M - newX;
        isMoving = false;
        GameManager.instance.movingBlocks.Remove(this);
              
    }

    public void UpdatePlaceOnBoard(int newX, int newY)
    {
        this.X = newX;
        this.Y = newY;  
        board.blocks[newX, newY] = this;
    }

    private void OnMouseDown()
    {   
        UIController.instance.handleScore.IncrementScore(groupedBlocks.Count() * multiplyFactor);
        if(groupedBlocks.Count > 1 && !GameManager.instance.cancelClick && !GameManager.instance.waitForShuffle)
        {
            foreach (var block in groupedBlocks)
            {
                if (!block.isDestroyed)
                {
                    Destroy(block.gameObject);
                    block.isDestroyed = true;
                }
                                
            }
        }                 
        board.CheckColumns(groupedBlocks);
        
    }

}
