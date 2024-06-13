using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlowUpTiles : MonoBehaviour
{
    public Tilemap tiles;
    public TileBase emptyTile;

    public Level[] levels = new Level[1];

    public List<TileBase> tileTypes = new List<TileBase>();

    public static readonly Vector3Int[] hexagonEven = new Vector3Int[6] { // Hexagon tilesets are WEIRD and even y positions are different from odd ones
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, 0),
        };

    public static readonly Vector3Int[] hexagonOdd = new Vector3Int[6] {
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, -1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 1, 0),
        };

    void Start()
    {
        // Thanks to raarc on Unity Forums for this part
        for (int x = tiles.cellBounds.xMin; x < tiles.cellBounds.xMax; x++)
            for (int y = tiles.cellBounds.yMin; y < tiles.cellBounds.yMax; y++)
                if (tiles.GetTile(new Vector3Int(x, y)) != null && tiles.GetTile(new Vector3Int(x, y)) != emptyTile)
                    UpdateTile(new Vector3Int(x, y));

        SaveCurrentAsLevel(0);
    }

    public void BlowUp(Explosion boom)
    {
        List<Vector3Int> temp = new List<Vector3Int>();

        for (int x = tiles.WorldToCell(new Vector3(boom.location.x - boom.magnitude / 2f, 0, 0)).x - 1; x < tiles.WorldToCell(new Vector3(boom.location.x + boom.magnitude / 2f, 0)).x + 1; x++) {
            for (int y = tiles.WorldToCell(new Vector3(0, boom.location.y - boom.magnitude / 2f)).y - 1; y < tiles.WorldToCell(new Vector3(0, boom.location.y + boom.magnitude / 2f)).y + 1; y++)
            {
                if (Vector2.Distance((Vector3)tiles.WorldToCell(boom.location), new Vector3(x, y)) < boom.magnitude * 1.5f &&
                    (!(tiles.GetTile(new Vector3Int(x, y)) == null || tiles.GetTile(new Vector3Int(x, y)) == emptyTile)))
                {
                    tiles.SetTile(new Vector3Int(x, y), emptyTile);
                }
                else if (!(tiles.GetTile(new Vector3Int(x, y)) == null || tiles.GetTile(new Vector3Int(x, y)) == emptyTile))
                {
                    temp.Add(new Vector3Int(x, y));
                }
            }
        }

        foreach (Vector3Int t in temp)
            UpdateTile(t);
    }

    public TileBase CustomTile(bool t1, bool t2, bool t3, bool t4, bool t5, bool t6, byte map)
    { // 101010 would be t1 true t2 false 3 true false true false
        if (!t1 && !t2 && !t3 && !t4 && !t5 && !t6)
            return emptyTile;
        foreach (TileBase t in tileTypes)
        {
            if (((t.name[0] == "0"[0] && !t1) || (t.name[0] == "1"[0] && t1)) &&
                ((t.name[1] == "0"[0] && !t2) || (t.name[1] == "1"[0] && t2)) &&
                ((t.name[2] == "0"[0] && !t3) || (t.name[2] == "1"[0] && t3)) &&
                ((t.name[3] == "0"[0] && !t4) || (t.name[3] == "1"[0] && t4)) &&
                ((t.name[4] == "0"[0] && !t5) || (t.name[4] == "1"[0] && t5)) &&
                ((t.name[5] == "0"[0] && !t6) || (t.name[5] == "1"[0] && t6)))
                return t;
        }
        return emptyTile;
    }

    public void UpdateTile(Vector3Int position)
    {
        for (int i = 0; i < 6; i++)
        {
            if (position.y % 2 == 0)
            {
                bool t1 = (tiles.GetTile(position + hexagonEven[0]) != emptyTile) && (tiles.GetTile(position + hexagonEven[0]) != null);
                bool t2 = (tiles.GetTile(position + hexagonEven[1]) != emptyTile) && (tiles.GetTile(position + hexagonEven[1]) != null);
                bool t3 = (tiles.GetTile(position + hexagonEven[2]) != emptyTile) && (tiles.GetTile(position + hexagonEven[2]) != null);
                bool t4 = (tiles.GetTile(position + hexagonEven[3]) != emptyTile) && (tiles.GetTile(position + hexagonEven[3]) != null);
                bool t5 = (tiles.GetTile(position + hexagonEven[4]) != emptyTile) && (tiles.GetTile(position + hexagonEven[4]) != null);
                bool t6 = (tiles.GetTile(position + hexagonEven[5]) != emptyTile) && (tiles.GetTile(position + hexagonEven[5]) != null);
                tiles.SetTile(position, CustomTile(t1, t2, t3, t4, t5, t6, 0));
            }
            else
            {
                bool t1 = (tiles.GetTile(position + hexagonOdd[0]) != emptyTile) && (tiles.GetTile(position + hexagonOdd[0]) != null);
                bool t2 = (tiles.GetTile(position + hexagonOdd[1]) != emptyTile) && (tiles.GetTile(position + hexagonOdd[1]) != null);
                bool t3 = (tiles.GetTile(position + hexagonOdd[2]) != emptyTile) && (tiles.GetTile(position + hexagonOdd[2]) != null);
                bool t4 = (tiles.GetTile(position + hexagonOdd[3]) != emptyTile) && (tiles.GetTile(position + hexagonOdd[3]) != null);
                bool t5 = (tiles.GetTile(position + hexagonOdd[4]) != emptyTile) && (tiles.GetTile(position + hexagonOdd[4]) != null);
                bool t6 = (tiles.GetTile(position + hexagonOdd[5]) != emptyTile) && (tiles.GetTile(position + hexagonOdd[5]) != null);
                tiles.SetTile(position, CustomTile(t1, t2, t3, t4, t5, t6, 0));
            }
        }
    }

    public void SaveCurrentAsLevel(int index)
    {
        List<TileBase> tempTiles = new List<TileBase>();
        Vector2Int lowest = (Vector2Int)tiles.cellBounds.min;
        Vector2Int highest = (Vector2Int)tiles.cellBounds.max;
        List<Vector3Int> tempPositions = new List<Vector3Int>();

        for (int x = lowest.x; x < highest.x; x++)
        {
            for (int y = lowest.y; y < highest.y; y++)
            {
                tempTiles.Add(tiles.GetTile(new Vector3Int(x, y)));
                tempPositions.Add(new Vector3Int(x, y));
            }
        }

        levels[index] = new Level(tempTiles, lowest, highest, tempPositions);
    }

    public void LoadLevel(int index)
    {
        //tiles.DeleteCells(tiles.cellBounds.min, tiles.cellBounds.max);
        tiles.ClearAllTiles();

        for (int i = 0; i < levels[index].tiles.Count; i++)
            tiles.SetTile(levels[index].positions[i], levels[index].tiles[i]);

    }
}

[System.Serializable]
public class Level
{
    public List<TileBase> tiles = new List<TileBase>();
    public List<Vector3Int> positions = new List<Vector3Int>();
    public Vector2Int lowest;
    public Vector2Int highest;

    public Level (List<TileBase> _tiles, Vector2Int _lowest, Vector2Int _highest, List<Vector3Int> _positions)
    {
        tiles = _tiles;
        lowest = _lowest;
        highest = _highest;
        positions = _positions;
    }
}