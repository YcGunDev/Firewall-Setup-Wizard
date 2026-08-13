using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridFill : MonoBehaviour
{
    [SerializeField] Tilemap bgMap;
    [SerializeField] Tile testTile;

    [SerializeField] Vector2 mapDimesnions;
    [SerializeField] Vector3Int currentPos;
    [SerializeField] float fillDelay;

    [SerializeField] Color grey;
    [SerializeField] Color green;

    private void Awake()
    {
        currentPos = Vector3Int.zero;
        bgMap.ClearAllTiles();
        StartCoroutine(FillMapC());
    }
    
    //this is line by line which im not a fan of ngl, plus its too slow
    IEnumerator FillMapA()
    {
        //if there is a tile at the position go next
        for (int i = 0; i < mapDimesnions.x + 1; i++)
        {
            for (int j = 0; j < mapDimesnions.y + 1; j++)
            {
                currentPos = new Vector3Int(i, j);
                if (bgMap.HasTile(currentPos)) continue;
                else//no tile, for now lets gamble
                {
                    float rand = Random.Range(0f, 1f);
                    if (rand <= 0.33f)
                    {
                        bgMap.SetTile(currentPos, testTile);
                    }
                }
                //yield return new WaitForSeconds(fillDelay);
                yield return null;
            }
        }
        yield return null;

    }

    IEnumerator FillMapB()
    {
        Queue<Vector3Int> tiles = new Queue<Vector3Int>();

        currentPos = new Vector3Int((int)-mapDimesnions.x / 2, (int)-mapDimesnions.y / 2);
        //add the first corner one
        tiles.Enqueue(currentPos);

        while (tiles.Count > 0)
        {
            for (int k = 0; k < 5; k++)
            {
                if (!tiles.TryDequeue(out Vector3Int currentTile)) break;

                float rand = Random.Range(0f, 1f);
                testTile.color = Color.Lerp(grey, green, rand);
                bgMap.SetTile(currentTile, testTile);
                //bgMap.SetColor(currentTile, Color.Lerp(grey, green, rand));

                //check neighbours
                for (int i = 0; i <= 1; i++)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        if (i == 0 && j == 0 || i == 1 && j == 1) continue; //ignore self and corners
                        Vector3Int neighbourTile = new Vector3Int(i, j);
                        neighbourTile += currentTile;


                        //check if its within bounds
                        if (neighbourTile.x >= (int)-mapDimesnions.x / 2 && neighbourTile.x <= (int)mapDimesnions.x / 2 &&
                            neighbourTile.y >= (int)-mapDimesnions.y / 2 && neighbourTile.y <= (int)mapDimesnions.y / 2)
                        {
                            //if it passes all these tests, then add it to the queue uniquely
                            if (!tiles.Contains(neighbourTile))
                                tiles.Enqueue(neighbourTile);
                        }
                    }
                }
            }

            
            yield return new WaitForSeconds(fillDelay);
        }
    }

    IEnumerator FillMapC()
    {
        List<Vector3Int> tiles = new List<Vector3Int>();
        for (int i = (int)-mapDimesnions.x / 2; i <= (int)mapDimesnions.x / 2; i++)
        {
            for (int j = (int)-mapDimesnions.y / 2; j <= (int)mapDimesnions.y / 2; j++)
            {
                tiles.Add(new Vector3Int(i, j));
            }
        }

        while (tiles.Count > 0)
        {
            for (int k = 0; k < 5; k++)
            {
                if (tiles.Count <= 0) break;
                int rand1 = Random.Range(0, tiles.Count);

                float rand2 = Random.Range(0f, 1f);
                testTile.color = Color.Lerp(grey, green, rand2);
                bgMap.SetTile(tiles[rand1], testTile);
                tiles.Remove(tiles[rand1]);
            }

            

            yield return new WaitForSeconds(fillDelay);
        }
    }
}
