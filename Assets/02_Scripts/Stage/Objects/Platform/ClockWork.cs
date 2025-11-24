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

    struct FootholdStruct
    {
        public readonly CircleAround circleMove;
        public readonly MovingObj movingObj;

        public FootholdStruct(CircleAround circleMove, MovingObj movingObj)
        {
            this.circleMove = circleMove;
            this.movingObj = movingObj;
        }
    }
    List<FootholdStruct> footHolds = new ();
    private float radius;
    private void Awake()
    {
        CreateFootHolds();
    }

    public override GameObject CreatePlatform(Vector2 position)
    {
        GameObject footHold = base.CreatePlatform(position);
        var circleMove = new CircleAround(this, footHold.transform, radius, speed, direction);
        MovingObj movingObj = footHold.GetOrAddComponent<MovingObj>();
        FootholdStruct footholdStruct = new(circleMove,movingObj);
        footHolds.Add(footholdStruct);
        
        circleMove.lookCenter = false;
        movingObj.layers = layers;
        
        return footHold;
    }

    public override void Return(GameObject platform)
    {
        if (platform.TryGetComponent(out MovingObj movingObj))
        {
            Destroy(movingObj);
        }
        base.Return(platform);
    }

    void CreateFootHolds()
    {
        footHolds ??= new();
        radius = transform.lossyScale.x / 2;
        float angle = 360f / count;
        
        for (int i = 0; i < count; i++)
        {
            float rad = angle * i * Mathf.Deg2Rad;
            Vector2 spawnPos = transform.position + new Vector3(radius * Mathf.Sin(rad), radius * Mathf.Cos(rad));
            CreatePlatform(spawnPos);
            footHolds[i].circleMove.Degree = angle * i;
        }
    }

    private void FixedUpdate()
    {
        footHolds.ForEach(footHold =>
        {
            footHold.circleMove.Update();
        });
    }

    
}
