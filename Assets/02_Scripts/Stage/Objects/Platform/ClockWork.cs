using System.Collections.Generic;
using Apis;
using Default;
using Sirenix.OdinInspector;
using UnityEngine;

public class ClockWork : PlatformCreator
{
    [LabelText("발판 갯수")] public int count;
    [LabelText("회전 속도 (angle/s")] public float speed;
    [LabelText("회전방향")] public CircleAround.Direction direction;
    [LabelText("설정 Layer")] public LayerMask layers;
    private List<FootholdStruct> footHolds = new();
    private float radius;

    private void Awake()
    {
        CreateFootHolds();
    }

    private void FixedUpdate()
    {
        footHolds.ForEach(footHold => { footHold.circleMove.Update(); });
    }

    public override GameObject CreatePlatform(Vector2 position)
    {
        var footHold = base.CreatePlatform(position);
        var circleMove = new CircleAround(this, footHold.transform, radius, speed, direction);
        var movingObj = footHold.GetOrAddComponent<MovingObj>();
        FootholdStruct footholdStruct = new(circleMove, movingObj);
        footHolds.Add(footholdStruct);

        circleMove.lookCenter = false;
        movingObj.layers = layers;

        return footHold;
    }

    public override void Return(GameObject platform)
    {
        if (platform.TryGetComponent(out MovingObj movingObj)) Destroy(movingObj);
        base.Return(platform);
    }

    private void CreateFootHolds()
    {
        footHolds ??= new List<FootholdStruct>();
        radius = transform.lossyScale.x / 2;
        var angle = 360f / count;

        for (var i = 0; i < count; i++)
        {
            var rad = angle * i * Mathf.Deg2Rad;
            Vector2 spawnPos = transform.position + new Vector3(radius * Mathf.Sin(rad), radius * Mathf.Cos(rad));
            CreatePlatform(spawnPos);
            footHolds[i].circleMove.Degree = angle * i;
        }
    }

    private struct FootholdStruct
    {
        public readonly CircleAround circleMove;
        public readonly MovingObj movingObj;

        public FootholdStruct(CircleAround circleMove, MovingObj movingObj)
        {
            this.circleMove = circleMove;
            this.movingObj = movingObj;
        }
    }
}