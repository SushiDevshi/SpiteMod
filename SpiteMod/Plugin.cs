using BepInEx;
using BepInEx.Configuration;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace SpiteMod
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.SushiDev.SpiteMod", "SpiteMod", "1.0.0")]
    public class Spite : BaseUnityPlugin
    {
        private static ConfigEntry<bool> BetterEffects;
        private static ConfigEntry<bool> Enabled;
        public void Awake()
        {
            {
                BetterEffects = Config.Bind(
                "Enable/Disable",
                "BetterEffects",
                true,
                "Whether or not the added effects are enabled. Turn off to disable the added effects."
                );
                Enabled = Config.Bind(
                "Enable/Disable",
                "Enabled",
                 true,
                "Whether or not the mod is enabled. Turn off to disable the mod."
                );
                GameObject FunBall = Resources.Load<GameObject>("Prefabs/Projectiles/Funball");
                if (BetterEffects.Value)
                {
                    ProjectileController FunballController = FunBall.GetComponent<ProjectileController>();
                    ProjectileFunballBehavior FunballBehavior = FunBall.GetComponent<ProjectileFunballBehavior>();
                    FunballBehavior.explosionPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/bellbodypartsimpact");
                    FunballController.ghostPrefab = Resources.Load<GameObject>("prefabs/projectileghosts/bellballghost");
                }
                if (Enabled.Value)
                {
                    On.RoR2.GlobalEventManager.OnCharacterDeath += delegate (On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
                    {
                        CharacterBody attackerBody = damageReport.attackerBody;
                        CharacterMaster attackerMaster = damageReport.attackerMaster;
                        TeamComponent teamComponent = damageReport.victimBody.teamComponent;
                        Inventory inventory = attackerMaster ? attackerMaster.inventory : null;
                        TeamIndex attackerTeamIndex = damageReport.attackerTeamIndex;
                        TeamIndex teamIndex = damageReport.victimTeamIndex;
                        if (teamComponent)
                        {
                            teamIndex = teamComponent.teamIndex;
                            if (teamIndex == TeamIndex.Monster)
                            {
                                //Debug.Log("funball OKAY");
                                HurtBoxGroup hurtBoxGroup = damageReport.victimBody.hurtBoxGroup;
                                if (hurtBoxGroup)
                                {
                                    //Debug.Log("victimHurtBoxGroup OK");
                                    float damage = 0f;
                                    if (damageReport.victimBody)
                                    {
                                        damage = damageReport.victimBody.damage;
                                    }
                                    HurtBoxGroup.VolumeDistribution volumeDistribution = hurtBoxGroup.GetVolumeDistribution();
                                    int num = Mathf.CeilToInt(volumeDistribution.totalVolume / 10f);
                                    Debug.LogFormat("bombCount={0}", new object[]
                                    {
                                 num
                                    });
                                    for (int i = 0; i < num; i++)
                                    {
                                        ProjectileManager.instance.FireProjectile(Resources.Load<GameObject>("Prefabs/Projectiles/Funball"), volumeDistribution.randomVolumePoint, Quaternion.identity, gameObject, damage, 700f, false, DamageColorIndex.Default, null, -1f);
                                    }
                                    orig(self, damageReport);
                                };
                            }
                        }
                    };
                }   
            }
        } 
    }
}
