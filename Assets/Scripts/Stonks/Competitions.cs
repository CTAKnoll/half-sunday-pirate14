using Plants;
using Services;
using Yarn.Unity;
using static Plants.TulipData;

namespace Stonks
{
    public class Competitions : IService
    {
        public static CompetitionResults LastCompetitionResults;
        public struct CompetitionResults
        {
            public TulipVarietal FirstPlace;
            public TulipVarietal SecondPlace;
            public TulipVarietal ThirdPlace;
            public int PlayerPlacement;
            public float PlayerPayout;
        }

        private Economy Economy;
        
        public Competitions()
        {
            Economy = ServiceLocator.LazyLoad<Economy>();
        }

        public void RunCompetition(bool competitorsCanBringNew)
        {
            LastCompetitionResults = new CompetitionResults
            {
                
            };
        }

        [YarnFunction("first_place_tulip")]
        public static string FirstPlace()
        {
            return LastCompetitionResults.FirstPlace.ToString();
        }
        
        [YarnFunction("second_place_tulip")]
        public static string SecondPlace()
        {
            return LastCompetitionResults.SecondPlace.ToString();
        }
        
        [YarnFunction("third_place_tulip")]
        public static string ThirdPlace()
        {
            return LastCompetitionResults.ThirdPlace.ToString();
        }
        
        [YarnFunction("get_player_place")]
        public static string PlayerPlace()
        {
            return LastCompetitionResults.PlayerPlacement.ToString();
        }
        
        [YarnFunction("get_player_payout")]
        public static string PlayerPayout()
        {
            return LastCompetitionResults.PlayerPayout.ToString();
        }

        [YarnCommand("pay_the_player")]
        public static void PayOutCompetition()
        {
            Economy econ = ServiceLocator.LazyLoad<Economy>();
            econ.WinCompetition(LastCompetitionResults);
        }
    }
}