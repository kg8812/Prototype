using UnityEngine;

namespace Apis
{
    public interface ITriggerStrategy
    {
        public void OnTriggerEnter2D(Collider2D col);

        /**
         * * 만약 Stay도 사용한다면 주석 처리 off
         * public void OnTriggerStay2D(Collider2D col)
         * {
         * 
         * }
         */
        public void OnTriggerExit2D(Collider2D col);

        public bool CheckAvailable(Collider2D col);
    }
}