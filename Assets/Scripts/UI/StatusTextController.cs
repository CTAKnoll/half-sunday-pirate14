﻿using System;
namespace UI
{
    public class StatusTextController : UIController<StatusTextView, StatusTextModel>
    {
        
        public StatusTextController(StatusTextView view) : base(view)
        {
            Model.DateText = Timeline.Now.ToString("dd MMMM, yyyy");
            Model.FundsText = "$" + Economy.Funds;
            Timeline.DateChanged += UpdateDate;
            Economy.FundsChanged += UpdateFunds;
        }

        private void UpdateDate(DateTime newDate)
        {
            Model.DateText = newDate.ToString("dd MMMM, yyyy");
            UpdateViewAtEndOfFrame();
        }

        private void UpdateFunds(float newFunds)
        {
            float.TryParse(Model.FundsText.Substring(1), out float oldFunds);
            if(newFunds > oldFunds)
                Audio.PlayOneShot(View.sfx_gained);

            if(oldFunds > newFunds)
                Audio.PlayOneShot(View.sfx_lost);


            Model.FundsText = "$" +  newFunds.ToString("N2");

            UpdateViewAtEndOfFrame();
        }
    }
}