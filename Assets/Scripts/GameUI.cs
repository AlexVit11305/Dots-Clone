using System;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [SerializeField] 
    private Text scoreLabel;

    [Header("Buttons")] 
    [SerializeField] 
    private Button clearSaveButton;

    public event Action onClearSaveButtonClick;

    private void Awake()
    {
        clearSaveButton.onClick.AddListener(() => onClearSaveButtonClick?.Invoke());
    }

    public void UpdateInfo(GameSession current)
    {
        if(current == null)
            scoreLabel.text = "Total";
        else
        {
            scoreLabel.text = "Total: " + current.scores.ToString();
        }
    }
}
