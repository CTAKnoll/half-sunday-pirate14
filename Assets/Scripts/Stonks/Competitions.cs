using Plants;
using Services;
using Utils;
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

        public void RunCompetition(TulipVarietal playerSubmission, bool competitorsCanBringNew)
        {
            LastCompetitionResults = new CompetitionResults();
            
            TulipVarietal randomOne = TulipVarietal.GetRandomTulipVarietal(competitorsCanBringNew);
            TulipVarietal randomTwo = TulipVarietal.GetRandomTulipVarietal(competitorsCanBringNew);

            float priceOne = Economy.GetCurrentPrice(playerSubmission);
            float priceTwo = Economy.GetCurrentPrice(randomOne);
            float priceThree = Economy.GetCurrentPrice(randomTwo);
            float normalized = priceOne + priceTwo + priceThree;

            float chanceOne = priceOne / normalized;
            float chanceTwo = priceTwo / normalized;
            float chanceThree = priceThree / normalized;
            
            float winner = FloatExtensions.RandomBetween(0f, 1f);
            float second = FloatExtensions.RandomBetween(0f, 1f);
            if (winner < chanceOne)
            {
                LastCompetitionResults.FirstPlace = playerSubmission;
                if (second < chanceTwo / (chanceTwo + chanceThree))
                {
                    LastCompetitionResults.SecondPlace = randomOne;
                    LastCompetitionResults.ThirdPlace = randomTwo;
                    LastCompetitionResults.PlayerPlacement = 1;
                    LastCompetitionResults.PlayerPayout = 4 * Economy.GetAveragePrice();
                }
                else
                {
                    LastCompetitionResults.SecondPlace = randomTwo;
                    LastCompetitionResults.ThirdPlace = randomOne;
                    LastCompetitionResults.PlayerPlacement = 1;
                    LastCompetitionResults.PlayerPayout = 4 * Economy.GetAveragePrice();
                }
            }
            else if (winner < chanceOne + chanceTwo)
            {
                LastCompetitionResults.FirstPlace = randomOne;
                if (second < chanceOne / (chanceOne + chanceThree))
                {
                    LastCompetitionResults.SecondPlace = playerSubmission;
                    LastCompetitionResults.ThirdPlace = randomTwo;
                    LastCompetitionResults.PlayerPlacement = 2;
                    LastCompetitionResults.PlayerPayout = Economy.GetAveragePrice();
                }
                else
                {
                    LastCompetitionResults.SecondPlace = randomTwo;
                    LastCompetitionResults.ThirdPlace = playerSubmission;
                    LastCompetitionResults.PlayerPlacement = 3;
                    LastCompetitionResults.PlayerPayout = 0;
                }
            }
            else
            {
                LastCompetitionResults.FirstPlace = randomTwo;
                if (second < chanceOne / (chanceOne + chanceTwo))
                {
                    LastCompetitionResults.SecondPlace = playerSubmission;
                    LastCompetitionResults.ThirdPlace = randomOne;
                    LastCompetitionResults.PlayerPlacement = 2;
                    LastCompetitionResults.PlayerPayout = Economy.GetAveragePrice();
                }
                else
                {
                    LastCompetitionResults.SecondPlace = randomOne;
                    LastCompetitionResults.ThirdPlace = playerSubmission;
                    LastCompetitionResults.PlayerPlacement = 3;
                    LastCompetitionResults.PlayerPayout = 0;
                }
            }
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