using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public BlockGenerator blockGenerator;
    public HashSet<Block> groupedBlocks = new HashSet<Block>();
    bool init = true;
    public bool cancelClick;
    List<Block> blocks = new List<Block>();
    public List<Block> movingBlocks = new List<Block>();
    public ShuffleBoard shuffle = new ShuffleBoard();
    public bool waitForShuffle;
    [SerializeField] private float shuffleWaitAmount;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (waitForShuffle)
        {
            Debug.Log("Cancel click:" + cancelClick);
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && movingBlocks.Count() == 0)
        {
            Debug.Log("Shuffle initted");
            StartCoroutine(StartShuffling());
        }
        
        if (IsItLocked() && movingBlocks.Count() == 0 && !cancelClick)
        {
            Debug.Log("Locked");
            StartCoroutine(StartShuffling());
        }

        if (init)
        {
            ExractBlockList();
            init = !init;
        }
        CancelClickCheck();
        ClearPath();

    }

    private void CancelClickCheck()
    {
        bool moving = AreBlocksMoving(movingBlocks);

        if (moving)
        {
            return;
        }

        if (cancelClick)
        {
            ExractBlockList();
            Debug.Log("Extracted");
        }
        cancelClick = false;

    }

    private bool IsItLocked()
    {
        int counter = 0;
        if (movingBlocks.Count() != 0)
            return false;

        foreach (var block in blocks)
        {
            if (block.groupedBlocks.Count() == 1)
            {
                counter++;
            }

        }
        Debug.Log("Lock Status Counter:" + counter);
        return counter == blockGenerator.M * blockGenerator.N ? true : false;
    }

    private bool AreBlocksMoving(List<Block> blocksToCheck)
    {
        foreach (var block in blocksToCheck)
        {
            if (block != null)
            {
                if (block.isMoving)
                {
                    cancelClick = true;

                    return true;
                }
            }

        }
        return false;
    }

    private void ExractBlockList()
    {
        blocks.Clear();
        for (int i = 0; i < blockGenerator.board.blocks.GetLength(0); i++)
        {
            for (int j = 0; j < blockGenerator.board.blocks.GetLength(1); j++)
            {
                blocks.Add(blockGenerator.board.blocks[i, j]);
                
            }
        }
        
    }

    private void ClearPath()
    {
        foreach (var block in blocks)
        {
            if(block != null)
                if(!block.isMoving)
                    block.groupedBlocks.Clear();
        }
    }

    private IEnumerator StartShuffling()
    {
        UIController.instance.shuffleText.SetActive(true);
        waitForShuffle = true;
        yield return new WaitForSeconds(shuffleWaitAmount);
        UIController.instance.shuffleText.SetActive(false);
        shuffle.InitShuffle(blockGenerator.board);
        waitForShuffle = false;
           
    }
 
}
