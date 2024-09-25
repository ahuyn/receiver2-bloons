using UnityEngine;
using Receiver2;
using SimpleJSON;
namespace Bloons{
    public abstract class BaseBalloon: MonoBehaviour {
        public abstract bool OnShoot(ref Balloon instance, ref ShootableQuery query);
        public abstract bool OnTouch(ref Balloon instance, Collider collider);
        public virtual void OnStart(ref Balloon instance) {}
        public void Start() {
            Balloon parent = GetComponentInParent<Balloon>();
            OnStart(ref parent);
            
        }

        public virtual void GetPersistentData(ref Balloon instance,  ref JSONObject result){
        }
        public virtual void SetPersistentData(ref Balloon instance,  JSONObject data){}
        public static void CustomPop(Balloon __instance, ShootableQuery shootable_query, int min, int max) {

            if (__instance.IsPopped)
            {
                return;
            }

            int num = UnityEngine.Random.Range(min, max);
            if (ReceiverCoreScript.Instance().RoundPrefab != null)
            {
                for (int i = 0; i < num; i++)
                {
                    ShellCasingScript component = Instantiate(ReceiverCoreScript.Instance().RoundPrefab).GetComponent<ShellCasingScript>();
                    component.transform.position = __instance.transform.position + UnityEngine.Random.insideUnitSphere * 0.1f;
                    component.transform.rotation = UnityEngine.Random.rotationUniform;
                    component.item_owner_id = -1;
                    component.GetComponent<InventoryItem>().Move(null);
                    if (ReceiverCoreScript.Instance().IsPlayerAlive())
                    {
                        LocalAimHandler.player_instance.PickUpBullet(component);
                    }
                }
            }

            AudioManager.PlayOneShot3D(__instance.balloon_pop, __instance.transform.position);
            ReceiverEvents.TriggerEvent(new EntityDestroyed
            {
                shootable_query = shootable_query,
                entity_type = ReceiverEntityType.Balloon,
                game_object = __instance.gameObject
            });
            Instantiate(__instance.pop_prefab, __instance.transform.position, __instance.transform.rotation);
            __instance.SetPopped();
        }
    }
}