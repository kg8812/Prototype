// using UnityEngine;
//
// namespace Apis
// {
//     public class OptimizationActivator: MonoBehaviour
//     {
//         private void OnTriggerEnter2D(Collider2D other)
//         {
//             if (other.gameObject.CompareTag("Enemy") && other.isTrigger)
//             {
//                 if (other.gameObject.TryGetComponent(out CommonMonster2.CommonMonster2 monster))
//                 {
//                     // Debug.Log("monster Activated");
//                     monster._isInMonsterActivator = true;
//                 }
//             }
//         }
//
//         private void OnTriggerExit2D(Collider2D other)
//         {
//             // isTrigger false인 collider가 진짜 몬스터 물리 콜라이더(true는 피격 콜라이더)
//             if (other.gameObject.CompareTag("Enemy") && other.isTrigger)
//             {
//                 if (other.gameObject.TryGetComponent(out CommonMonster2 monster))
//                 {
//                     
//                     // Debug.Log("monster Activated nooooooooo");
//                     monster._isInMonsterActivator = false;
//                 }
//             }
//         }
//     }
// }