using UnityEngine;
using System.Collections;
using Receiver2;
using SimpleJSON;
namespace Bloons{
    public class QuickDrawListener: MonoBehaviour {
        private bool qd = false;

        public bool lead = false;
        public bool ghost = false;
        public bool snipe = false;
        public bool quickDraw() {
            return qd;
        }
        public void startQuickdraw() {
            if (qd) {return;}
            qd = true;
            StartCoroutine(endQuickdraw());
        }
        private IEnumerator endQuickdraw() {
            yield return new WaitForSeconds(2);
            qd = false;
            yield return null;
        }
     }
}