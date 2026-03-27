using System;
using Apis;
using UnityEngine;

[RequireComponent(typeof(BuffSystem))]
public partial class Actor
{
    private BuffSystem _buffSystem;
    public BuffSystem BuffSystem => _buffSystem ??= GetComponent<BuffSystem>();
}