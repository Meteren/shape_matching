using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Board
{
    public Block[,] blocks;
    public GameObject[,] placeHolders;

    public int A { get; set; }
    public int B { get; set; }
    public int C { get; set; }

    float spawnInterval = 0.15f;
    float spawnYOffset = 5f;

    List<int> columnIndexes = new List<int>();

    BlockGenerator blockGenerator => GameManager.instance.blockGenerator;
    public Board(Block[,] blocks,GameObject[,] placeHolders, int A, int B, int C)
    {
        this.blocks = blocks;
        this.placeHolders = placeHolders;
        this.A = A;
        this.B = B;
        this.C = C;
    }

    public void CheckColumns(HashSet<Block> groupedBlocks)
    {
        foreach(var block in groupedBlocks)
        {
            columnIndexes.Add(block.Y);
        }
        columnIndexes = columnIndexes.Distinct().ToList();
        foreach(var block in groupedBlocks.ToList())
        {
            block.groupedBlocks.Clear();
        }
        IterateOverColumns(columnIndexes);
    }

    private void IterateOverColumns(List<int> columnIndexes)
    {
        foreach(var i in columnIndexes)
        {
            List<Block> reversedColumnBlocks = Enumerable.Range(0,blocks.GetLength(0))
                .Select(x => blocks[x, i]).Where(block => block != null).Reverse().ToList();
            MoveBlocks(reversedColumnBlocks,i);
        }
    }

    private void MoveBlocks(List<Block> columnBlocks,int columnIndex)
    {
        
        HashSet<int> columnBlocksDropped = new HashSet<int>();
        List<Block> destroyedOnes = columnBlocks.Where(block => block.isDestroyed).ToList();
        int counter = destroyedOnes.Count();
        for (int i = 0; i < columnBlocks.Count; i++)
        {
            if (columnBlocks[i].isDestroyed || columnBlocksDropped.Contains(i))
            {
                for (int j = i + 1; j < columnBlocks.Count; j++)
                {
                    if (!columnBlocks[j].isDestroyed && !columnBlocks[j].isMoving 
                        && !columnBlocksDropped.Contains(j))
                    {           
                        blocks[columnBlocks[j].X, columnBlocks[j].Y] = null;     
                        columnBlocks[j].MovePosition(
                            columnBlocks[j].transform.position, columnBlocks[i].transform.position,
                            columnBlocks[i].X, columnBlocks[i].Y);
                        columnBlocksDropped.Add(j);
                        break;
                    }
                }
              
            }

        }  
        GameManager.instance.StartCoroutine(FillBlanks(counter,columnIndex));
    }
    private IEnumerator FillBlanks(int destroyedCount,int columnIndex)
    {
        Vector2 spawnPoint = new Vector2(
            placeHolders[0,columnIndex].transform.position.x, placeHolders[0, columnIndex].transform.position.y + spawnYOffset);
        for (int i = destroyedCount - 1; i >= 0; i--)
        {
            Block spawnedBlock = SpawnBlock(spawnPoint);
            spawnedBlock.MovePosition(spawnPoint, placeHolders[i,columnIndex].transform.position, i, columnIndex);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    private Block SpawnBlock(Vector2 spawnPoint)
    {
        int rand = Random.Range(0, blockGenerator.ChoosedBlocks.Count());
        Block blockToBeInstantiated = blockGenerator.ChoosedBlocks[rand];
        Block block = GameObject.Instantiate(blockToBeInstantiated, spawnPoint, 
            Quaternion.identity,GameManager.instance.blockGenerator.transform);
        block.Init(0, 0, 0);
        return block;
    }

}
