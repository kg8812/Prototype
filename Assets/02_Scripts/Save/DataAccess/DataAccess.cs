using System.Collections;
using System.Collections.Generic;
using Save.Schema;
using UnityEngine;

namespace Save.Schema
{
    public static class DataAccess
    {
        public static readonly Settings Settings = new();
        public static readonly GameData GameData = new();
    }
}