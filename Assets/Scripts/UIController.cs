using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController INSTANCE;
    public Text turnTxt;
    public Text chipIsClickedTxt;

    public Sprite[] diceSprites;
    public Button rollBtn;
    public Button restartBtn;
    public Button chipOutBtn;
    public Image diceOne, diceTwo;

    GameManager gm;

    private void Awake()
    {
        INSTANCE = this;
    }

    void Start()
    {
        rollBtn.onClick.AddListener(OnDiceRoll);
        restartBtn.onClick.AddListener(() => SceneManager.LoadScene(0));
        chipOutBtn.onClick.AddListener(ChipOut);
        gm = GameManager.INSTANCE;
    }

    void OnDiceRoll()
    {
        UpdateRollPanel(false);
        gm.FullTurn();
        var n = gm.DiceRoll();
        diceOne.sprite = VisualizeRoll(n[0]);
        diceTwo.sprite = VisualizeRoll(n[1]);
    }

    Sprite VisualizeRoll(int n)
    {
        if (n == 1) return diceSprites[0];
        else if (n == 2) return diceSprites[1];
        else if (n == 3) return diceSprites[2];
        else if (n == 4) return diceSprites[3];
        else if (n == 5) return diceSprites[4];
        else return diceSprites[5];
    }

    public void UpdateRollPanel(bool x)
    {
        rollBtn.gameObject.SetActive(x);
        diceOne.gameObject.SetActive(!x);
        diceTwo.gameObject.SetActive(!x);
        gm.canPlay = !x;
    }

    void ChipOut()
    {
        chipOutBtn.gameObject.SetActive(false);
        gm.Score();
    }

}
