using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Screen_Interface : MonoBehaviour
{
    [SerializeField]
    private GameObject damagePopUp;
    [SerializeField]
    private GameObject healthBar;
    [SerializeField]
    private GameObject weaponSlot;
    [SerializeField]
    private TextMeshProUGUI ammoText;

    private Transform healthBarController;
    private Transform selectedSlot;
    public static Transform canvas;
    private Transform crosshairParent;
    private RectTransform[] crosshairTrans = new RectTransform[4];
    private Vector3 currentHealthBarScale;
    private Vector3 newHealthBarScale;
    private Vector3[] crosshairPos;
    private Vector3[] crosshairPosOrigin;
    private Vector3 originalCHPos;
    private float chMoveAmt, chMoveSpeed;
    private float currentSlot;
    private float runningTime;
    private float chRunningTime;

    private const float kInfoY = 80;
    private const float kSlotY = 30;
    private const float kAmmoFromHBX = 120;
    private const float kHealthBarAnimationLength = 1;

    private TimeBased timeBasedClass;
    private TimeBased crossHairTB;

    private void Start()
    {
        currentSlot = 1;
        selectedSlot = weaponSlot.transform.GetChild(0);
        canvas = transform.GetChild(0);
        healthBarController = healthBar.transform.parent;
        healthBarController.position = new Vector3(Screen.width / 2, kInfoY, 0);
        weaponSlot.transform.position = new Vector3(Screen.width / 2, kSlotY, 0);
        ammoText.transform.position = new Vector3(healthBarController.position.x - kAmmoFromHBX, kInfoY, 0);
    }

    public void IndicateDamage(Vector3 position, float damage, Health_Base.DamageType d)
    {
        Color c = d == Health_Base.DamageType.EnemyHead ? Color.yellow : Color.white;
        Vector3 pos = Instant_Reference.mainCamera.WorldToScreenPoint(position);
        GameObject tmp = Instantiate(damagePopUp);

        tmp.GetComponent<Damage_Transform>().SetPosition(position, damage,c);
        tmp.transform.SetParent(canvas);
    }

    public void ChangeHealthBar(float amount)
    {
        runningTime = 0;
        healthBar.transform.parent.GetChild(2).GetComponent<Text>().text = amount.ToString();
        currentHealthBarScale = amount != Player_Controller.playerHealth ? healthBar.transform.localScale : new Vector3(1, 1, 1);
        newHealthBarScale = new Vector3(
            amount / Player_Controller.playerHealth, currentHealthBarScale.y, currentHealthBarScale.z);
        Event_Controller.TimedEvent(null, HealthBarAnimation, kHealthBarAnimationLength, out timeBasedClass);
    }

    private void HealthBarAnimation()
    {
        runningTime += Time.deltaTime;
        float lerpPct = runningTime / kHealthBarAnimationLength;

        healthBar.transform.localScale = Vector3.Slerp(healthBar.transform.localScale, newHealthBarScale, lerpPct);
    }

    public void ChangeAmmo(float amount)
    {
        ammoText.text = amount.ToString();
    }

    public void ChangeCrosshair(Event_Controller.PlayerModeDelegate finishEvent,float moveAmount, float moveSpeed, bool replace)
    {
        if (crossHairTB != null && replace) { crossHairTB.killTime(false); }
        chRunningTime = 0;
        chMoveAmt = moveAmount;
        chMoveSpeed = moveSpeed;

        for (int i = 0; i < crosshairTrans.Length; i++)
        {
            crosshairPos[i] = crosshairPosOrigin[i] * moveAmount;
        }
        Event_Controller.TimedEvent(finishEvent, MoveCrosshair, moveSpeed, out crossHairTB);
    }

    public void SetCrosshair(RectTransform crosshair)
    {
        crosshairTrans = new RectTransform[crosshair.childCount];
        crosshairPos = new Vector3[crosshair.childCount];
        crosshairPosOrigin = new Vector3[crosshair.childCount];

        crosshairParent = Instantiate(crosshair, transform.GetChild(0));

        for (int i = 0; i < crosshair.childCount; i++)
        {
            crosshairTrans[i] = crosshairParent.GetChild(i).GetComponent<RectTransform>();
            crosshairPos[i] = crosshairTrans[i].localPosition;
            crosshairPosOrigin[i] = crosshairTrans[i].localPosition;
        }
    }

    private void MoveCrosshair()
    {
        chRunningTime += Time.deltaTime;
        float pct = chRunningTime / chMoveSpeed;

        for (int i = 0; i < crosshairTrans.Length; i++)
        { 
            crosshairTrans[i].localPosition = Vector3.Lerp(crosshairTrans[i].localPosition, crosshairPos[i], pct);
        }
    }

    public void DeleteCrossHair()
    {
        print(crosshairParent);
        crossHairTB.killTime(false);
        Destroy(crosshairParent.gameObject);
    }

    public float GetCrosshairBurst()
    {
        return crosshairTrans[0].localPosition.magnitude / crosshairPosOrigin[0].magnitude;
    }

    public void SwitchWeaponSlot(int slot)
    {
        Instant_Reference.playerRightHand.GetComponent<Weapon_Control>().toggleWeaponScripts(slot);
        selectedSlot.position = weaponSlot.transform.GetChild(slot).position;
    } 
}
