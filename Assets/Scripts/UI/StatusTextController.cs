using System;
namespace UI
{
    public class StatusTextController : UIController<StatusTextView, StatusTextModel>
    {
        
        public StatusTextController(StatusTextView view) : base(view)
        {
            Model.DateText = Timeline.Now.ToLongDateString();
            Model.FundsText = "$" + Economy.Funds;
            Timeline.DateChanged += UpdateDate;
            Economy.FundsChanged += UpdateFunds;
        }

        private void UpdateDate(DateTime newDate)
        {
            Model.DateText = newDate.ToLongDateString();
            UpdateViewAtEndOfFrame();
        }

        private void UpdateFunds(float newFunds)
        {
            Model.FundsText = "$" + newFunds;
            UpdateViewAtEndOfFrame();
        }
    }
}