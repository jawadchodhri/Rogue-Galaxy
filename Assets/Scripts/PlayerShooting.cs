using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    private float nextFireTime;

    private void Update()
    {
        TryShoot();
    }

    private void TryShoot()
    {
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        Shoot();
    }

    private void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }
}
