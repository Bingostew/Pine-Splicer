using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * Top Class for weapons: Shootables, Shootables, and Melees
 * Use as template for all shootables.
 */
 
    /* TODO: Bug: When throwing, instantiation puts new object at current object position, causing layer parameter to be null
      */

public class Shootable : Weapon_Control
{
    private Shootable_Object.BulletType bulletMode;
    private GameObject bullet;
    private float fireRate;
    private float runTime;
    private int ammo;
    private Vector2 burst, anteAttackBurst;
    private bool shootReady;
    private TimeBased reloadTimeController;
    private TimeBased shootController;
    private TimeBased burstController;

    [SerializeField]
    private GameObject UIController;

    public void OnEnable()
    {
        Event_Controller.attackEvent += FireWeapon;
        bulletMode = shootableWeapon.BulletMode;
        bullet = shootableWeapon.Bullet;
        fireRate = shootableWeapon.ShootableFireRate;
        ammo = shootableWeapon.Ammo;
        burst = shootableWeapon.ShootableBurst.BaseBurst;
        UIController.GetComponent<Screen_Interface>().SetCrosshair(shootableWeapon.Crosshair);
        Event_Controller.addAimStream(AimBurst);
        Event_Controller.addWalkStream(WalkBurst);
        Event_Controller.addRunStream(RunBurst);
        Event_Controller.addJumpStream(JumpBurst);
        Event_Controller.addIdleStream(IdleBurst);
        Event_Controller.addStateChangeStream(ChangeCrossHair);
        ChangeCrossHair();
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

    private void DetectHitscanHit() // **KNown Issue - calling damageAttributes cause damage marker instantiate on gun position, not hit position
    {
        RaycastHit h;
        if (Physics.Raycast(heldWeapon.GetComponent<Weapon_Data>().getShootOrigin(), CalculateBurst(heldWeapon.GetComponent<Weapon_Data>().getShootOrigin(),
                            Instant_Reference.getRightHandToHitRay(shootableWeapon.ShootableForce).direction,
                            shootableWeapon.ShootableForce, Instant_Reference.UIController.GetComponent<Screen_Interface>().GetCrosshairBurst()) -
                            heldWeapon.GetComponent<Weapon_Data>().getShootOrigin(), 
                            out h, shootableWeapon.ShootableForce, LayerMask.GetMask(Instant_Reference.GetPlayerLayermask())))
        {
            OnObjectHit(h.transform.gameObject, gameObject, shootableWeapon.ShootableDamage, shootableWeapon.ShootableCriticalDamage);
        }
    }

    private void ShootWeapon()
    {
        if (shootableWeapon.BulletMode == Shootable_Object.BulletType.hitscan)
        {
            DetectHitscanHit();
        }

        else
        {
            GameObject bulletGb = Instantiate(bullet, Instant_Reference.getRightHandPosition(), Quaternion.identity);
            bulletGb.GetComponent<Object_Motion>().setFlight(
                        shootableWeapon.ShootableForce, shootableWeapon.ShootableDamage, shootableWeapon.ShootableCriticalDamage, shootableWeapon.ShootableBlastRange,
                        heldWeapon.GetComponent<Weapon_Data>().getShootOrigin(),
                        CalculateBurst(
                            Instant_Reference.getRightHandToHitRay(shootableWeapon.ShootableForce).origin,
                            Instant_Reference.getRightHandToHitRay(shootableWeapon.ShootableForce).direction,
                            shootableWeapon.ShootableForce, Instant_Reference.UIController.GetComponent<Screen_Interface>().GetCrosshairBurst()),
                         shootableWeapon.HitParticle,
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
        null, shootableWeapon.ReloadSpeed, out reloadTimeController);
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
        float b = Instant_Reference.UIController.GetComponent<Screen_Interface>().GetCrosshairBurst();

        if (burstController != null) { burstController.killTime(true); }
        burst[0] = b + shootableWeapon.ShootableBurst.AttackBurstIncrement[0] < anteAttackBurst[0] + shootableWeapon.ShootableBurst.MaxAttackBurst 
            ? b + shootableWeapon.ShootableBurst.AttackBurstIncrement[0] 
            : anteAttackBurst[0] + shootableWeapon.ShootableBurst.MaxAttackBurst;

       UIController.GetComponent<Screen_Interface>().ChangeCrosshair(UnAttackBurst,
           burst[0], shootableWeapon.ShootableBurst.AttackBurstIncrement[1], true);
    }
    private void UnAttackBurst()
    {
        float unAttackSpeed = Instant_Reference.UIController.GetComponent<Screen_Interface>().GetCrosshairBurst() / anteAttackBurst[0]
            * shootableWeapon.ShootableBurst.UnAttackBurstSpeed;

        UIController.GetComponent<Screen_Interface>().ChangeCrosshair(() => burst = anteAttackBurst, anteAttackBurst[0],
            unAttackSpeed, false);
    }

    private void ChangeCrossHair()
    { 
        anteAttackBurst = burst;
        UIController.GetComponent<Screen_Interface>().ChangeCrosshair(null, burst[0], burst[1], true);
    }

    private void OnDisable()
    {
        Event_Controller.removeAimStream(AimBurst);
        Event_Controller.removeWalkStream(WalkBurst);
        Event_Controller.removeRunStream(RunBurst);
        Event_Controller.removeJumpStream(JumpBurst);
        Event_Controller.removeIdleStream(IdleBurst);
        Event_Controller.removeStateChangeStream(ChangeCrossHair);
        Event_Controller.attackEvent -= FireWeapon;
        UIController.GetComponent<Screen_Interface>().DeleteCrossHair(); // must call here because enable/disable weapon functions in same method
    }

    private void Update()
    {
        FireRate();
    }
}
