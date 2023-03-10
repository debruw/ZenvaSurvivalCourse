using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ZenvaSurvival.Player
{
    public class PlayerNeeds : MonoBehaviour, IDamagable
    {
        public Need health, hunger, thirst, sleep;

        public float noHungerhealthDecay, noThirstHealthDecay;

        public UnityEvent onTakeDamage;

        private void Start()
        {
            //set start values
            health.curValue = health.startValue;
            hunger.curValue = hunger.startValue;
            thirst.curValue = thirst.startValue;
            sleep.curValue = sleep.startValue;
        }

        private void Update()
        {
            //decay need over time
            hunger.Subtract(hunger.decayRate * Time.deltaTime);
            thirst.Subtract(thirst.decayRate * Time.deltaTime);
            sleep.Add(sleep.regenRate * Time.deltaTime);

            if (hunger.curValue == 0)
                health.Subtract(noHungerhealthDecay * Time.deltaTime);
            if (thirst.curValue == 0)
                health.Subtract(noThirstHealthDecay * Time.deltaTime);

            if (health.curValue == 0)
            {
                Die();
            }

            //update ui bars
            health.uiBar.fillAmount = health.GetPercentage();
            hunger.uiBar.fillAmount = hunger.GetPercentage();
            thirst.uiBar.fillAmount = thirst.GetPercentage();
            sleep.uiBar.fillAmount = sleep.GetPercentage();
        }

        public void Heal(float amount)
        {
            health.Add(amount);
        }
        
        public void Eat(float amount)
        {
            hunger.Add(amount);
        }
        
        public void Drink(float amount)
        {
            thirst.Add(amount);
        }
        
        public void Sleep(float amount)
        {
            sleep.Subtract(amount);
        }

        public void TakePhysicalDamage(int amount)
        {
            health.Subtract(amount);
            onTakeDamage?.Invoke();
        }

        public void Die()
        {
            Debug.Log("Player is dead");
        }
    }

    [System.Serializable]
    public class Need
    {
        [HideInInspector] public float curValue;
        public float maxValue;
        public float startValue;
        public float regenRate;
        public float decayRate;
        public Image uiBar;

        public void Add(float amount)
        {
            curValue = Mathf.Min(curValue + amount, maxValue);
        }

        public void Subtract(float amount)
        {
            curValue = Mathf.Max(curValue - amount, 0);
        }

        public float GetPercentage()
        {
            return curValue / maxValue;
        }
    }

    public interface IDamagable
    {
        void TakePhysicalDamage(int damageAmount);
    }
}