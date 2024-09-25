using UnityEngine;
using Receiver2;
using System.Collections;
using SimpleJSON;
using System.Collections.Generic;

namespace Bloons{
    public class SpawnBalloon: BaseBalloon {

        public override void OnStart(ref Balloon instance)
        {
            if (instance == null || instance.IsPopped) {return;}
            instance.GetComponentInChildren<Renderer>().material.color = new Color(1f, 0f, 1f);
        }

        public override bool OnShoot(ref Balloon instance, ref ShootableQuery query) {
            if (!instance.IsPopped) {
                PopSpawn(instance, query);
				query.shot_response = ShotResponse.PassThroughAndHit;
				query.hit_event = ShootableQuery.Event.HitBalloon;
            }
            return false;
        }
        public override void GetPersistentData(ref Balloon instance,  ref JSONObject result){
            if (instance == null || instance.IsPopped) {return;}
            result.Add("type", new JSONString("spawn"));
        }
        public override void SetPersistentData(ref Balloon instance, JSONObject data) {
            if (data.HasKey("type") && (data["type"].Value == "spawn")) {
                instance.gameObject.AddComponent<SpawnBalloon>();
            }
        }
        public override bool OnTouch(ref Balloon instance, Collider other) {
            PopSpawn(instance, null);
            return false;
        }

        private void PopSpawn(Balloon instance, ShootableQuery query){
            // ReceiverCoreScript.Instance().SpawnEnemyNearPlayer(SpawnType.STUN_DRONE, 0,  3f);
            // RuntimeTileLevelGenerator.instance.SpawnEnemyCloseToPlayer(SpawnType.STUN_DRONE, 0,  3f);
            TileEnemySpawn droneSpawn = new TileEnemySpawn();
            droneSpawn.spawn_type = SpawnType.STUN_DRONE;
            droneSpawn.offset = Vector3.up;
            GameObject drone = ReceiverCoreScript.Instance().SpawnStandaloneEnemy(instance.transform.position, droneSpawn);
            CustomPop(instance, query, 2, 6);
            IEnumerator routine = AlertDrone(drone);
            StartCoroutine(routine);
        }
        private IEnumerator AlertDrone(GameObject drone) {
            yield return new WaitForSeconds(0.01f);
            
            ShockDrone component2 = drone.GetComponent<ShockDrone>();
            CameraPart camera = ((component2 != null) ? component2.camera_part : null);
            if (camera != null) {
                camera.AlertImmediate();
            }
            yield return null;
        }
    }
}