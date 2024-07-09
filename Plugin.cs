using BepInEx;
using BoplFixedMath;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace ExplosiveArrows
{
    [BepInPlugin("com.PizzaMan730.ExplosiveArrows", "ExplosiveArrows", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("ExplosiveArrows has loaded!");

            Harmony harmony = new Harmony("com.PizzaMan730.ExplosiveArrows");


            MethodInfo original = AccessTools.Method(typeof(Arrow), "OnCollide");
            MethodInfo patch = AccessTools.Method(typeof(myPatches), "OnCollide_Patch");
            harmony.Patch(original, new HarmonyMethod(patch));
        }

        public class myPatches
        {
            public static bool OnCollide_Patch(ref Arrow __instance, ref FixTransform ___fixTrans, ref DPhysicsCircle ___hitbox, CollisionInformation collision)
            {
                if (collision.layer != LayerMask.NameToLayer("wall")) {return true;}


                Explosion prefab = new Explosion();
                Explosion[] explosions = Resources.FindObjectsOfTypeAll(typeof(Explosion)) as Explosion[];
                foreach (Explosion obj in explosions)
                {
                    if (obj.name == "Explosion")
                    {
                        prefab = obj;
                    }
                }

                Explosion explosion = FixTransform.InstantiateFixed<Explosion>(prefab, ___fixTrans.position);
	            explosion.GetComponent<IPhysicsCollider>().Scale = ___hitbox.Scale;
	            explosion.PlayerOwnerId = 255;
                AudioManager.Get().Play("explosion");
                return true;
            }
        }
    }
}
//          dotnet build "C:\Users\ajarc\BoplMods\ExplosiveArrows\ExplosiveArrows.csproj"