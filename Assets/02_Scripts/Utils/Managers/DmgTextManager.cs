using DG.Tweening;
using Default;
using TMPro;
using UnityEngine;

namespace Apis
{
    public class DmgTextManager
    {

        static GameObject dmgText;

        static GameObject DmgText
        {
            get
            {
                if (dmgText == null)
                {
                    dmgText = ResourceUtil.Load<GameObject>("Prefabs/DmgText");
                }

                return dmgText;
            }
        }

        public static void ShowDmgText(Vector3 pos, float dmg)
        {            
            GameObject obj = Object.Instantiate(DmgText);

            obj.transform.position = pos + Vector3.up * Random.Range(0f,0.5f);
            obj.transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
            int value = Mathf.RoundToInt(dmg);

            obj.GetComponent<TextMeshPro>().text = value.ToString();
            obj.transform.DOMoveY(obj.transform.position.y + 0.5f,1).OnComplete(() => Object.Destroy(obj));
        }

        public static void ShowDmgText(Vector3 pos, float dmg,Color color)
        {
            GameObject obj = Object.Instantiate(DmgText);
            obj.transform.position = pos + Vector3.up * Random.Range(0f, 0.5f);
            obj.transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);

            obj.GetComponent<TextMeshPro>().color = color;
            int value = Mathf.RoundToInt(dmg);
            obj.GetComponent<TextMeshPro>().text = value.ToString();
            obj.transform.DOMoveY(obj.transform.position.y + 0.5f, 1).OnComplete(() => Object.Destroy(obj));
        }
    }
}