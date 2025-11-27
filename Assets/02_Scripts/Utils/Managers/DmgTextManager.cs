using Default;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Apis
{
    public class DmgTextManager
    {
        private static GameObject dmgText;

        private static GameObject DmgText
        {
            get
            {
                if (dmgText == null) dmgText = ResourceUtil.Load<GameObject>("Prefabs/DmgText");

                return dmgText;
            }
        }

        public static void ShowDmgText(Vector3 pos, float dmg)
        {
            var obj = Object.Instantiate(DmgText);

            obj.transform.position = pos + Vector3.up * Random.Range(0f, 0.5f);
            obj.transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);
            var value = Mathf.RoundToInt(dmg);

            obj.GetComponent<TextMeshPro>().text = value.ToString();
            obj.transform.DOMoveY(obj.transform.position.y + 0.5f, 1).OnComplete(() => Object.Destroy(obj));
        }

        public static void ShowDmgText(Vector3 pos, float dmg, Color color)
        {
            var obj = Object.Instantiate(DmgText);
            obj.transform.position = pos + Vector3.up * Random.Range(0f, 0.5f);
            obj.transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0, 0);

            obj.GetComponent<TextMeshPro>().color = color;
            var value = Mathf.RoundToInt(dmg);
            obj.GetComponent<TextMeshPro>().text = value.ToString();
            obj.transform.DOMoveY(obj.transform.position.y + 0.5f, 1).OnComplete(() => Object.Destroy(obj));
        }
    }
}