using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollisionEvents
{
    public void OnCollide(GameObject other);
    public void OnCollideExit(GameObject other);
}
