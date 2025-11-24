using Save.Schema;
using UnityEngine.Serialization;

namespace Apis.DataType
{
    [System.Serializable]
    public class StringDataType
    {
        public string kr;
        public string en;
    }
    
    [System.Serializable]
    public class TipDataType
    {
        public int index;
        public string tipNameKor;
        public string tipNameEng;
        public string tipDescKor;
        public string tipDescEng;
        public string helpVideo;
    }

    [System.Serializable]
    public class CutSceneDataType
    {
        public int index;
        public string nameText;
        public string thumbnail;
        public string video;
    }
    [System.Serializable]
    public class TmiDataType
    {
        public string tmi;
    }
    
    [System.Serializable]
    public class ScriptDataType
    {
        public int id;
        public int speaker;
        public int index;
        public int priority;
        public bool unique;
        public bool isAuto;
        public float[] duration;
        public string[] scriptsKor;
        public string[] scriptsEng;
    }
    
    [System.Serializable]
    public class NoteDataType
    {
        public int index;
        public string note_name_korean;
        public string note_name_english;
        public string note_desc_korean;
        public string note_desc_english;
    }
    
    [System.Serializable]
    public class InGameCutSceneDataType
    {
        public int cutSceneId;
        public int index;
        public int speaker;
        public bool isAuto;
        public float duration;
        public string scriptKor;
        public string scriptEng;
    }
}