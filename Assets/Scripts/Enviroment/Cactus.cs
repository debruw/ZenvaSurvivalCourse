using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZenvaSurvival.Player;

namespace ZenvaSurvival.Enviroment
{
    public class Cactus : MonoBehaviour
    {
        public int damage;
        public float damageRate;

        private List<IDamagable> thingsToDamage = new List<IDamagable>();

        private void Start()
        {
            StartCoroutine(DealDamage());
        }

        IEnumerator DealDamage()
        {
            while (true)
            {
                for (int i = 0; i < thingsToDamage.Count; i++)
                {
                    thingsToDamage[i].TakePhysicalDamage(damage);
                }

                yield return new WaitForSeconds(damageRate);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IDamagable damagable))
            {
                thingsToDamage.Add(damagable);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IDamagable damagable))
            {
                thingsToDamage.Remove(damagable);
            }
        }
    }
}