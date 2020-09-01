using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager INSTANCE;

    public Player[] player;
    public List<Transform> slotlist;
    public GameObject whiteChipPrefab, blackChipPrefab;
    public Slot deadWhites, deadBlacks;
    public int[] whiteRevivalSlots, blackRevivalSlots;
    public int[] rolledDice;
    public bool isDiceOnePlayed, isDiceTwoPlayed;
    public bool canPlay;
    [HideInInspector] public Chip whiteChip, blackChip;
    public bool chipIsClicked;

    UIController uI;
    List<Slot> slots;
    bool isPair;
    int moveCounter;
    [SerializeField] Slot from, to;
    Player currentPlayer;
    bool turnCounter;
    bool isAllInBase;


    private void Awake()
    {
        INSTANCE = this;
        slots = new List<Slot>(24);
        for (int i = 0; i < 24; i++) slots.Add(null);
        player = new Player[2];
        player[0] = new Player(whiteChip, -1, whiteRevivalSlots, blackRevivalSlots);
        player[1] = new Player(blackChip, 1, blackRevivalSlots, whiteRevivalSlots);
        whiteChip = whiteChipPrefab.GetComponent<Chip>();
        blackChip = blackChipPrefab.GetComponent<Chip>();
        currentPlayer = player[0];
        rolledDice = new int[2];
    }

    void Start()
    {
        uI = UIController.INSTANCE;
        turnCounter = true;
        InitBoard();
    }

    void InitBoard()
    {
        for (int i = 0; i < 24; i++)
        {
            Slot s = slotlist[i].gameObject.AddComponent<Slot>();
            AmountTypeAndPlayer aat = new AmountTypeAndPlayer();
            s.id = i;
            s.slotPos = slotlist[i].transform;
            aat = FirstArrangement(i);
            if (aat != null)
            {
                s.DropChips(aat);
            }
            slots[i] = s;
        }
    }

    public void FullTurn()
    {
        if (turnCounter)
        {
            uI.turnTxt.text = "white";
            currentPlayer = player[0];
            uI.chipIsClickedTxt.text = "chip isn't clicked";
            from = null;
            to = null;
            if (currentPlayer.hasDaed)
            {
                from = deadWhites;
                uI.chipIsClickedTxt.text = "chip is clicked";
            }
        }
        else
        {
            uI.turnTxt.text = "black";
            currentPlayer = player[1];
            to = null;
            from = null;
            uI.chipIsClickedTxt.text = "chip isn't clicked";
            if (currentPlayer.hasDaed)
            {
                from = deadBlacks;
                uI.chipIsClickedTxt.text = "chip is clicked";
            }
        }
        isAllInBase = IsAllInBase();

    }

    public int[] DiceRoll()
    {
        rolledDice[0] = Random.Range(1, 6);
        rolledDice[1] = Random.Range(1, 6);
        return rolledDice;
    }

    void KillChip(Slot deadsSlot, Slot from)
    {
        player.Single(x => x != currentPlayer).hasDaed = true;
        from.chipPos = from.id < 12 ? from.chipPos - from.deltaPos : from.chipPos + from.deltaPos;
        DeliverTo(deadsSlot, from);
    }

    bool IsOnRolled()
    {
        if (!isDiceOnePlayed || !isDiceTwoPlayed)
        {
            // if (rolledDice[0] == rolledDice[1])
            // {
            //     int[] nums = { 1, 2, 3, 4 };
            //     var count = 0;
            //     bool b = false;
            //     foreach (var item in nums)
            //     {
            //         count++;
            //         // var n = (int)(from?.id + rolledDice[0] * item * currentPlayer.direction);
            //         // if (slots[n].chipsStack.Count > 0)
            //         //     if (slots[n].chipsStack.Peek().player != currentPlayer &&
            //         //         slots[n].chipsStack.Count == 1)
            //         //     {
            //         //         KillChip(deadBlacks, slots[n]);
            //         //     }
            //         if (to?.id == from?.id + rolledDice[0] * item * currentPlayer.direction)
            //         {

            //             moveCounter += count;
            //             if (moveCounter <= 4) b = true;
            //             else moveCounter -= count;
            //         }
            //     }
            //     nums = nums.Take(nums.Count() - count).ToArray();
            //     if (moveCounter >= 4)
            //     {
            //         isDiceOnePlayed = true;
            //         isDiceTwoPlayed = true;
            //         moveCounter = 0;
            //         isPair = false;
            //     }
            //     return b;
            // }
            if (rolledDice[0] == rolledDice[1])
            {
                bool b = false;
                if (moveCounter <= 4 && to?.id == from?.id + rolledDice[0] * currentPlayer.direction)
                {
                    moveCounter++;
                    if (moveCounter >= 4)
                    {
                        isDiceOnePlayed = true;
                        isDiceTwoPlayed = true;
                        moveCounter = 0;
                    }
                    b = true;
                }
                return b;
            }
            else if (to?.id == from?.id + rolledDice[0] * currentPlayer.direction && !isDiceOnePlayed)
            {
                isDiceOnePlayed = true;
                return true;
            }
            else if (to?.id == from?.id + rolledDice[1] * currentPlayer.direction && !isDiceTwoPlayed)
            {
                isDiceTwoPlayed = true;
                return true;
            }
            // else if (!isDiceOnePlayed && !isDiceTwoPlayed)
            // {
            //     if (to?.id == from?.id + (rolledDice[0] + rolledDice[1]) * currentPlayer.direction)
            //     {
            //         var n = (int)(from.id + rolledDice[0] * currentPlayer.direction);
            //         if (slots[n].chipsStack.Peek().player != currentPlayer &&
            //                 slots[n].chipsStack.Count == 1)
            //         {
            //             if (currentPlayer == player[0]) KillChip(deadBlacks, to);
            //             else KillChip(deadWhites, to);
            //         }
            //         isDiceOnePlayed = true;
            //         isDiceTwoPlayed = true;
            //         return true;
            //     }
            //     else return false;
            // }
            else return false;
        }
        else return false;
    }

    void DeliverTo(Slot to, Slot form)
    {
        uI.chipIsClickedTxt.text = "chip isn't clicked";
        from.chipPos = from.id < 12 ? from.chipPos - from.deltaPos : from.chipPos + from.deltaPos;
        var c = form?.chipsStack.Pop();
        to.chipsStack.Push(c);
        chipIsClicked = false;
        to.PlaceChip(c);
        foreach (var item in slots)
        {
        }
    }

    void ExtraUpdate()
    {
        if (currentPlayer.direction == 1 && deadWhites.chipsStack.Count() == 0)
        {
            currentPlayer.hasDaed = false;
        }
        else if (currentPlayer.direction == -1 && deadBlacks.chipsStack.Count() == 0)
        {
            currentPlayer.hasDaed = false;
        }
        turnCounter = !turnCounter;
        isDiceOnePlayed = false;
        isDiceTwoPlayed = false;
        uI.UpdateRollPanel(true);
    }

    private bool IsAllInBase()
    {
        int n = currentPlayer.baseId;
        int min, max;
        bool b = false; ;
        if (currentPlayer.direction == -1)
        {
            min = 6;
            max = 23;
        }
        else
        {
            min = 0;
            max = 17;
        }

        for (int i = min; i <= max; i++)
        {
            if (slots[i].chipsStack.Count > 0)
                if (slots[i].chipsStack.Peek() == currentPlayer.chip)
                {
                    b = false;
                }
                else b = true;
        }
        return b;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canPlay)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit != false)
            {
                var clickedSlot = hit.collider.GetComponent<Slot>();
                if (!currentPlayer.hasDaed)
                {

                    if (hit.collider.tag == "Slot")
                    {
                        if (clickedSlot.chipsStack.Count > 0 && !chipIsClicked && currentPlayer == clickedSlot.chipsStack?.Peek().player)
                        {
                            uI.chipIsClickedTxt.text = "chip is clicked";
                            from = clickedSlot;
                            chipIsClicked = true;
                            if (isAllInBase)
                            {
                                // var tempDiceOne = rolledDice[0];
                                // var tempDiceTwo = rolledDice[1];
                                // List<Slot> asd = new List<Slot>();
                                // if (currentPlayer.direction == -1)
                                //     asd = slots.Where(s => s.id > from.id && s.id < 6).ToList();
                                // if (currentPlayer.direction == 1)
                                //     asd = slots.Where(s => s.id > 17 && s.id < from.id).ToList();


                                // if (!asd.Any(s => s.chipsStack.Count > 0))
                                // {
                                //     tempDiceOne -= asd.Count;
                                //     tempDiceTwo -=asd.Count;
                                // }
                                // var emptyInBase = slots.Where(slot => slot.chipsStack.Count == 0).ToList();
                                
                                // if (Mathf.Abs(currentPlayer.baseId - currentPlayer.direction - rolledDice[0]) == from.id && !isDiceOnePlayed)
                                if (Mathf.Abs(currentPlayer.baseId + currentPlayer.direction - from.id) == rolledDice[0] && !isDiceOnePlayed)
                                {
                                    isDiceOnePlayed = true;
                                    uI.chipOutBtn.gameObject.SetActive(true);
                                    chipIsClicked = true;
                                    canPlay = false;
                                }
                                // else if (Mathf.Abs(currentPlayer.baseId - currentPlayer.direction - rolledDice[1]) == from.id && !isDiceTwoPlayed)
                                else if (Mathf.Abs(currentPlayer.baseId + currentPlayer.direction - from.id) == rolledDice[1] && !isDiceTwoPlayed)
                                {
                                    isDiceTwoPlayed = true;
                                    uI.chipOutBtn.gameObject.SetActive(true);
                                    chipIsClicked = true;
                                    canPlay = false;
                                }
                                if (isDiceOnePlayed && isDiceTwoPlayed)
                                {
                                    ExtraUpdate();
                                }
                            }
                        }
                        else if (clickedSlot.chipsStack.Count == 0 || currentPlayer == clickedSlot.chipsStack?.Peek().player)
                        {
                            to = hit.transform.GetComponent<Slot>();

                            if (IsOnRolled())
                            {
                                DeliverTo(to, from);
                                if (isDiceOnePlayed && isDiceTwoPlayed)
                                {
                                    ExtraUpdate();
                                }
                            }
                            else
                            {
                                chipIsClicked = false;
                                from = null;
                                uI.chipIsClickedTxt.text = "chip isn't clicked";
                                to = null;
                            }
                        }
                        else if (clickedSlot.chipsStack.Count == 1)
                        {
                            to = hit.transform.GetComponent<Slot>();
                            if (IsOnRolled())
                            {
                                if (currentPlayer == player[0]) KillChip(deadBlacks, to);
                                else KillChip(deadWhites, to);
                                // var pl = player.Except(canDoMove);
                                DeliverTo(to, from);
                                if (isDiceOnePlayed && isDiceTwoPlayed)
                                {
                                    ExtraUpdate();
                                }
                            }
                        }
                        else
                        {
                            chipIsClicked = false;
                        }
                    }
                }
                else
                {
                    if (hit.collider.tag == "Slot")
                    {
                        to = hit.transform.GetComponent<Slot>();
                        for (int i = 0; i < 6; i++)
                        {
                            if (to.id == currentPlayer.revivalSlots[i])
                            {
                                if (clickedSlot.chipsStack.Count == 0 || currentPlayer == clickedSlot.chipsStack?.Peek().player)
                                {
                                    if (IsOnRolled())
                                    {
                                        DeliverTo(to, from);

                                        currentPlayer.hasDaed = false;
                                        if (isDiceOnePlayed && isDiceTwoPlayed)
                                        {
                                            ExtraUpdate();
                                        }
                                    }
                                    else
                                    {
                                        chipIsClicked = false;
                                        uI.chipIsClickedTxt.text = "chip isn't clicked";
                                        from = null;
                                        to = null;
                                    }
                                }
                                else if (clickedSlot.chipsStack.Count == 1)
                                {
                                    if (IsOnRolled())
                                    {
                                        KillChip(deadBlacks, to);
                                        DeliverTo(to, from);
                                        currentPlayer.hasDaed = false;
                                        if (isDiceOnePlayed && isDiceTwoPlayed)
                                        {
                                            ExtraUpdate();
                                        }
                                    }
                                }
                                else
                                {
                                    chipIsClicked = false;
                                }
                                break;
                            }
                        }
                    }
                }
                
            }
        }
    }
    public void Score()
    {

        from.chipsStack.Peek().gameObject.SetActive(false);
        from.chipsStack.Pop();
        from.chipPos -= from.deltaPos * -currentPlayer.direction;
        uI.chipIsClickedTxt.text = "chip isn't clicked";
        canPlay = true;
        chipIsClicked = false;
    }

    AmountTypeAndPlayer FirstArrangement(int i)
    {
        AmountTypeAndPlayer aat = new AmountTypeAndPlayer();
        if (i == 0)
        {
            aat.amount = 2;
            aat.chipPrefab = blackChipPrefab;
        }
        else if (i == 5)
        {
            aat.amount = 5;
            aat.chipPrefab = whiteChipPrefab;
        }
        else if (i == 7)
        {
            aat.amount = 3;
            aat.chipPrefab = whiteChipPrefab;
        }
        else if (i == 11)
        {
            aat.amount = 5;
            aat.chipPrefab = blackChipPrefab;
        }
        else if (i == 12)
        {
            aat.amount = 5;
            aat.chipPrefab = whiteChipPrefab;
        }
        else if (i == 16)
        {
            aat.amount = 3;
            aat.chipPrefab = blackChipPrefab;
        }
        else if (i == 18)
        {
            aat.amount = 5;
            aat.chipPrefab = blackChipPrefab;
        }
        else if (i == 23)
        {
            aat.amount = 2;
            aat.chipPrefab = whiteChipPrefab;
        }
        else return null;
        if (aat.chipPrefab == whiteChipPrefab) aat.player = player[0];
        else if (aat.chipPrefab == blackChipPrefab) aat.player = player[1];
        return aat;
    }
}

public class AmountTypeAndPlayer
{
    public int amount;
    public GameObject chipPrefab;
    public Player player;

}
