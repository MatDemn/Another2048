using UnityEngine;

public class Coords {
    public int x;
    public int y;

    public Coords(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return base.ToString() + this.x.ToString() + ", " + this.y.ToString();
    }
}
