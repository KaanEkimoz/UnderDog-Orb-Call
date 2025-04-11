using UnityEngine;

namespace com.game
{
    public class LightController : MonoBehaviour
    {
        [SerializeField] Light baseLight;
        [SerializeField] Light enemyLight;
        [Range(0f,30f)] 
        [SerializeField] float value = 1f;

        [SerializeField] float rangePerValue = 1f;
        [SerializeField] float intensityPerValue = 1f;

        [SerializeField] float baseLightAddition = 2f;

        private void OnValidate()
        {
            UpdadeValue();
        }
        private void Update()
        {
            
        }

        public void UpdadeValue()
        {
    

            baseLight.intensity = baseLightAddition + value * intensityPerValue;
            baseLight.range = baseLightAddition + value * rangePerValue;

            enemyLight.intensity = value * intensityPerValue;
            enemyLight.range = value * rangePerValue;
        }
    }
}
