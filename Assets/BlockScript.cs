using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BlockScript : MonoBehaviour
{
    public int value;
    public TextMeshProUGUI valueText;
    public float moveTime;
    public bool isMoving;
    public bool canMerge;
    public Vector3 finalPosition;
    public Vector3 moveVector;
    public GameObject otherBlock;

    // Start is called before the first frame update
    void Start()
    {
        changeValue(value);
        isMoving = false;
        canMerge = true;
        finalPosition = transform.position;
        otherBlock = null;
    }

    // This function calculates at which position every block should stop
    public void movePhase(Vector3 pos, Vector3 direction) {
        isMoving = true;
        GameMechanics.changeBlocksMoving(1);
        finalPosition = pos;
        moveVector = direction;
    }

    bool checkBounds() {
        if(transform.position.x > GameMechanics.x_size-1 || transform.position.x < 0 || transform.position.z > GameMechanics.z_size-1 || transform.position.z < 0)
            return false;
        return true;
    }

    void stopBlock() {
        if(isMoving) {
            isMoving = false;
            transform.position = new Vector3(Mathf.Round(transform.position.x), 0, Mathf.Round(transform.position.z));
            moveVector = Vector3.zero;
            finalPosition = transform.position;
            GameMechanics.changeBlocksMoving(-1);
        }
    }

    void FixedUpdate()
    {
        if(isMoving) {
            if(Vector3.Distance(transform.position, finalPosition) < .1f) {
                stopBlock();
            }

            transform.position += moveTime * Time.deltaTime * moveVector;
        }
    }

    public void changeValue(int value)
    {
        this.value = value;
        GetComponent<Renderer>().material = Resources.Load<Material>("BlockMaterials/Block_"+value.ToString());
        valueText.text = value.ToString();
    }

    public void changeValueTimesTwo()
    {
        this.value = 2*this.value;
        GetComponent<Renderer>().material = Resources.Load<Material>("BlockMaterials/Block_"+value.ToString());
        valueText.text = value.ToString();
    }
}
