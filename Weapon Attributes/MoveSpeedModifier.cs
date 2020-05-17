using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSpeedModifier : AttributeInstance
{
    [SerializeField]
    private float slownessLength = 0;
    [SerializeField]
    private float slownessPercent = 0;
    private TimeBased t;
    
    private void OnEnable()
    {
        AddAttribute(gameObject, startModifier);
    }

    public void startModifier(GameObject hitGb)
    {
        if (!speedStream.ContainsKey(hitGb))
        {
            Event_Controller.TimedEvent(() =>
            {
                if (hitGb != null)
                {
                    switch (hitGb.tag)
                    {
                        case "Enemy":
                            Weapon_Control.GetHealthBaseObject(hitGb).GetComponent<Enemy_Controller>().enemySpeedModifier = 1;
                            break;
                        case "Player":
                            Player_Controller.speedModifier = 1;
                            break;
                    }
                }
                speedStream.Remove(hitGb);
            }, null, slownessLength, out t);

            switch (hitGb.tag)
            {
                case "Enemy":
                    Weapon_Control.GetHealthBaseObject(hitGb).GetComponent<Enemy_Controller>().enemySpeedModifier *= slownessPercent;
                    AddSpeedStream(hitGb, t);
                    break;
                case "Player":
                    Player_Controller.speedModifier *= slownessPercent;
                    AddSpeedStream(hitGb, t);
                    break;
            }
        }
    }
}
