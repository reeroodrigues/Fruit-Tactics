using System.Collections.Generic;

namespace New_GameplayCore.Services
{
    public class SwapService : ISwapService
    {
        private readonly IHandService _hand;
        private readonly IDeckService _deck;
        private readonly ITimeManager _time;
        private readonly LevelConfigSO _cfg;
        private readonly DailyMissionsController _dailyMissions;

        public SwapService(IHandService hand, IDeckService deck, ITimeManager time, LevelConfigSO cfg,  DailyMissionsController dailyMissions)
        {
            _hand = hand;
            _deck = deck;
            _time = time;
            _cfg = cfg;
            _dailyMissions = dailyMissions;
        }

        public bool TrySwapAll()
        {
            if (!_time.CanPay(_cfg.swapAllTimePenalty))
                return false;

            int currentCount = _hand.Cards.Count;
            if (currentCount == 0)
                return false;

            _time.TryPay(_cfg.swapAllTimePenalty);
            
            var toDiscard = new List<CardInstance>(_hand.Cards);
            _hand.RemoveWhere(_ => true, toDiscard);
            _deck.DiscardMany(toDiscard);
            
            var newCards = new List<CardInstance>(currentCount);
            DrawUpTo(currentCount, newCards);
            
            _dailyMissions?.ReportSwapAll();
            
            _hand.AddMany(newCards);
            
            return newCards.Count > 0;
        }
        
        private void DrawUpTo(int target, IList<CardInstance> buffer)
        {
            int remaining = target;
            
            int drawn = _deck.DrawMany(remaining, buffer);
            remaining -= drawn;
            
            if (remaining > 0 && _cfg.allowEmptyDeckRefill && _deck.TryRefillFromDiscard())
            {
                drawn = _deck.DrawMany(remaining, buffer);
                remaining -= drawn;
            }
        }
        
        public bool TrySwapRandom()
        {
            if (!_time.CanPay(_cfg.swapRandomTimePenalty))
                return false;

            _time.TryPay(_cfg.swapRandomTimePenalty);

            if (_hand.Cards.Count == 0) return false;

            int idx = UnityEngine.Random.Range(0, _hand.Cards.Count);
            var toRemove = _hand.Cards[idx];
            _hand.TryRemove(toRemove);
            _deck.Discard(toRemove);
            
            _dailyMissions?.ReportSwapRandom();

            if (_deck.TryDraw(out var newCard))
                _hand.TryAdd(newCard);

            return true;
        }
    }
}