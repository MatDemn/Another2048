using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameMechanics : MonoBehaviour
{

    public static int x_size;

    public static int z_size;

    public static GameObject valueBlockPrefab;

    public static bool canMove;

    public static int blocksMoving;

    public static List<Pair<GameObject, GameObject>> mergePairs;

    public static GameObject[,,] simulationTable;

    public GameObject terrainObj;

    public bool releasedButton;

    public float lastTimeMoved;

    float moveDelay;

    public static Action<int> onScoreUpdate;

    [SerializeField]
    Camera camRef;

    static Transform gameOverObj;

    // Start is called before the first frame update
    void Start()
    {
        lastTimeMoved = -1f;
        moveDelay = .5f;
        mergePairs = new List<Pair<GameObject, GameObject>>();
        blocksMoving = 0;
        x_size = 4;
        z_size = 4;
        canMove = true;
        releasedButton = true;
        valueBlockPrefab = Resources.Load<GameObject>("ValueBlock");
        simulationTable = new GameObject[x_size,z_size,2];
        terrainObj.GetComponent<Renderer>().material.mainTextureScale = new Vector2(x_size, z_size);
        terrainObj.transform.localScale = new Vector3((float)x_size/10, 1, (float)z_size/10);
        terrainObj.transform.position = new Vector3((x_size-1)*0.5f, 0, (z_size-1)*0.5f);

        cameraPositionSet();

        
        Vector3[] cords = new Vector3[2];

        int index = 0;
        while(true)
        {
            int x, z;
            x = UnityEngine.Random.Range(0, x_size - 1);
            z = UnityEngine.Random.Range(0, z_size - 1);
            if(index == 0)
            {
                cords[0] = new Vector3(x, 0, z);
                index++;
            }
            else
            {
                // If other cords than previously added
                if(cords[0].x != x || cords[0].z != z)
                {
                    cords[1] = new Vector3(x, 0, z);
                    break;
                }
            }
        }

        for (int i = 0; i < cords.Length; i++)
        {
            GameObject go = Instantiate(valueBlockPrefab, cords[i], transform.rotation);
            go.gameObject.name = $"B{cords[i].x}, {cords[i].z}";
            simulationTable[(int)cords[i].x, (int)cords[i].z, 0] = go;
        }

        /*
        Vector3[] cords = new Vector3[16];

        for(int i = 0; i<cords.Length; i++)
        {
            cords[i] = new Vector3(i / 4, 0, i % 4);
        }

        for (int i = 0; i<cords.Length-1; i++) {
            GameObject go = Instantiate(valueBlockPrefab, cords[i], transform.rotation);
            go.gameObject.name = $"B{cords[i].x}, {cords[i].z}";
            simulationTable[(int)cords[i].x, (int)cords[i].z,0] = go;
            go.GetComponent<BlockScript>().changeValue(1 << (i%12));
        }*/

        gameOverObj = GameObject.Find("UI").transform.GetChild(1);

        if(gameOverObj == null)
        {
            Debug.LogError("No gameOverObj found!");
        }

        //printSim();
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove && (Time.time > lastTimeMoved + moveDelay) ) {
            float x_move_temp = Input.GetAxis("Horizontal");
            float z_move_temp = Input.GetAxis("Vertical");

            if(MoveBlocks(x_move_temp, z_move_temp))
            {
                AudioManager.instance.Play("Drag");
                lastTimeMoved = Time.time;
            }
        }
        
    }
    public bool SimulateMove(float x_input, float z_input) {
        bool anyBlockMoved = false;
        // Right
        if(x_input > 0) {
            for(int z = 0; z<z_size; z++) {
                for(int x = x_size-2; x>=0; x--) {
                    if(simulationTable[x,z,0] == null) {
                        continue;
                    }
                    int checkVal = simulationTable[x,z,0].GetComponent<BlockScript>().value;
                    for(int i = x+1; i < x_size; i++) {
                        // Looking for good place to place block
                        if(simulationTable[i,z,0] != null) {
                            if(simulationTable[i,z,0].GetComponent<BlockScript>().value != checkVal) {
                                break;
                            }
                            // Check if second place is ocuppied
                            if(simulationTable[i,z,1] != null) {
                                break;
                            }
                            else {
                                simulationTable[i,z,1] = simulationTable[i-1,z,0];
                                simulationTable[i-1, z,0] = null;
                                mergePairs.Add(new Pair<GameObject, GameObject>(simulationTable[i,z,0], simulationTable[i,z,1]));
                                anyBlockMoved = true;
                                break;
                            }
                        }
                        else {
                            simulationTable[i,z,0] = simulationTable[i-1,z,0];
                            simulationTable[i-1,z,0] = null;
                            anyBlockMoved = true;
                        }
                        
                    }
                }
            }
        }
        // Left
        else if(x_input < 0) {
            for(int z = 0; z<z_size; z++) {
                for(int x = 1; x<x_size; x++) {
                    if(simulationTable[x,z,0] == null) {
                        continue;
                    }
                    int checkVal = simulationTable[x,z,0].GetComponent<BlockScript>().value;
                    for(int i = x-1; i >= 0; i--) {
                        // Looking for good place to place block
                        if(simulationTable[i,z,0] != null) {
                            if(simulationTable[i,z,0].GetComponent<BlockScript>().value != checkVal) {
                                break;
                            }
                            // Check if second place is ocuppied
                            if(simulationTable[i,z,1] != null) {
                                break;
                            }
                            else {
                                simulationTable[i,z,1] = simulationTable[i+1,z,0];
                                simulationTable[i+1, z,0] = null;
                                mergePairs.Add(new Pair<GameObject, GameObject>(simulationTable[i,z,0], simulationTable[i,z,1]));
                                anyBlockMoved = true;
                                break;
                            }
                        }
                        else {
                            simulationTable[i,z,0] = simulationTable[i+1,z,0];
                            simulationTable[i+1,z,0] = null;
                            anyBlockMoved = true;
                        }
                    }
                }
            }
        }
        // Up
        else if(z_input > 0) {
            for(int z = z_size-2; z >= 0; z--) {
                for(int x = 0; x<x_size; x++) {
                    if(simulationTable[x,z,0] == null) {
                        continue;
                    }
                    int checkVal = simulationTable[x,z,0].GetComponent<BlockScript>().value;
                    for(int i = z+1; i < z_size; i++) {
                        // Looking for good place to place block
                        if(simulationTable[x,i,0] != null) {
                            if(simulationTable[x,i,0].GetComponent<BlockScript>().value != checkVal) {
                                break;
                            }
                            // Check if second place is ocuppied
                            if(simulationTable[x,i,1] != null) {
                                break;
                            }
                            else {
                                simulationTable[x,i,1] = simulationTable[x,i-1,0];
                                simulationTable[x,i-1,0] = null;
                                mergePairs.Add(new Pair<GameObject, GameObject>(simulationTable[x,i,0], simulationTable[x,i,1]));
                                anyBlockMoved = true;
                                break;
                            }
                        }
                        else {
                            simulationTable[x,i,0] = simulationTable[x,i-1,0];
                            simulationTable[x,i-1,0] = null;
                            anyBlockMoved = true;
                        }
                        
                    }
                }
            }
        }
        // Down
        else if(z_input < 0) {
            for(int z = 1; z < z_size; z++) {
                for(int x = 0; x<x_size; x++) {
                    if(simulationTable[x,z,0] == null) {
                        continue;
                    }
                    int checkVal = simulationTable[x,z,0].GetComponent<BlockScript>().value;
                    for(int i = z-1; i >= 0; i--) {
                        // Looking for good place to place block
                        if(simulationTable[x,i,0] != null) {
                            if(simulationTable[x,i,0].GetComponent<BlockScript>().value != checkVal) {
                                break;
                            }
                            // Check if second place is ocuppied
                            if(simulationTable[x,i,1] != null) {
                                break;
                            }
                            else {
                                simulationTable[x,i,1] = simulationTable[x,i+1,0];
                                simulationTable[x,i+1,0] = null;
                                mergePairs.Add(new Pair<GameObject, GameObject>(simulationTable[x,i,0], simulationTable[x,i,1]));
                                anyBlockMoved = true;
                                break;
                            }
                        }
                        else {
                            simulationTable[x,i,0] = simulationTable[x,i+1,0];
                            simulationTable[x,i+1,0] = null;
                            anyBlockMoved = true;
                        }
                        
                    }
                }
            }
        }
        return anyBlockMoved;
    }

    static void printSim() {
        Debug.Log("------------");
        for(int z=z_size-1; z>=0; z--) {
            string res = "";
            for(int x=0; x<x_size; x++) {
                if(simulationTable[x,z,0] != null) {
                    if(simulationTable[x,z,1] != null)
                        res += $"[{simulationTable[x,z,0].transform.position.x}|{simulationTable[x,z,0].transform.position.z}] ";
                    else
                        res += $"({simulationTable[x,z,0].transform.position.x}|{simulationTable[x,z,0].transform.position.z}) ";
                }
                else {
                    res += "(_|_) ";
                }
            }
            Debug.Log(res);
        }
    }

    static bool checkIfMovePossible()
    {
        // Right move
        for (int z = 0; z < z_size; z++)
        {
            for (int x = x_size - 2; x >= 0; x--)
            {
                // If there are any free place on grid, move is always possible
                if (simulationTable[x, z, 0] == null)
                {
                    return true;
                }
                // Otherwise, check if this block (x,z) can move
                int checkVal = simulationTable[x, z, 0].GetComponent<BlockScript>().value;
                for (int i = x + 1; i < x_size; i++)
                {
                    // If there are some block next to it
                    if (simulationTable[i, z, 0] != null)
                    {
                        // Check if it hase the same value. If not, this (x,z) block can't move there,
                        // break from this loop and check another block
                        if (simulationTable[i, z, 0].GetComponent<BlockScript>().value != checkVal)
                        {
                            break;
                        }
                        // If values are the same, you can move by merge
                        return true;
                    }
                    // If there are no block nex to it, move is always possible
                    else
                    {
                        return true;
                    }
                }
            }
        }
        // Down
        for (int z = 1; z < z_size; z++)
        {
            for (int x = 0; x < x_size; x++)
            {
                // If there are any free place on grid, move is always possible
                if (simulationTable[x, z, 0] == null)
                {
                    return true;
                }
                // Otherwise, check if this block (x,z) can move
                int checkVal = simulationTable[x, z, 0].GetComponent<BlockScript>().value;
                for (int i = z - 1; i >= 0; i--)
                {
                    // If there are some block next to it
                    if (simulationTable[x, i, 0] != null)
                    {
                        // Check if it hase the same value. If not, this (x,z) block can't move there,
                        // break from this loop and check another block
                        if (simulationTable[x, i, 0].GetComponent<BlockScript>().value != checkVal)
                        {
                            break;
                        }
                        // If values are the same, you can move by merge
                        return true;
                    }
                    // If there are no block nex to it, move is always possible
                    else
                    {
                        return true;
                    }
                }
            }
        }
        // Left
        for (int z = 0; z < z_size; z++)
        {
            for (int x = 1; x < x_size; x++)
            {
                // If there are any free place on grid, move is always possible
                if (simulationTable[x, z, 0] == null)
                {
                    return true;
                }
                // Otherwise, check if this block (x,z) can move
                int checkVal = simulationTable[x, z, 0].GetComponent<BlockScript>().value;
                for (int i = x - 1; i >= 0; i--)
                {
                    // If there are some block next to it
                    if (simulationTable[i, z, 0] != null)
                    {
                        // Check if it hase the same value. If not, this (x,z) block can't move there,
                        // break from this loop and check another block
                        if (simulationTable[i, z, 0].GetComponent<BlockScript>().value != checkVal)
                        {
                            break;
                        }
                        // If values are the same, you can move by merge
                        return true;
                    }
                    // If there are no block nex to it, move is always possible
                    else
                    {
                        return true;
                    }
                }
            }
        }

        // Top
        for (int z = z_size - 2; z >= 0; z--)
        {
            for (int x = 0; x < x_size; x++)
            {
                // If there are any free place on grid, move is always possible
                if (simulationTable[x, z, 0] == null)
                {
                    return true;
                }
                // Otherwise, check if this block (x,z) can move
                int checkVal = simulationTable[x, z, 0].GetComponent<BlockScript>().value;
                for (int i = z + 1; i < z_size; i++)
                {
                    // If there are some block next to it
                    if (simulationTable[x, i, 0] != null)
                    {
                        // Check if it hase the same value. If not, this (x,z) block can't move there,
                        // break from this loop and check another block
                        if (simulationTable[x, i, 0].GetComponent<BlockScript>().value != checkVal)
                        {
                            break;
                        }
                        // If values are the same, you can move by merge
                        return true;
                    }
                    // If there are no block nex to it, move is always possible
                    else
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool MoveBlocks(float x_input, float z_input) {
        bool anyBlockMoved = false;
        // Right
        if(x_input > 0) {
            canMove = false;
            if(SimulateMove(x_input, z_input)) 
            {
                anyBlockMoved = true;
                for (int z = 0; z < z_size; z++)
                {
                    for (int x = x_size - 1; x >= 0; x--)
                    {
                        if (simulationTable[x, z, 0] != null)
                        {
                            simulationTable[x, z, 0].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.right);
                            if (simulationTable[x, z, 1] != null)
                            {
                                simulationTable[x, z, 1].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.right);
                            }
                        }
                    }
                }
            }
            
        }
        // Left
        else if(x_input < 0) {
            canMove = false;
            if(SimulateMove(x_input, z_input))
            {
                anyBlockMoved = true;
                for (int z = 0; z < z_size; z++)
                {
                    for (int x = 0; x < x_size; x++)
                    {
                        if (simulationTable[x, z, 0] != null)
                        {
                            simulationTable[x, z, 0].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.left);
                            if (simulationTable[x, z, 1] != null)
                            {
                                simulationTable[x, z, 1].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.left);
                            }
                        }
                    }
                }
            }
            
        }
        // Up
        else if(z_input > 0) {
            canMove = false;
            if(SimulateMove(x_input, z_input))
            {
                anyBlockMoved = true;
                for (int x = 0; x < x_size; x++)
                {
                    for (int z = 0; z < z_size; z++)
                    {
                        if (simulationTable[x, z, 0] != null)
                        {
                            simulationTable[x, z, 0].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.forward);
                            if (simulationTable[x, z, 1] != null)
                            {
                                simulationTable[x, z, 1].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.forward);
                            }
                        }
                    }
                }
            }
            
        }
        // Down
        else if(z_input < 0) {
            canMove = false;
            if(SimulateMove(x_input, z_input))
            {
                anyBlockMoved = true;
                for (int x = 0; x < x_size; x++)
                {
                    for (int z = z_size - 1; z >= 0; z--)
                    {
                        if (simulationTable[x, z, 0] != null)
                        {
                            simulationTable[x, z, 0].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.back);
                            if (simulationTable[x, z, 1] != null)
                            {
                                simulationTable[x, z, 1].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.back);
                            }
                        }
                    }
                }
            }
            
        }
        if(!anyBlockMoved)
            canMove = true;
        return anyBlockMoved;
    }

    public static void MergeBlocks() {
        int newScore = 0;
        foreach(Pair<GameObject, GameObject> pair in mergePairs) {
            BlockScript firstBlockScript = pair.x.GetComponent<BlockScript>();
            firstBlockScript.changeValueTimesTwo();
            Destroy(pair.y);
            newScore += firstBlockScript.value;
        }
        if(newScore != 0)
        {
            AudioManager.instance.Play("Merge");
        }
        mergePairs.Clear();
        onScoreUpdate?.Invoke(newScore);
    }

    public static void generateBlock(float x, float z, int value) {
        GameObject go = Instantiate(valueBlockPrefab, new Vector3(x,0,z), Quaternion.identity);
        go.gameObject.name = $"B{go.transform.position.x}_{go.transform.position.z}";
        simulationTable[(int)x, (int)z, 0] = go;
        go.GetComponent<BlockScript>().changeValue(1);
    }

    public static Coords listFree() {
        Coords[] result = new Coords[24];

        int index = -1;
        for(int z = 0; z < z_size; z++) {
            for(int x = 0; x < x_size; x++) {
                result[index+1] = null;
                Collider[] res = Physics.OverlapSphere(new Vector3(x, 0, z), .4f, LayerMask.GetMask("ValueBlock"), QueryTriggerInteraction.Collide);
                if(res.Length == 0) {
                    index++;
                    result[index] = new Coords(x, z);
                }
            }
        }
        if(index == -1)
            return new Coords(-1,-1);
        return result[UnityEngine.Random.Range(0, index-1)];

    }

    public static void randomBlock() {
        Coords freeCord = listFree();
        // Generate if there was free coord
        if(freeCord.x != -1)
        {
            generateBlock(freeCord.x, freeCord.y, 1);
        }
        // Otherwise start gameOver phase
        else
        {
            runGameOver();
        }

        // Check if move if possible after generating block
        if(!checkIfMovePossible())
        {
            runGameOver();
        }
        
    }

    public static void changeBlocksMoving(int val) {
        blocksMoving += val;
        if(blocksMoving == 0) {
            MergeBlocks();
            randomBlock();
            canMove = true;
        }
    }

    void cameraPositionSet()
    {
        camRef.transform.position = new Vector3(terrainObj.transform.position.x, camRef.transform.position.y, terrainObj.transform.position.z);
    }

    public static void runGameOver()
    {
        canMove = false;
        gameOverObj.gameObject.SetActive(true);
    }

    public static void restartGame()
    {
        canMove = true;
        gameOverObj.gameObject.SetActive(false);
        SceneManager.LoadScene("SampleScene");
        ScoreScript.instance.ClearScore();
    }

    public static void endGame()
    {
        Application.Quit();
    }
}
