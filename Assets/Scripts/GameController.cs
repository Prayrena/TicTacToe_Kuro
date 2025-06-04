using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

[System.Serializable]
public class Player
{
    public Image panel;
    public TextMeshProUGUI textMeshProUGUI;
    public Button button;
}

[System.Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour
{
    public GridButton[] buttonList;

    private string playerSide;
    private string computerSide;
    public float computerDelay;
    private float computerCurrentDuration;
    private int computerSelectButtonIndex;
    public bool playerMove = true;

    [HideInInspector] public int numTurn = 0;

    public GameObject gameOverPanel;

    public GameObject restartButton;
    public GameObject startInfo;

    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;

    void SetAllButtonsInteractable(bool state)
    {
        foreach (var gridButton in buttonList)
        {
            gridButton.button.interactable = state;
        }
    }

    void SetPlayerButtonsInteractable(bool toggle)
    {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;
    }

    void Awake()
    {
        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);

        SetGameControllerRefOnButtons();

        SetAllButtonsInteractable(false);

        playerMove = true;

        // we'll let player decide to choose O to go first
        // or choose X to go second
        // playerSide = "O";
        // SetPlayerColors(playerO, playerX);
    }

    // give ref to this for all the grid buttons in game
    void SetGameControllerRefOnButtons()
    {
        foreach(var button in buttonList)
        {
            button.SetGameControllerRef(this);
        }
    }

    public string GetCurrentPlayerSide()
    {
        return playerSide;
    }

    public void EndTurn()
    {
        bool isGameOver = false;
        string winningSide = "";

        // check all the rows and columns
        for (int i = 0; i < 3; i++)
        {
            // check the row
            int first = i * 3;
            int second = first + 1;
            int third = second + 1;

            if (buttonList[first].buttonText.text == playerSide &&
                buttonList[second].buttonText.text == playerSide &&
                buttonList[third].buttonText.text == playerSide)
            {
                isGameOver = true;
                winningSide = playerSide;
            }
            else if (buttonList[first].buttonText.text == computerSide &&
            buttonList[second].buttonText.text == computerSide &&
            buttonList[third].buttonText.text == computerSide)
            {
                winningSide = computerSide;
                isGameOver = true;
            }

            // check the column
            first = i;
            second = first + 3;
            third = second + 3;

            if (buttonList[first].buttonText.text == playerSide &&
            buttonList[second].buttonText.text == playerSide &&
            buttonList[third].buttonText.text == playerSide)
            {
                winningSide = playerSide;
                isGameOver = true;
            }
            else if (buttonList[first].buttonText.text == computerSide &&
            buttonList[second].buttonText.text == computerSide &&
            buttonList[third].buttonText.text == computerSide)
            {
                winningSide = computerSide;
                isGameOver = true;
            }

            if (isGameOver)
            {
                GameOver(winningSide);
                break;
            }
        }

        // check the diagonals
        if (!isGameOver)
        {
            if (buttonList[0].buttonText.text == playerSide &&
            buttonList[4].buttonText.text == playerSide &&
            buttonList[8].buttonText.text == playerSide)
            {
                winningSide = playerSide;
                isGameOver = true;
            }
            else if (buttonList[0].buttonText.text == computerSide &&
            buttonList[4].buttonText.text == computerSide &&
            buttonList[8].buttonText.text == computerSide)
            {
                winningSide = computerSide;
                isGameOver = true;
            }

            if (buttonList[2].buttonText.text == playerSide &&
            buttonList[4].buttonText.text == playerSide &&
            buttonList[6].buttonText.text == playerSide)
            {
                winningSide = playerSide;
                isGameOver = true;
            }
            else if (buttonList[0].buttonText.text == computerSide &&
            buttonList[4].buttonText.text == computerSide &&
            buttonList[6].buttonText.text == computerSide)
            {
                winningSide = computerSide;
                isGameOver = true;
            }
        }

        if (isGameOver)
        {
            playerMove = !playerMove;
            GameOver(winningSide);
        }
        else
        {
            foreach (var button in buttonList)
            {
                button.CheckForReset();
            }

            ChangePlayerSide();
            computerCurrentDuration = 0;
            computerSelectButtonIndex = 0;
        }
    }

    // update the new player with active player color
    // the old player to be inactive player color
    void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.textMeshProUGUI.color = activePlayerColor.textColor;

        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.textMeshProUGUI.color = inactivePlayerColor.textColor;
    }

    void ChangePlayerSide()
    {
        // playerSide = (playerSide == "X") ? "O" : "X";
        playerMove = !playerMove;
        ++numTurn;

        if (playerSide == "X")
        {
            SetPlayerColors(playerX, playerO);
            computerSide = "O";
        }
        else
        {
            SetPlayerColors(playerO, playerX);
            computerSide = "X";
        }
    }

    private void Update()
    {
        if(playerMove == false)
        {
            computerCurrentDuration += Time.deltaTime;

            if (computerCurrentDuration < computerDelay) return;

            bool emptySpotFound = false;
            int score = 0;
            int emptyCount = 0;

            // check all the rows and columns
            for (int i = 0; i < 3; i++)
            {
                // check the row
                score = 0;
                emptyCount = 0;

                int first = i * 3;
                int second = first + 1;
                int third = second + 1;

                if (buttonList[first].buttonText.text == computerSide) ++score;
                if (buttonList[second].buttonText.text == computerSide) ++score;
                if (buttonList[third].buttonText.text == computerSide) ++score;

                if (score == 2)
                {
                    if (buttonList[first].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = first; }
                    else if (buttonList[second].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = second; }
                    else if (buttonList[third].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = third; }

                    if (emptyCount != 0)
                    {
                        emptySpotFound = true;
                        break;
                    }
                }

                if (!emptySpotFound)
                {
                    score = 0;
                    emptyCount = 0;

                    if (buttonList[first].buttonText.text == playerSide) ++score;
                    if (buttonList[second].buttonText.text == playerSide) ++score;
                    if (buttonList[third].buttonText.text == playerSide) ++score;

                    if (score == 2)
                    {
                        if (buttonList[first].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = first; }
                        else if (buttonList[second].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = second; }
                        else if (buttonList[third].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = third; }

                        if (emptyCount != 0)
                        {
                            emptySpotFound = true;
                            break;
                        }
                    }
                }

                // check the column
                if (!emptySpotFound)
                {
                    score = 0;
                    emptyCount = 0;

                    first = i;
                    second = first + 3;
                    third = second + 3;

                    if (buttonList[first].buttonText.text == computerSide) ++score;
                    if (buttonList[second].buttonText.text == computerSide) ++score;
                    if (buttonList[third].buttonText.text == computerSide) ++score;

                    if (score == 2)
                    {
                        if (buttonList[first].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = first; }
                        else if (buttonList[second].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = second; }
                        else if (buttonList[third].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = third; }

                        if (emptyCount != 0)
                        {
                            emptySpotFound = true;
                            break;
                        }
                    }

                    if (!emptySpotFound)
                    {
                        score = 0;
                        emptyCount = 0;

                        if (buttonList[first].buttonText.text == playerSide) ++score;
                        if (buttonList[second].buttonText.text == playerSide) ++score;
                        if (buttonList[third].buttonText.text == playerSide) ++score;

                        if (score == 2)
                        {
                            if (buttonList[first].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = first; }
                            else if (buttonList[second].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = second; }
                            else if (buttonList[third].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = third; }

                            if (emptyCount != 0)
                            {
                                emptySpotFound = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!emptySpotFound)
            {
                score = 0;
                emptyCount = 0;

                // check the diagonals
                if (buttonList[0].buttonText.text == computerSide) ++score;
                if (buttonList[4].buttonText.text == computerSide) ++score;
                if (buttonList[8].buttonText.text == computerSide) ++score;

                if (score == 2)
                {
                    if (buttonList[0].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 0; }
                    else if (buttonList[4].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 4; }
                    else if (buttonList[8].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 8; }

                    if (emptyCount != 0)
                    {
                        emptySpotFound = true;
                    }
                }

                if (!emptySpotFound)
                {
                    score = 0;
                    emptyCount = 0;

                    if (buttonList[0].buttonText.text == playerSide) ++score;
                    if (buttonList[4].buttonText.text == playerSide) ++score;
                    if (buttonList[8].buttonText.text == playerSide) ++score;

                    if (score == 2)
                    {
                        if (buttonList[0].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 0; }
                        else if (buttonList[4].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 4; }
                        else if (buttonList[8].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 8; }

                        if (emptyCount != 0)
                        {
                            emptySpotFound = true;
                        }
                    }
                }
            }

            if (!emptySpotFound)
            {
                score = 0;
                emptyCount = 0;

                if (buttonList[2].buttonText.text == computerSide) ++score;
                if (buttonList[4].buttonText.text == computerSide) ++score;
                if (buttonList[6].buttonText.text == computerSide) ++score;

                if (score == 2)
                {
                    if (buttonList[2].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 2; }
                    else if (buttonList[4].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 4; }
                    else if (buttonList[6].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 6; }

                    if (emptyCount != 0)
                    {
                        emptySpotFound = true;
                    }
                }

                if (!emptySpotFound)
                {
                    
                    score = 0;
                    emptyCount = 0;

                    if (buttonList[2].buttonText.text == playerSide) ++score;
                    if (buttonList[4].buttonText.text == playerSide) ++score;
                    if (buttonList[6].buttonText.text == playerSide) ++score;

                    if (score == 2)
                    {
                        if (buttonList[2].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 2; }
                        else if (buttonList[4].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 4; }
                        else if (buttonList[6].buttonText.text == "") { ++emptyCount; computerSelectButtonIndex = 6; }

                        if (emptyCount != 0)
                        {
                            emptySpotFound = true;
                        }
                    }
                }
            }

            if (emptySpotFound)
            {
                buttonList[computerSelectButtonIndex].GetComponentInChildren<TextMeshProUGUI>().text = computerSide;
                buttonList[computerSelectButtonIndex].GetComponentInChildren<Button>().interactable = false;
                buttonList[computerSelectButtonIndex].numTurnWasPressed = numTurn;
                EndTurn();
                return;
            }

            while (!emptySpotFound)
            {

                computerSelectButtonIndex = Random.Range(0, 9);
                if (buttonList[computerSelectButtonIndex].GetComponentInChildren<TextMeshProUGUI>().text == "")
                {
                    emptySpotFound = true;
                    buttonList[computerSelectButtonIndex].GetComponentInChildren<TextMeshProUGUI>().text = computerSide;
                    buttonList[computerSelectButtonIndex].GetComponentInChildren<Button>().interactable = false;
                    buttonList[computerSelectButtonIndex].numTurnWasPressed = numTurn;
                    EndTurn();
                    break;
                }
            }
        }
    }

    public void SetStartingSide(string startingSide)
    {
        playerSide = startingSide;

        if (playerSide == "X")
        {
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            SetPlayerColors(playerO, playerX);
        }

        StartGame();
    }

    void StartGame()
    {
        SetAllButtonsInteractable(true);
        SetPlayerButtonsInteractable(false);
        startInfo.SetActive(false);
    }

    void GameOver(string winningSide)
    {
        restartButton.SetActive(true);

        // disable all the buttons
        foreach (var button in buttonList)
        {
            button.button.interactable = false;
        }

        // based on the player side, update the game over text
        gameOverPanel.SetActive(true);
        // Find the TextMeshProUGUI component in the children
        TextMeshProUGUI text = gameOverPanel.GetComponentInChildren<TextMeshProUGUI>();
        // show which player wins
        text.text = winningSide + " Wins!";
    }

    public void RestartGame()
    {
        gameOverPanel.SetActive(false);
        numTurn = 0;
        playerMove = true;

        // reset each grid button
        foreach (var gridButton in buttonList)
        {
            gridButton.button.interactable = true;
            TextMeshProUGUI text = gridButton.button.GetComponentInChildren<TextMeshProUGUI>();
            text.text = "";

            gridButton.button.image.color = Color.white;
        }

        restartButton.SetActive(false);

        SetPlayerButtonsInteractable(true);

        SetAllPlayersColor(inactivePlayerColor.panelColor, inactivePlayerColor.textColor);
        SetAllButtonsInteractable(false);

        startInfo.SetActive(true);
    }

    void SetAllPlayersColor(Color panelColor, Color textColor)
    {
        playerX.panel.color = panelColor;
        playerX.textMeshProUGUI.color = textColor;

        playerO.panel.color = panelColor;
        playerO.textMeshProUGUI.color = textColor;
    }
}
