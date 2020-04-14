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
    private TextMeshProUGUI ammoText;

    private Transform healthBarController;
    public static Transform canvas;
    private RectTransform[] crosshairTrans;
    private Vector3 currentHealthBarScale;
    private Vector3 newHealthBarScale;
    private Vector3[] crossHairPos;
    private Vector3 originalCHPos;
    private float chMoveAmt, chMoveSpeed;
    private float runningTime;
    private float chRunningTime;

    private const float kInfoY = 50;
    private const float kAmmoFromHBX = 120;
    private const float kHealthBarAnimationLength = 1;

    private TimeBased timeBasedClass;
    private TimeBased crossHairTB;

    private void Start()
    {
        canvas = transform.GetChild(0);
        healthBarController = healthBar.transform.parent;
        healthBarController.position = new Vector3(Screen.width / 2, kInfoY, 0);
        ammoText.transform.position = new Vector3(healthBarController.position.x - kAmmoFromHBX, kInfoY, 0);
    }

    public void IndicateDamage(GameObject gb, float damage)
    {
        Vector3 pos = Instant_Reference.mainCamera.WorldToScreenPoint(gb.transform.position);
        GameObject tmp = Instantiate(damagePopUp);
        tmp.GetComponent<Damage_Transform>().SetPosition(gb.transform.position, damage);
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

    public void ChangeCrosshair(Event_Controller.PlayerModeDelegate finishEvent, RectTransform[] crosshairs, float moveAmount, float moveSpeed, Vector3[] originalPos, bool replace)
    {
        if (crossHairTB != null && replace) { crossHairTB.killTime(true); }
        chRunningTime = 0;
        crossHairPos = new Vector3[crosshairs.Length];
        crosshairTrans = crosshairs;
        originalCHPos = originalPos[0];
        chMoveAmt = moveAmount;
        chMoveSpeed = moveSpeed;

        for (int i = 0; i < crosshairs.Length; i++)
        {
            crossHairPos[i] = originalPos[i] * moveAmount;
        }
        Event_Controller.TimedEvent(finishEvent, MoveCrosshair, moveSpeed, out crossHairTB);
    }

    private void MoveCrosshair()
    {
        chRunningTime += Time.deltaTime;
        float pct = chRunningTime / chMoveSpeed;

        for (int i = 0; i < crosshairTrans.Length; i++)
        { 
            crosshairTrans[i].localPosition = Vector3.Lerp(crosshairTrans[i].localPosition, crossHairPos[i], pct);
        }
    }

    public float GetCrosshairBurst()
    {
        return crosshairTrans[0].localPosition.magnitude / originalCHPos.magnitude;
    }
}
