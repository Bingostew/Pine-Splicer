using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Base : MonoBehaviour
{
    public static Dictionary<GameObject, float> healthDataBase = new Dictionary<GameObject, float>();

    public static void addEntityHealth(GameObject gb, float health)
    {
        if(!healthDataBase.ContainsKey(gb))
        healthDataBase.Add(gb, health);
    }

    public static void changeEntityHeath(GameObject gb, float depletedHealth)
    {
        if (healthDataBase.ContainsKey(gb))
        {
            healthDataBase[gb] -= depletedHealth;
        }
        if(healthDataBase.ContainsKey(gb) && healthDataBase[gb] <= 0)
        {
            gb.SendMessage("Death");
            healthDataBase.Remove(gb);
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

public interface ObjectHealth
{
    void SpawnHealth(float health);
     void Death();
}