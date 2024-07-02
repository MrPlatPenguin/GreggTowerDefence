using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GridObject
{
    protected Grid<GridObject> grid;
    protected int x;
    protected int y;

    public GridObject(Grid<GridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }
    public abstract void OnClick();

    public abstract override string ToString();
}
