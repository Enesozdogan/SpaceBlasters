using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileBullet : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] GetInput inputGetter;
    [SerializeField] GameObject clientProjectile;
    [SerializeField] GameObject serverProjectile;
    [SerializeField] Transform projectileSpawnPoint;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] Collider2D playerCol;
    [SerializeField] CurrencyRepo currencyRepo;

    [Header("Adjustments")]
    [SerializeField] float projectileSpeed;
    [SerializeField] float fireRate;
    [SerializeField] float muzzleFlashDuration;
    [SerializeField] int fireCost;

    NetworkVariable<int> ServerPrefabDmg = new NetworkVariable<int>();


    private bool firing;
    private float muzzleTimer;
    private float fireTimer;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) return;

        inputGetter.PrimaryFireEvent += HandlePrimaryFire;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
    }
    private void Update()
    {
        //muzzle timer
        if (muzzleTimer > 0f)
        {
            muzzleTimer -= Time.deltaTime;
            if (muzzleTimer <= 0f)
            {
                muzzleFlash.SetActive(false);
            }
        }
        if (!IsOwner) return;

        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
        if (!firing) return;

        if (fireTimer > 0) return;

        //if (currencyRepo.TotalCurrencyCount.Value < fireCost) return;

        fireTimer = 1 / fireRate;
        //Soldier Prefab icin up degil right kullan bunu degistirmeyi unutma!!
       
        SpawnClientProjectile(projectileSpawnPoint.position, projectileSpawnPoint.right);
        PrimaryFireServerRpc(projectileSpawnPoint.position, projectileSpawnPoint.right);
    }

    [ServerRpc]
    private void PrimaryFireServerRpc(Vector3 position, Vector3 dir)
    {
        //Currency Control
        // if (currencyRepo.TotalCurrencyCount.Value < fireCost) return;
        // currencyRepo.Spend(fireCost);

     
        GameObject projectileGO = Instantiate(serverProjectile, position, Quaternion.identity);
        //Soldier Prefab icin up degil right kullan bunu degistirmeyi unutma!!
        projectileGO.transform.up = dir;

        if (projectileGO.TryGetComponent<DmgOutput>(out DmgOutput dmgOutput))
        {
            dmgOutput.SetOwnerId(OwnerClientId);
            dmgOutput.damage += ServerPrefabDmg.Value;
        }



        //Kendi Prefabi ile carpismayi onleme
        Physics2D.IgnoreCollision(playerCol, projectileGO.GetComponent<Collider2D>());
        if (projectileGO.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
        {
            rb2d.velocity = projectileGO.transform.up * projectileSpeed;
        }


        SpawnPrimaryProjectileClientRpc(position, dir);
    }

    [ClientRpc]
    private void SpawnPrimaryProjectileClientRpc(Vector3 position, Vector3 dir)
    {
        if (IsOwner) return;
        SpawnClientProjectile(position, dir);
    }
    private void SpawnClientProjectile(Vector3 position, Vector3 dir)
    {


       
        //Muzzle efektinin client larda olusturulmasi
        muzzleTimer = muzzleFlashDuration;
        muzzleFlash.SetActive(true);

        GameObject projectileGO = Instantiate(clientProjectile, position, Quaternion.identity);
        //Soldier Prefab icin up degil right kullan bunu degistirmeyi unutma!!
        projectileGO.transform.up = dir;



        Physics2D.IgnoreCollision(playerCol, projectileGO.GetComponent<Collider2D>());
        if (projectileGO.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
        {
            rb2d.velocity = projectileGO.transform.up * projectileSpeed;
        }


    }

    private void HandlePrimaryFire(bool firing)
    {
        this.firing = firing;

    }


    //UPGRADE FUNCTIONS

    [ServerRpc]
    public void IncreaseDmgServerRpc(int dmgInc, ServerRpcParams serverRpcParams = default)
    {
            ServerPrefabDmg.Value += dmgInc;
    }
}
