using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public HpController hpController;
    StaminaController staminaController;

    public Image hpBar;
    public Image staminaBar;

    public float maxHp = 10;

    public ShortRangeWeapon playerSpear;
    public StateMachineController stateMachineController;

    private void Start()
    {
        staminaController = GetComponent<StaminaController>();
    }
    private void Update()
    {
        hpBar.fillAmount = hpController.hp / maxHp;
        staminaBar.fillAmount = staminaController.stamina / staminaController.maxStamina;
    }
    
}
