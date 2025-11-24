using System.Collections.Generic;
using System.Linq;
using Apis;

namespace Apis
{
    public enum GoldType
    {
        Gold,       // 원념
        LobbyGold   // 로비 거시기 재화
    }
    public class Factory_Gold : ItemFactory<Gold_Drop>
    {
        //아이템 픽업 팩토리
        
        
        
        readonly IDictionary<int, Gold_Drop> goldDict;

        private readonly SortedDictionary<int, int> _goldGroup;
        private readonly SortedDictionary<int, int> _lobbyGoldGroup;

        // private readonly int[] _inGameGoldGroups = new[] { 1, 3, 30, 300 }; 
        // private readonly int[] _lobbyGoldGroups = new[] { 1, 5, 20, 50 };

        private List<Gold_Drop> goldDropTemp;
        public Factory_Gold(Gold_Drop[] goldDrops) : base(goldDrops)
        {
            goldDict = new Dictionary<int, Gold_Drop>();
            _goldGroup = new();
            _lobbyGoldGroup = new();
            
            foreach (var x in goldDrops)
            {
                goldDict.TryAdd(x.goldId, x);
                if (x.goldType == GoldType.Gold)
                {
                    _goldGroup.TryAdd(x.value, x.goldId);
                }
                else
                {
                    _lobbyGoldGroup.TryAdd(x.value, x.goldId);
                }
                
            }
            goldDropTemp = new();
        }
        
       
        public override Gold_Drop CreateNew(int itemId)
        {
            if (goldDict.TryGetValue(itemId, out var value))
            {
                return pool.Get(value.name);
            }

            return null;
        }

        // private StringBuilder std = new StringBuilder();

        public override Gold_Drop[] CreateNews(int itemId, int amount)
        {
            goldDropTemp.Clear();
            // std.Clear();
            // std.Append(goldName);
            // std.Append(": ");
            int _amount = amount;
            if (itemId == Gold_Drop.Gold1Id)
            {
                for (int i = _goldGroup.Count-1; i >= 0; i--)
                {
                    int goldValue = _goldGroup.ElementAt(i).Key;
                    for (int j = 0; j < _amount / goldValue; j++)
                    {
                        Gold_Drop goldDrop = CreateNew(_goldGroup.ElementAt(i).Value);
                        if (!ReferenceEquals(goldDrop, null))
                        {
                            goldDropTemp.Add(goldDrop);
                        }
                    }

                    _amount %= goldValue;
                }
            }
            else
            {
                for (int i = _lobbyGoldGroup.Count-1; i >= 0; i--)
                {
                    int goldValue = _lobbyGoldGroup.ElementAt(i).Key;
                    for (int j = 0; j < _amount / goldValue; j++)
                    {
                        Gold_Drop goldDrop = CreateNew(_lobbyGoldGroup.ElementAt(i).Value);
                        if (!ReferenceEquals(goldDrop, null))
                        {
                            goldDropTemp.Add(goldDrop);
                        }
                    }

                    _amount %= goldValue;
                }
            }
            
            // Debug.LogError(std.ToString());

            return goldDropTemp.ToArray();
        }

        public override Gold_Drop CreateRandom()
        {
            // 사용하는 경우 없음
            return null;
        }

        public override List<Gold_Drop> CreateAll()
        {
            // 사용하는 경우 없음.
            return null;
        }
    }
}