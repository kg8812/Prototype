using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    static void UnEquipSkills(Player player)
    {
        player.ChangeActiveSkill(null);
        player.ChangePassiveSkill(null);
    }
}
