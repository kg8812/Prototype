using UnityEngine;

public interface ICollisionEvents
{
    public void OnCollide(GameObject other);
    public void OnCollideExit(GameObject other);
}