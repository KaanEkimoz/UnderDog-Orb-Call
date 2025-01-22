using System;
using System.Collections;
using UnityEngine;

public class ExploderEnemy : Enemy
{
    [Header("Explosion Settings")]
    public float explosionDamage = 30f;
    public float preperationTime = 2f;
    public float explosionRadius = 5f;

    private bool isPreparingToExplode = false;
    private Coroutine preparationCoroutine = null;

    protected override void CustomUpdate()
    {
        if (CheckDistanceToPlayer()) //dusman karaktere yeterince yakinsa
        {
            if (isPreparingToExplode == false) //ve halihazirda patlamaya hazirlanmiyorsa
            {
                preparationCoroutine = StartCoroutine(PrepareToExplode()); //patlamaya hazirlan
            }
        }
        else //dusman karaktere yeterince yakin degilse
        {
            if (isPreparingToExplode == true) //ve halihazirda patlamaya hazirlaniyorsa
            {
                CancelPreparation(); //hazirligi iptal et
            }
        }
    } 

    private IEnumerator PrepareToExplode() 
    {
        isPreparingToExplode = true;
        float elapsedTime = 0f;

        while(elapsedTime < preperationTime) //patlama suresi dolmadigi surece
        {
            if (!CheckDistanceToPlayer()) //karakter dusmandan uzaklasirsa
            {
                isPreparingToExplode = false; //patlama hazirligini iptal et
                yield break;
            }

            elapsedTime += Time.deltaTime; //gecen zaman         
            yield return null;
        }

        Explode();
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider objectWithinRadius in colliders) //patlama yaricapi icersindeki tum colliderlarda
        {
            if (objectWithinRadius.CompareTag("Player")) //player tagli biri varsa
            {
                //HASAR VER 
            }
        }
        
        Destroy(gameObject);
    }

    private void CancelPreparation()
    {
        StopCoroutine(preparationCoroutine);
        preparationCoroutine = null;
        isPreparingToExplode = false;
    }
}
