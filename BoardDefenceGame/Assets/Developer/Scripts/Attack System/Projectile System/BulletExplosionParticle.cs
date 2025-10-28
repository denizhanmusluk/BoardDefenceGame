using EZ_Pooling;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class BulletExplosionParticle : MonoBehaviour
{
    public ParticleSystem expParticle;
    private void Despawn() => EZ_PoolManager.Despawn(this.transform);

    public void ParticlePlay()
    {
        expParticle.Play();
        StartCoroutine(Despawning());
    }
    IEnumerator Despawning()
    {
        yield return new WaitForSeconds(1f);
        Despawn();
    }
}
