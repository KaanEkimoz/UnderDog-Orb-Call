using UnityEngine;

namespace com.game.player
{
    public class PlayerParanoiaLogic : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] float m_currentPercentage;
        int m_currentSegment;

        public float TotalPercentage01 => m_currentPercentage;
        public float TotalPercentage => m_currentPercentage * 100f;
        public int SegmentIndex => m_currentSegment;
        public int Segment => m_currentSegment + 1;

        public bool Increase01(float percentage)
        {
            if (percentage < 0f)
                return Decrease01(percentage);

            return SetToPercentage01(m_currentPercentage + percentage);
        }

        public bool Decrease01(float percentage)
        {
            if (percentage < 0f)
                return Increase01(percentage);

            return SetToPercentage01(m_currentPercentage - percentage);
        }

        public bool SetToSegment(int segment)
        {
            int max = Constants.Paranoia.PARANOIA_SEGMENT_COUNT;

            if (segment < 1 || segment > max)
                return false;

            return SetToPercentage01(segment / max);
        }

        public bool SetToPercentage01(float percentage)
        {
            bool result = true;

            if (percentage < 0f)
            {
                percentage = 0f;
                result = false;
            }

            else if (percentage > 1f)
            {
                percentage = 1f;
                result = false;
            }

            m_currentPercentage = percentage;

            return result;
        }

        void FetchSegment()
        {
            m_currentSegment = Mathf.FloorToInt(m_currentPercentage * Constants.Paranoia.PARANOIA_SEGMENT_COUNT);
        }

        private void OnValidate()
        {
            FetchSegment();
        }
    }
}
