using UnityEngine;
using System.Collections;
using Receiver2;
using SimpleJSON;
using System;
namespace Bloons{
    public class GhostBalloon: BaseBalloon {
        

        public override void OnStart(ref Balloon instance)
        {
            if (instance == null || instance.IsPopped) {return;}
            instance.GetComponentInChildren<Renderer>().material.color = Color.white;
            instance.GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");
            instance.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
        }

        public override void GetPersistentData(ref Balloon instance,  ref JSONObject result){
            if (instance == null || instance.IsPopped) {return;}
            result.Add("type", new JSONString("ghost"));
        }
        public override void SetPersistentData(ref Balloon instance, JSONObject data) {
             if (data.HasKey("type") && (data["type"].Value == "ghost")) {
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
            QuickDrawListener qdl = lah.gameObject.GetComponent<QuickDrawListener>();
            if (qdl.quickDraw()) {
                GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.white * 2);
            } else {
                GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black );
            }
        }
        public override bool OnShoot(ref Balloon instance, ref ShootableQuery query) {
            LocalAimHandler lah = LocalAimHandler.player_instance;
            if (lah == null) {return false;}
            QuickDrawListener qdl = lah.gameObject.GetComponent<QuickDrawListener>();
            if (!instance.IsPopped ) {
                if ( qdl.quickDraw()) {
                    CustomPop(instance, query, 1, 4);
                    query.shot_response = ShotResponse.PassThroughAndHit;
                    query.hit_event = ShootableQuery.Event.HitBalloon;
                } else {
                    query.shot_response = ShotResponse.PassThrough;
        			query.hit_event = ShootableQuery.Event.Nothing;
                    
                    if (!qdl.ghost) {
                        ReceiverOnScreenMessage.QueueMessage("", "Draw and shoot quickly to break ghost balloons.", true);
                        qdl.ghost = true;
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