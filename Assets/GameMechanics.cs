using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Start is called before the first frame update
    void Start()
    {   
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
        /*
        for(int i = 0; i<x_size; i++) {
            for(int j = 0; jz_size; j++) {
                if(i!=2 || j!=2)
                    continue;
                GameObject go = Instantiate(valueBlockPrefab, new Vector3(i,0,j), transform.rotation);
                go.gameObject.name = $"B{i}_{j}";
                simulationTable[i, j,0] = go;
            }
        }*/

        Vector3[] cords = new Vector3[] {new Vector3(0,0,0), new Vector3(x_size-1,0,z_size-1) /*, new Vector3(1,0,2), new Vector3(3,0,1)*/};

        for(int i = 0; i<cords.Length; i++) {
            GameObject go = Instantiate(valueBlockPrefab, cords[i], transform.rotation);
            go.gameObject.name = $"B{i}";
            simulationTable[(int)cords[i].x, (int)cords[i].z,0] = go;
        }

        //printSim();
    }

    // Update is called once per frame
    void Update()
    {
        if(canMove) {
            float x_move_temp = Input.GetAxis("Horizontal");
            float z_move_temp = Input.GetAxis("Vertical");

            MoveBlocks(x_move_temp, z_move_temp);
        }
    }
    public void SimulateMove(float x_input, float z_input) {
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
                                break;
                            }
                        }
                        else {
                            simulationTable[i,z,0] = simulationTable[i-1,z,0];
                            simulationTable[i-1,z,0] = null;
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
                                break;
                            }
                        }
                        else {
                            simulationTable[i,z,0] = simulationTable[i+1,z,0];
                            simulationTable[i+1,z,0] = null;
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
                                break;
                            }
                        }
                        else {
                            simulationTable[x,i,0] = simulationTable[x,i-1,0];
                            simulationTable[x,i-1,0] = null;
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
                                break;
                            }
                        }
                        else {
                            simulationTable[x,i,0] = simulationTable[x,i+1,0];
                            simulationTable[x,i+1,0] = null;
                        }
                        
                    }
                }
            }
        }
        return;
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
    public void MoveBlocks(float x_input, float z_input) {
        // Right
        if(x_input > 0) {
            canMove = false;
            SimulateMove(x_input, z_input);
            for(int z = 0; z<z_size; z++) {
                for(int x = x_size-1; x>=0; x--) {
                    if(simulationTable[x,z,0] != null) {
                        simulationTable[x,z,0].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.right);
                        if(simulationTable[x,z,1] != null) {
                            simulationTable[x,z,1].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.right);
                        }
                    }
                }
            }
        }
        // Left
        else if(x_input < 0) {
            canMove = false;
            SimulateMove(x_input, z_input);
            for(int z = 0; z<z_size; z++) {
                for(int x = 0; x<x_size; x++) {
                    if(simulationTable[x,z,0] != null) {
                        simulationTable[x,z,0].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.left);
                        if(simulationTable[x,z,1] != null) {
                            simulationTable[x,z,1].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.left);
                        }
                    }
                }
            }
        }
        // Up
        else if(z_input > 0) {
            canMove = false;
            SimulateMove(x_input, z_input);
            for(int x = 0; x<x_size; x++) {
                for(int z = 0; z<z_size; z++) {
                    if(simulationTable[x,z,0] != null) {
                        simulationTable[x,z,0].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.forward);
                        if(simulationTable[x,z,1] != null) {
                            simulationTable[x,z,1].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.forward);
                        }
                    }
                }
            }
        }
        // Down
        else if(z_input < 0) {
            canMove = false;
            SimulateMove(x_input, z_input);
            for(int x = 0; x<x_size; x++) {
                for(int z = z_size-1; z>=0; z--) {
                    if(simulationTable[x,z,0] != null) {
                        simulationTable[x,z,0].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.back);
                        if(simulationTable[x,z,1] != null) {
                            simulationTable[x,z,1].GetComponent<BlockScript>().movePhase(new Vector3(x, 0, z), Vector3.back);
                        }
                    }
                }
            }
        }
    }

    public static void MergeBlocks() {
        foreach(Pair<GameObject, GameObject> pair in mergePairs) {
            pair.x.GetComponent<BlockScript>().changeValueTimesTwo();
            Destroy(pair.y);
            //simulationTable[(int)pair.x.transform.position.x, (int)pair.x.transform.position.z,1] = null;
        }
        mergePairs.Clear();
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
        return result[Random.Range(0, index-1)];

    }

    public static void randomBlock() {
        Coords freeCord = listFree();
        generateBlock(freeCord.x, freeCord.y, 1);
    }

    public static void changeBlocksMoving(int val) {
        blocksMoving += val;
        if(blocksMoving == 0) {
            MergeBlocks();
            randomBlock();
            printSim();
            canMove = true;
        }
    }
}
