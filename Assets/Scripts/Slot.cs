using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public int id;
    public Transform slotPos;
    public Vector3 deltaPos, chipPos;
    [SerializeField] public Stack<Chip> chipsStack;

    GameManager gm;

    private void Awake()
    {
        deltaPos = new Vector2(0, .8f);
        chipsStack = new Stack<Chip>();
        chipPos = this.transform.position;
        gm = GameManager.INSTANCE;
    }


    public void DropChips(AmountTypeAndPlayer atap)
    {
        for (int i = 0; i < atap.amount; i++)
        {
            var chipObject = Instantiate(atap.chipPrefab, this.transform);

            Chip chipScript = chipObject.GetComponent<Chip>();
            chipScript.player = atap.player;
            chipsStack.Push(chipScript);
            PlaceChip(chipScript);
        }
    }

    public void PlaceChip(Chip c)
    {
        c.transform.position = chipPos;
        chipPos = id < 12 ? chipPos + deltaPos : chipPos - deltaPos;

    }

}
