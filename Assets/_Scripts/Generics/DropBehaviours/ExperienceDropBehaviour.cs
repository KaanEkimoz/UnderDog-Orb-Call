using com.game.miscs;
using com.game.player;

namespace com.game.generics
{
    public class ExperienceDropBehaviour : DropBehaviour
    {
        public override bool DestroyOnGather => true;

        protected override void Start()
        {
            if (Amount <= 0)
            {
                Destroy();
                return;
            }

            base.Start();
        }

        protected override bool OnGather(IGatherer sender)
        {
            Player.Instance.Hub.Leveling.GainExperience(Amount);
            PopupManager.Instance.CreateExperiencePopup(Amount, transform.position);
            return true;
        }
    }
}
