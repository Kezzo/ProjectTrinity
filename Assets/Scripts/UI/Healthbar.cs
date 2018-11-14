using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour 
{
    [SerializeField]
    private Image healthbarImage;

    [SerializeField]
    private Color fullHealthColor;

    [SerializeField]
    private Color damagedHealthColor;

    public void UpdateHealthFill(float fillamount)
    {
        healthbarImage.fillAmount = fillamount;
        healthbarImage.color = fillamount < 1f ? damagedHealthColor : fullHealthColor;
    }
}
