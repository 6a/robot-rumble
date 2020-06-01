using UnityEngine;
using UnityEngine.UI;
using RR.Views;
using RR.Facilitators.UI;

namespace RR.Properties
{
    public class NetPlayerProperties : BaseProperties
    {
        public Rigidbody Rigidbody;
        public PlayerCameraView PlayerCamera;
        public GameObject MinimapCamera;
        public GameObject MinimapEnemyIcon;
        public Transform HealthbarAnchor;
        public FloatingHealthBar HealthBarUI;
        public Animator Animator;
        public float MaxSpeed;
        public float AttackInputTimeBuffer;
        public Color[] PlayerColors;
        public SkinnedMeshRenderer SkinnedMeshRenderer;
        public float KillFloorY;
        public Collider WeaponCollider;
        public GameObject[] AttackParticles;
        public GameObject[] DeathParticles;
        public GameObject[] SpawnParticles;
        public AudioClip[] AttackSounds;
        public AudioClip[] DeathSounds;
        public AudioClip[] SpawnSounds;
    }
}
