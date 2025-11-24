using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer : IMonoBehaviour
{
    void CorrectingPlayerPosture(bool isLanding = true);
}
