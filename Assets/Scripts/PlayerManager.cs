using UnityEngine;
using Live2D.Cubism.Core;
using Random = System.Random;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private CubismModel player;
    [SerializeField] private CubismParameter playerColor, playerItem, filled;
    [SerializeField] private string colorID ="ParamDrinkColor";
    [SerializeField] private string itemID = "ParamItem";
    [SerializeField] private string glassEmptiness = "ParamGlassEmptiness";
    [SerializeField] private int filledValue = 10;
    private void Awake()
    {
        player = this.FindCubismModel();
        playerColor = player.Parameters.FindById(colorID);
        playerItem = player.Parameters.FindById(itemID);
        filled = player.Parameters.FindById(glassEmptiness);
    }

    private void Start()
    {
        RandomizeColor();
        RandomizeItem();
        FillGlass();
    }
    
    private void RandomizeColor()
    {
        var random = new Random();
        playerColor.Value = random.Next(0, 21);
    }
    private void RandomizeItem()
    {
        var random = new Random();
        playerItem.Value = random.Next(0, 4);
    }
    private void FillGlass() => filled.Value = filledValue;
}