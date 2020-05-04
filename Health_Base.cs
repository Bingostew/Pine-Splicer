using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ObjectHealth
{
    void SpawnHealth(float health);
    void Death();
}

public class Health_Base : MonoBehaviour
{
    public static Dictionary<GameObject, float> healthDataBase = new Dictionary<GameObject, float>();
    public static Dictionary<GameObject, float> runningDamage = new Dictionary<GameObject, float>();
    private static TimeBased damageController;
    private delegate void HealthDepletion();

    public static void addEntityHealth(GameObject gb, float health)
    {
        if(!healthDataBase.ContainsKey(gb))
        healthDataBase.Add(gb, health);
    }

    public static void changeEntityHeath(GameObject gb, float depletedHealth, bool cumulativeDamage)
    {
        if (healthDataBase.ContainsKey(gb))
        {
            healthDataBase[gb] -= depletedHealth;
            if (cumulativeDamage) { ShowCumulativeDamage(null, gb, depletedHealth); }
            else { ShowDamage(gb, depletedHealth); }
        }
    }

    private static void ShowDamage(GameObject gb, float depletedHealth)
    {
        if (gb == Instant_Reference.playerReference && healthDataBase[gb] < 100)
        {
            Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeHealthBar(healthDataBase[gb]);
        }
        else if (gb != Instant_Reference.playerReference && healthDataBase.ContainsKey(gb))
        {
            Instant_Reference.UIController.GetComponent<Screen_Interface>().IndicateDamage(gb, depletedHealth);
        }
        if (healthDataBase[gb] <= 0)
        {
            healthDataBase.Remove(gb);
            gb.SendMessage("Death");
        }
    }
    
    private static void ShowCumulativeDamage(Event_Controller.PlayerModeDelegate endEvent, GameObject gb, float depletedHealth)
    {
        if (healthDataBase[gb] <= 0)
        {
            Event_Controller.TimedEvent(() =>
            {
                healthDataBase.Remove(gb);
                if (gb != null) gb.SendMessage("Death");
            }, null, .05f, out damageController);
        }
        if (!runningDamage.ContainsKey(gb))
        {
            runningDamage.Add(gb, depletedHealth);
            Event_Controller.TimedEvent(() =>
            {
                endEvent?.Invoke();
                print(runningDamage[gb]);
                if (gb == Instant_Reference.playerReference)
                {
                    Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeHealthBar(healthDataBase[gb]);
                }
                else if(healthDataBase.ContainsKey(gb)){
                    Instant_Reference.UIController.GetComponent<Screen_Interface>().IndicateDamage(gb, runningDamage[gb]);
                }
                runningDamage.Remove(gb);
            }, null, .05f, out damageController);
        }
        else { runningDamage[gb] += depletedHealth; }
    }

    public static float getEntityHeath(GameObject gb)
    {
        if (healthDataBase.ContainsKey(gb))
        {
            return healthDataBase[gb];
        }
        return 0;
    }
}
