using UnityEngine;
using Receiver2;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Bloons{
    public class BlinkBalloon: BaseBalloon {

		private float next_time = 0f;

		private bool on = false;
        public override void OnStart(ref Balloon instance)
        {
            if (instance == null || instance.IsPopped) {return;}
			next_time = Time.time + 0.5f;
			on = false;
            instance.GetComponentInChildren<Renderer>().material.color = Color.yellow;
            instance.GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");
            instance.GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black);


        }
		public void Update() {
			
            if (Time.frameCount % 5 != 0)
            {
                return;
            }
            
            Balloon parent = GetComponentInParent<Balloon>();
            if (parent == null || parent.IsPopped) {return;}
            if (on) {
                GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.yellow * 2);
            } else {
                GetComponentInChildren<Renderer>().material.SetColor("_EmissionColor", Color.black );
            }
			if (Time.time < next_time) {
				return;
			}
			on = !on;
			next_time = Time.time + (on ? 0.5f : UnityEngine.Random.Range(1.0f, 2.0f));
		}
        public override bool OnShoot(ref Balloon instance, ref ShootableQuery query) {
            if (!instance.IsPopped) {
                PopAlarm(instance, query);
				query.shot_response = ShotResponse.PassThroughAndHit;
				query.hit_event = ShootableQuery.Event.HitBalloon;
            }
            return false;
        }
        public override void GetPersistentData(ref Balloon instance,  ref JSONObject result){
            if (instance == null || instance.IsPopped) {return;}
            result.Add("type", new JSONString("blink"));
        }
        public override void SetPersistentData(ref Balloon instance, JSONObject data) {
            if (data.HasKey("type") && (data["type"].Value == "blink")) {
                instance.gameObject.AddComponent<BlinkBalloon>();
            }
        }
        public override bool OnTouch(ref Balloon instance, Collider other) {
            PopAlarm(instance, null);
            return false;
        }

        private void PopAlarm(Balloon instance, ShootableQuery query){
			if (!on) {
	            CustomPop(instance, query, 0, 1);
			} else {
	            CustomPop(instance, query, 1, 4);
			}
        }
	}
}