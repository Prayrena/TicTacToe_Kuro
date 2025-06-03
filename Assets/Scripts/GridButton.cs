using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GridButton : MonoBehaviour
{
    public Button button;
    public TextMeshProUGUI buttonText;
    [HideInInspector]public int numTurnWasPressed = 0;

    private GameController gameController;

    public void SetSpace()
    {
        buttonText.text = gameController.GetCurrentPlayerSide();
        numTurnWasPressed = gameController.numTurn;

        button.interactable = false;
        gameController.EndTurn();
    }

    public void SetGameControllerRef(GameController controller)
    {
        gameController = controller;
    }

    public void CheckForReset()
    {
        // this button is change to empty after it was pressed 5 turns later
        if(gameController.numTurn == (numTurnWasPressed + 6) && buttonText.text != "")
        {
            buttonText.text = "";
            button.interactable = true;

            button.image.color = Color.white;
        }
        else if(gameController.numTurn == (numTurnWasPressed + 5) && buttonText.text != "")
        {
            button.image.color = Color.yellow;
        }
    }
}
