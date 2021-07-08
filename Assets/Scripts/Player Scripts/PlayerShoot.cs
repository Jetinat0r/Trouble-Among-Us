using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private WeaponManager weaponManager;
    private Player player;
    [SerializeField]
    private PlayerUI playerUI;
    private bool allowedToShoot = false;
    private bool isReloading = false;
    private bool UIAllowShoot = true;

    private void Start()
    {
        weaponManager = gameObject.GetComponent<WeaponManager>();
        player = gameObject.GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isReloading)
        {
            Shoot(weaponManager.GetCurrentWeapon());
        }
    }

    private void Shoot(Weapon _curWeapon)
    {
        if (allowedToShoot && UIAllowShoot)
        {
            //for (int i = 0; i < _curWeapon.numShots; i++)
            //{
            //    //SET NEW ROTATION FOR BULLAT UP HERE, JUST IN CASE
            //    Vector3 _angle = _curWeapon.shootPoint.transform.rotation.eulerAngles;
            //    _angle.z += Random.Range(-_curWeapon.shotAngle, _curWeapon.shotAngle);

            //    CreateBullet(_curWeapon, _angle);
            //}

            //NOTE: position = shootPoint position
            float _movementSpeed = gameObject.GetComponent<PlayerMovement>().movementSpeed;
            ClientSend.Shoot(_curWeapon.shootPoint.transform.position, _curWeapon.shootPoint.transform.rotation, _movementSpeed);

            //Reload stuff
            StartCoroutine(Reload(_curWeapon.reloadTimer));
        }
    }

    //An easy place to remember what I'm putting into bullets
    //private void CreateBullet(Weapon _curWeapon, Vector3 _angle)
    //{
    //    GameObject _curBullet = Instantiate(_curWeapon.bullet, _curWeapon.shootPoint.transform.position, Quaternion.Euler(_angle));
    //    _curBullet.name = _curWeapon.name + " Bullet";

    //    Bullet _bulletScript = _curBullet.GetComponent<Bullet>();

    //    _bulletScript.velocity = _curWeapon.velocity * Random.Range(1 - _curWeapon.velocityRandomizer, 1 + _curWeapon.velocityRandomizer) * gameObject.GetComponent<PlayerMovement>().movementSpeed;
    //    _bulletScript.damage = _curWeapon.damage;
    //    _bulletScript.shotOffset = _curWeapon.shotOffset;
    //}

    private IEnumerator Reload(float timer)
    {
        isReloading = true;
        playerUI.Reload(timer);

        float t = 0;

        while(t < timer) {
            t += Time.deltaTime;

            yield return null;
        }

        isReloading = false;

        
    }

    public void SwapWeapons()
    {
        if (!isReloading)
        {
            StartCoroutine(Reload(weaponManager.GetCurrentWeapon().reloadTimer / 8));
        }
    }

    public void DisableShooting()
    {
        allowedToShoot = false;
    }

    public void AllowShooting()
    {
        //check dead anyways bc idk exploits and what not
        if (player.isAlive)
        {
            allowedToShoot = true;
        }
    }

    public void UIDisableShooting()
    {
        UIAllowShoot = false;
    }

    public void UIAllowShooting()
    {
        UIAllowShoot = true;
    }
}
