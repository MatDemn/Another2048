using UnityEngine;

public class Pair<T,R> {
    public T x;
    public R y;

    public Pair(T x, R y) {
        this.x = x;
        this.y = y;
    }
    /*
    public override string ToString()
    {
        return this.x.ToString() + ", " + this.y.ToString();
    }*/
}
