using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBattery : MonoBehaviour
{
    [SerializeField] private float maxCharge = 100f; // Maximum battery charge
    [SerializeField] private float currentCharge; // Current battery charge
    [SerializeField] private float rechargeRate = 2f; // Rate of charge replenishment per second
    [SerializeField] private float criticalChargeThreshold = 10f; // Threshold for critical battery level
    [SerializeField] private float nominalVoltage = 12f; // Nominal voltage of the battery
    [SerializeField] private float loadEffectMultiplier = 0.5f; // Multiplier to simulate load effect on voltage
    [SerializeField] private float recoveryRate = 0.1f; // Rate at which voltage recovers when not under load

    [SerializeField] private float useRate = 0.1f; // Rate at which voltage recovers when not under load

    [SerializeField] private bool infinityMode = false; // Enable infinite battery mode

    private float currentVoltage; // Current voltage of the battery

    private void Start()
    {
        currentCharge = maxCharge; // Initialize battery charge
        UpdateVoltage(); // Initialize voltage based on full charge
    }

    private void Update()
    {
        // Check if the battery is critically low
        if (currentCharge == 0) {
            Debug.LogWarning("Battery is dead! Please recharge.");
        } else if (currentCharge <= criticalChargeThreshold) {
            Debug.LogWarning("Battery critical! Please recharge.");
        }

        // Simulate recovery of voltage if the load is not applied
        if (currentVoltage < nominalVoltage)
        {
            currentVoltage += recoveryRate * Time.deltaTime;
            currentVoltage = Mathf.Clamp(currentVoltage, 0, nominalVoltage); // Prevent over-recovery
        }
    }

    public void UseBatteryPower(float amount)
    {
        if (!infinityMode) // Only affect charge if not in Infinity Mode
        {
            currentCharge -= amount * useRate;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge); // Prevent negative charge

            // Simulate load effect on voltage
            currentVoltage = nominalVoltage * (currentCharge / maxCharge) * loadEffectMultiplier;
        }
    }

    public void RechargeBattery(float amount)
    {
        if (!infinityMode) // Only recharge if not in Infinity Mode
        {
            // Recharge the battery by a certain amount
            currentCharge += amount * rechargeRate * Time.deltaTime;
            currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge); // Prevent overcharging

            // Update voltage based on the current charge
            UpdateVoltage();
        }
    }

    private void UpdateVoltage()
    {
        // Calculate current voltage based on the current charge
        currentVoltage = nominalVoltage * (currentCharge / maxCharge);
    }

    public float GetCurrentCharge()
    {
        return currentCharge;
    }

    public float GetCurrentVoltage()
    {
        return currentVoltage;
    }

    public bool IsBatteryDepleted()
    {
        return currentCharge <= 0 && !infinityMode; // Battery is depleted unless in Infinity Mode
    }

    public void ResetBattery()
    {
        currentCharge = maxCharge; // Reset the battery to full charge
        UpdateVoltage(); // Reset voltage based on full charge
    }

    // Method to enable or disable Infinity Mode
    public void SetInfinityMode(bool enabled)
    {
        infinityMode = enabled;
        if (infinityMode)
        {
            currentCharge = maxCharge; // Ensure charge is full when enabling Infinity Mode
        }
    }

    public bool IsInfinityModeActive()
    {
        return infinityMode;
    }
}
