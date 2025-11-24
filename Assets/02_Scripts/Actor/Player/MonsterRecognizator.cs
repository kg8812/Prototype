using UnityEngine;

namespace Apis
{
    public class MonsterRecognizator: MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Enemy") && other.isTrigger)
            {
                if (other.TryGetComponent(out CommonMonster2.CommonMonster2 monster))
                {
                    // Debug.Log("monster recognized");
                    monster.IsInRecognitionCircle = true;
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Enemy") && other.isTrigger)   
            {
                if (other.TryGetComponent(out CommonMonster2.CommonMonster2 monster))
                {
                    // Debug.Log("monster recognized noooooooo");
                    monster.IsInRecognitionCircle = false;
                }
            }
        }
    }
}