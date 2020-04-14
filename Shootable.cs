using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Top Class for weapons: Shootables, Shootables, and Melees
 * Use as template for all shootables.
 */

public class Shootable : Weapon_Control
{
    private Shootable_Object.BulletType bulletMode;
    private GameObject bullet;
    private RectTransform[] crosshairs;
    private Vector3[] crosshairsPos;
    private float fireRate;
    private float runTime;
    private int ammo;
    private Vector2 burst, anteAttackBurst;
    private bool shootReady;
    private TimeBased reloadTimeController;
    private TimeBased shootController;
    private TimeBased burstController;

    public Transform canvas;

    public void OnEnable()
    {
        Event_Controller.attackEvent += FireWeapon;
        bulletMode = shootableWeapon.BulletMode;
        bullet = shootableWeapon.Bullet;
        fireRate = shootableWeapon.ShootableFireRate;
        ammo = shootableWeapon.Ammo;
        burst = shootableWeapon.ShootableBurst.BaseBurst;

        Event_Controller.addAimStream(AimBurst);
        Event_Controller.addWalkStream(WalkBurst);
        Event_Controller.addRunStream(RunBurst);
        Event_Controller.addJumpStream(JumpBurst);
        Event_Controller.addIdleStream(IdleBurst);
        Event_Controller.addStateChangeStream(ChangeCrossHair);
        SetCrosshairs();
    }

    
    private void FireWeapon()
    {
        if (shootReady)
        {
            shootReady = false;
            Instantiate(shootableWeapon.ShootParticle, heldWeaponData.getShootOrigin(), Quaternion.identity);
            Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeAmmo(ammo);
            runTime = 0;
            ammo -= 1;
            AttackBurst();

            for (int i = 0; i < shootableWeapon.ShootableAmount; i++)
            {
                 Event_Controller.TimedEvent(ShootWeapon, null, shootableWeapon.ShootableLatency * i, out shootController);
            }
        }
    }

    private void ShootWeapon()
    {
        GameObject bulletGb = Instantiate(bullet, Instant_Reference.getRightHandPosition(), Quaternion.identity);
        if (bulletGb.GetComponent<Object_Motion>() != null)
        {
            bulletGb.GetComponent<Object_Motion>().setFlight(
                        shootableWeapon.ShootableForce, shootableWeapon.ShootableDamage, shootableWeapon.ShootableBlastRange,
                        heldWeapon.GetComponent<Weapon_Data>().getShootOrigin(),
                        CalculateBurst(
                            Instant_Reference.getRightHandToHitRay(shootableWeapon.ShootableForce).origin,
                            Instant_Reference.getRightHandToHitRay(shootableWeapon.ShootableForce).direction,
                            shootableWeapon.ShootableForce, Instant_Reference.UIController.GetComponent<Screen_Interface>().GetCrosshairBurst()),
                         shootableWeapon.HitParticle,
                        shootableWeapon.ShootableAttributes.ShootableAttributeList, shootableWeapon.ShootableAttributes.ShootableAttributeValues,
                        Instant_Reference.GetPlayerLayermask(),
                        bulletMode.ToString(), Instant_Reference.FixArcSine(PlayerAngle()), shootableWeapon.ShootableTime);
        }
    }

    private void FireRate()
    {
        runTime += Time.deltaTime;

        if ((ammo <= 0 || Input.GetKeyDown(KeyCode.R)) && reloadTimeController == null)
        {
            shootReady = false;
            Reload();
            ammo = shootableWeapon.Ammo;
        }
        else if (runTime > fireRate && reloadTimeController == null && ammo > 0)
        {
            runTime = 0;
            shootReady = true;
        }
    }

    private void Reload()
    {
        // besure that ammoReloadIndex is reset to integers smaller than 0, because index starts at 0.
        Event_Controller.TimedEvent(() => 
        {
            ammo = shootableWeapon.Ammo; reloadTimeController = null; Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeAmmo(ammo);
        }, 
        null, 2, out reloadTimeController);
    }
    
    private void SetCrosshairs()
    {
        crosshairs = new RectTransform[shootableWeapon.Crosshair.childCount];
        crosshairsPos = new Vector3[shootableWeapon.Crosshair.childCount];
        Transform crosshairParent = Instantiate(shootableWeapon.Crosshair, canvas);

        for (int i = 0; i < shootableWeapon.Crosshair.childCount; i++)
        {
            crosshairs[i] = crosshairParent.GetChild(i).GetComponent<RectTransform>();
            crosshairsPos[i] = crosshairs[i].localPosition;
        }
    }

    private void AimBurst()
    {
        burst = shootableWeapon.ShootableBurst.AimBurst;
    }
    private void WalkBurst()
    {
        burst = shootableWeapon.ShootableBurst.WalkBurst;
    }
    private void RunBurst()
    {
        burst = shootableWeapon.ShootableBurst.RunBurst;
    }
    private void JumpBurst()
    {
        burst = shootableWeapon.ShootableBurst.JumpBurst;
    }
    private void IdleBurst()
    {
        burst = shootableWeapon.ShootableBurst.BaseBurst;
    }
    private void AttackBurst()
    {
        if (burstController != null) { burstController.killTime(false); }
        burst[0] = burst[0] < anteAttackBurst[0] + shootableWeapon.ShootableBurst.MaxAttackBurst 
            ? burst[0] + shootableWeapon.ShootableBurst.AttackBurstIncrement[0] 
            : anteAttackBurst[0] + shootableWeapon.ShootableBurst.MaxAttackBurst;

       Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeCrosshair(UnAttackBurst, crosshairs,
           burst[0], shootableWeapon.ShootableBurst.AttackBurstIncrement[1], crosshairsPos, true);
    }
    private void UnAttackBurst()
    {
        Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeCrosshair(RestoreBurst, crosshairs, 
            anteAttackBurst[0],shootableWeapon.ShootableBurst.UnAttackBurstSpeed, crosshairsPos, false);
    }
    private void RestoreBurst()
    {
        Event_Controller.TimedEvent(() => burst = anteAttackBurst, null, shootableWeapon.ShootableBurst.BurstRestorationSpeed, out burstController);
    }
    private void ChangeCrossHair()
    { 
        anteAttackBurst = burst;
        Instant_Reference.UIController.GetComponent<Screen_Interface>().ChangeCrosshair(null, crosshairs, burst[0], burst[1], crosshairsPos, true);
    }

    private void OnDisable()
    {
        Event_Controller.removeAimStream(AimBurst);
        Event_Controller.removeWalkStream(WalkBurst);
        Event_Controller.removeRunStream(RunBurst);
        Event_Controller.removeJumpStream(JumpBurst);
        Event_Controller.removeIdleStream(IdleBurst);
        Event_Controller.removeStateChangeStream(ChangeCrossHair);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        FireRate();
    }
}
