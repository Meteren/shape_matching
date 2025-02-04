using System.Linq;
using UnityEngine;

public class ShuffleBoard
{
    public Block[,] blocks;
   
    public void InitShuffle(Board board)
    { 
        AssignBlocks(board.blocks);
        for (int i = 0; i < blocks.GetLength(0); i++)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                if (blocks[i,j] != null && blocks[i,j].gameObject != null)
                    if (blocks[i,j].groupedBlocks.Count() == 1 && !blocks[i,j].isMoving)
                        IterateOverAdjacents(blocks[i, j]);       
            }
        }
    }
    public void AssignBlocks(Block[,] blocks)
    {
        this.blocks = blocks;
    }

    private void IterateOverAdjacents(Block block)
    {
        foreach(var adjacent in block.adjacents)
        {
            if (!adjacent.isMoving && block.GetType() != adjacent.GetType() && adjacent.groupedBlocks.Count() == 1)
            {
                foreach (var adj_block in adjacent.adjacents)
                {
                    if (adj_block != block && adj_block.GetType() == block.GetType() && !adj_block.isMoving)
                    {
                        SwapPositions(block, adjacent);
                        return;
                    }
                }
            }
            
        }
           
    }
    private void SwapPositions(Block block, Block adjacent)
    {
        Vector2 blockPosition = block.transform.position;
        Vector2 adjacentPosition = adjacent.transform.position;
        block.MovePosition(blockPosition, adjacentPosition, adjacent.X, adjacent.Y);
        adjacent.MovePosition(adjacentPosition, blockPosition, block.X, block.Y);
    }
}
