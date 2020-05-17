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
    public enum DamageType
    {
        EnemyNormal,
        EnemyHead,
        none
    }
    private static DamageType damageTypeInstance;
    public static Dictionary<GameObject, float> healthDataBase = new Dictionary<GameObject, float>();
    public static Dictionary<GameObject, float> runningDamage = new Dictionary<GameObject, float>();
    private static TimeBased damageController;
    private delegate void HealthDepletion();

    public static void addEntityHealth(GameObject gb, float health)
    {
        if(!healthDataBase.ContainsKey(gb))
        healthDataBase.Add(gb, health);
    }

    /// <summary>
    /// heals or damages entity
    /// </summary>
    /// <param name="gb">Gameobject of entity</param>
    /// <param name="position">Position where the damage marker will appear</param>
    /// <param name="depletedHealth">Health taken (Negative if healing)</param>
    /// <param name="cumulativeDamage">If damage appears on a timer (For instantaneous shots like shotguns)</param>
    public static void changeEntityHeath(GameObject gb, Vector3 position, float depletedHealth, bool cumulativeDamage, DamageType d)
    {
        if (healthDataBase.ContainsKey(gb))
        {
            healthDataBase[gb] -= depletedHealth;

            if (cumulativeDamage) { ShowCumulativeDamage(null, gb, position, depletedHealth, d); }
            else { ShowDamage(gb, position, depletedHealth, d); }

        }
    }

    
    private static void ShowDamage(GameObject gb, Vector3 position, float depletedHealth, DamageType d)
    {
        if (gb == Instant_Reference.playerReference && healthDataBase[gb] < 100)
        {
            Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeHealthBar(healthDataBase[gb]);
        }
        else if (gb != Instant_Reference.playerReference && healthDataBase.ContainsKey(gb))
        {
            Instant_Reference.UIController.GetComponent<Screen_Interface>().IndicateDamage(position, depletedHealth, d);
        }
        if (healthDataBase[gb] <= 0)
        {
            healthDataBase.Remove(gb);
            gb.SendMessage("Death");
        }
    }
    
    private static void ShowCumulativeDamage(Event_Controller.PlayerModeDelegate endEvent, GameObject gb, Vector3 position, float depletedHealth, DamageType d)
    {
        if (!runningDamage.ContainsKey(gb))
        {
            runningDamage.Add(gb, depletedHealth);
            Event_Controller.TimedEvent(() =>
            {
                if(damageTypeInstance != DamageType.EnemyHead) { damageTypeInstance = d; } // if no bullet hits head, then display as normal
                endEvent?.Invoke();
                print(runningDamage[gb]);
                if (gb == Instant_Reference.playerReference)
                {
                    Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeHealthBar(healthDataBase[gb]);
                }
                else 
                {
                    Instant_Reference.UIController.GetComponent<Screen_Interface>().IndicateDamage(position, runningDamage[gb], damageTypeInstance);
                }
                runningDamage.Remove(gb);
                damageTypeInstance = DamageType.none; // resets instance
            }, null, .05f, out damageController);
        }
        else
        {
            runningDamage[gb] += depletedHealth;
            if (d == DamageType.EnemyHead) { damageTypeInstance = d; } // customized to that if one bullet hits head, then display as headshot
        }
        if (healthDataBase[gb] <= 0)
        {
            Event_Controller.TimedEvent(() =>
            {
                healthDataBase.Remove(gb);
                if (gb != null) gb.SendMessage("Death");
            }, null, .1f, out damageController);
        }
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
