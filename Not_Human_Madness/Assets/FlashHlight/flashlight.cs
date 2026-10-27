using UnityEngine;
using UnityEngine.UI; 
using TMPro;          

public class Flashlight : MonoBehaviour
{
    [Header("Настройки фонарика")]
    public Light targetLight;

    [Header("Настройки батареи")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float drainSpeed = 5f;    
    public float chargeTime = 10f;   

    [Header("UI Элементы")]
    public TextMeshProUGUI batteryText; 

    private bool isCharging = false;

    private void Start()
    {
        if (targetLight == null)
            targetLight = GetComponent<Light>();

        if (targetLight != null)
            targetLight.enabled = false;

        UpdateUIText();
    }

    private void Update()
    {
       
        if (Input.GetKey(KeyCode.M) && currentBattery < maxBattery)
        {
            isCharging = true;

          
            if (targetLight != null) targetLight.enabled = false;

           
            float chargeRate = maxBattery / chargeTime;
            currentBattery += chargeRate * Time.deltaTime;

           
            if (currentBattery > maxBattery) currentBattery = maxBattery;

            UpdateUIText();
        }
        else
        {
            isCharging = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (currentBattery > 0 && !isCharging && targetLight != null)
            {
                targetLight.enabled = !targetLight.enabled;
            }
        }

        if (targetLight != null && targetLight.enabled)
        {
            currentBattery -= drainSpeed * Time.deltaTime;
            UpdateUIText();

            if (currentBattery <= 0)
            {
                currentBattery = 0;
                targetLight.enabled = false;
                Debug.Log("Батарейка села!");
            }
        }
    }

    private void UpdateUIText()
    {
        if (batteryText != null)
        {
            batteryText.text = Mathf.CeilToInt(currentBattery).ToString();

            if (currentBattery <= 20)
                batteryText.color = Color.red;
            else
                batteryText.color = Color.white;
        }
    }
}