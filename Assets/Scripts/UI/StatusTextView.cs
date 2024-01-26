using TMPro;

namespace UI
{
    public class StatusTextView : UIView<StatusTextModel>
    {
        public TextMeshProUGUI DateText;
        public TextMeshProUGUI FundsText;

        public AudioEvent sfx_gained;
        public AudioEvent sfx_lost;

        public override void UpdateViewWithModel(StatusTextModel model)
        {
            DateText.text = model.DateText;
            FundsText.text = model.FundsText;
        }
    }
}