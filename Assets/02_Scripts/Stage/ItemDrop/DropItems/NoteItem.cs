using System.Collections;
using System.Collections.Generic;
using Apis;
using Apis.Managers;
using Managers;
using UnityEngine;


public class NoteItem : DropItem
{
    private void Awake()
    {
        isInteractable = true;
    }
    public int noteIndex;
    public override void InvokeInteraction()
    {
        var data = NoteModel.notes[noteIndex];

        SystemManager.SystemAlert($"{data.note_name_korean}\n{data.note_desc_korean}", () =>
        {
            Destroy(gameObject);
        });
    }

    protected override void ReturnObject(SceneData _)
    {
        base.ReturnObject(_);
        Destroy(gameObject);
    }
}
