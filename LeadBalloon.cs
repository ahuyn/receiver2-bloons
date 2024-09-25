using UnityEngine;
using System.Collections;
using Receiver2;
using SimpleJSON;

namespace Bloons{
    public class LeadBalloon: BaseBalloon {
        private bool canBreak = false;
        private IEnumerator routine;

        public override void OnStart(ref Balloon instance)
        {
            if (instance == null || instance.IsPopped) {return;}
            instance.GetComponentInChildren<Renderer>().material.color = new Color(0.2f, 0.2f, 0.2f);
            instance.GetComponentInChildren<BallisticMaterialSpecial>().ballistic_properties.no_penetration = true;
            instance.transform.localScale = new Vector3(1.6f, 1.6f, 1.6f);
            instance.GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");
            instance.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black);
        }

        public override bool OnShoot(ref Balloon instance, ref ShootableQuery query) {
            if (!instance.IsPopped && query.hit_type == ShotEvent.Type.Entrance) {
                if (!canBreak) {
                    canBreak = true;
                    routine = clearShot(instance);
                    StartCoroutine(routine);
                    instance.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.gray * 2);
                    query.shot_response = ShotResponse.Destroy;
                    query.hit_event = ShootableQuery.Event.Nothing;
                } else {
                    StopCoroutine(routine);
                    canBreak = false;
                    CustomPop(instance, query, 2, 6);
                    query.shot_response = ShotResponse.PassThroughAndHit;
                    query.hit_event = ShootableQuery.Event.HitBalloon;
                }
            }
            return false;
        }
        public override void GetPersistentData(ref Balloon instance,  ref JSONObject result){
            if (instance == null || instance.IsPopped) {return;}
            result.Add("type", new JSONString("lead"));
        }
        public override void SetPersistentData(ref Balloon instance, JSONObject data) {
             if (data.HasKey("type") && (data["type"].Value == "lead")) {
                instance.gameObject.AddComponent<LeadBalloon>();
            }
        }
        public override bool OnTouch(ref Balloon instance, Collider other) {
            return false;
        }

        private IEnumerator clearShot(Balloon instance) {
            yield return new WaitForSeconds(1);
            Debug.Log(Time.time + " Reset!");
            canBreak = false;
            instance.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black);
            LocalAimHandler lah = LocalAimHandler.player_instance;
            if (lah != null) {
                QuickDrawListener qdl = lah.gameObject.GetComponent<QuickDrawListener>();
                if (!qdl.lead) {
                ReceiverOnScreenMessage.QueueMessage("", "Lead balloons require a double tap to break.", true);
                    qdl.lead = true;
                }
            }
            yield return null;
        }

    }
}