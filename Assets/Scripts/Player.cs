using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public Chip chip;
    public int baseId;
    public int[] playerBase;
    public int direction; //1,-1
    public bool hasDaed; // AQEDAN VAGRDZELEB
    public int[] revivalSlots;
    public Player(Chip c, int dir, int[] revivalSlots, int[] playerBase)
    {
        this.revivalSlots = revivalSlots;
        chip = c;
        direction = dir;
        this.playerBase = playerBase;
        baseId = playerBase[0];
    }
}
