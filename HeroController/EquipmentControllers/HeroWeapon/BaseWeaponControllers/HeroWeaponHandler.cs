using UnityEngine;

public class HeroWeaponHandler : MonoBehaviour
{
    [SerializeField] 
    private ParticleSystem _fireEffectParticleSystem;

    public void PlayFireEffect() => _fireEffectParticleSystem.Play();
}
