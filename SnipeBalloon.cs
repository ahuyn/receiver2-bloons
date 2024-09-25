using UnityEngine;
using System.Collections;
using Receiver2;
using SimpleJSON;
using System;
namespace Bloons{
    public class SnipeBalloon: BaseBalloon {
        

        public override void OnStart(ref Balloon instance)
        {
            if (instance == null || instance.IsPopped) {return;}
            instance.GetComponentInChildren<Renderer>().material.color = Color.green;
            instance.GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");
        }

        public override void GetPersistentData(ref Balloon instance,  ref JSONObject result){
            if (instance == null || instance.IsPopped) {return;}
            result.Add("type", new JSONString("snipe"));
        }
        public override void SetPersistentData(ref Balloon instance, JSONObject data) {
             if (data.HasKey("type") && (data["type"].Value == "snipe")) {
                instance.gameObject.AddComponent<GhostBalloon>();
            }
        }
        public void Update() {
            
            if (Time.frameCount % 5 != 0)
            {
                return;
            }
            Balloon parent = GetComponentInParent<Balloon>();
            if (parent == null || parent.IsPopped) {return;}
            LocalAimHandler lah = LocalAimHandler.player_instance;
            if (lah == null) {return;}

            if (Vector3.Distance(parent.transform.position, lah.gameObject.transform.position) > 12f) {
                GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.green * 2);
            } else {
                GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black );
            }
        }
        public override bool OnShoot(ref Balloon instance, ref ShootableQuery query) {
            LocalAimHandler lah = LocalAimHandler.player_instance;
            if (lah == null) {return false;}
            QuickDrawListener qdl = lah.gameObject.GetComponent<QuickDrawListener>();
            if (!instance.IsPopped ) {
                if (Vector3.Distance(instance.transform.position, lah.gameObject.transform.position) > 12f){
                    CustomPop(instance, query, 1, 4);
                    query.shot_response = ShotResponse.PassThroughAndHit;
                    query.hit_event = ShootableQuery.Event.HitBalloon;
                } else {
                    query.shot_response = ShotResponse.PassThrough;
        			query.hit_event = ShootableQuery.Event.Nothing;
                    if (!qdl.snipe) {
                        ReceiverOnScreenMessage.QueueMessage("", "Stand far enough away until the balloon glows.", true);
                        qdl.snipe = true;
                    }
                }
            }
            return false;
        }

        
        public override bool OnTouch(ref Balloon instance, Collider other) {
            return false;
        }
    }
}