using System.Collections.Generic;
using Save.Schema;

public class ConvenienceFunctions
{
    static Dictionary<int, List<IConvenience>> dict;

    public static Dictionary<int, List<IConvenience>> Dict
    {
        get
        {
            if (dict == null)
            {
                dict = new();
                dict.TryAdd(1000, new ());
                dict.TryAdd(1001, new ());
                dict.TryAdd(1002, new ());
                dict.TryAdd(1003, new ());
                dict.TryAdd(1010, new ());
                dict.TryAdd(1011, new ());
                dict.TryAdd(1012, new ());
                dict.TryAdd(1020, new ());
                dict.TryAdd(1030, new ());
                dict.TryAdd(1031, new ());
                dict.TryAdd(1040, new ());
                dict.TryAdd(1041, new ());
                dict.TryAdd(1042, new ());
                dict.TryAdd(1050, new ());
                dict.TryAdd(1060, new ());
                dict.TryAdd(1061, new ());
                dict.TryAdd(1070, new ());
                dict[1000].Add(new JumpMax(LobbyDatabase.convenienceDict[1000].amounts[0]));
                dict[1001].Add( new UnLock(201));
                dict[1002].Add( new UnLock(202));
                dict[1003].Add( new UnLock(203));
                dict[1010].Add( new StartSoul(LobbyDatabase.convenienceDict[1010].amounts[0]));
                dict[1011].Add( new StartSoul(LobbyDatabase.convenienceDict[1011].amounts[0]));
                dict[1012].Add( new StartSoul(LobbyDatabase.convenienceDict[1012].amounts[0]));
                dict[1020].Add( new UnLock(101));
                dict[1030].Add( new PotionCount(LobbyDatabase.convenienceDict[1030].amounts[0]));
                dict[1031].Add( new PotionCount(LobbyDatabase.convenienceDict[1031].amounts[0]));
                dict[1040].Add( new ShopDiscount(LobbyDatabase.convenienceDict[1040].amounts[0]));
                dict[1041].Add( new ShopDiscount(LobbyDatabase.convenienceDict[1041].amounts[0]));
                dict[1042].Add( new ShopDiscount(LobbyDatabase.convenienceDict[1042].amounts[0]));
                dict[1050].Add(new UnLock(301));
                dict[1060].Add(new UnLock(401));
                dict[1060].Add(new EnhanceWellOfWarmth(LobbyDatabase.convenienceDict[1060].amounts[0]));
                dict[1061].Add(new EnhanceWellOfWarmth(LobbyDatabase.convenienceDict[1061].amounts[0]));
                dict[1070].Add(new UnLock(501));
                
            }

            return dict;
        }
    }
    
    public class JumpMax : IConvenience
    {
        private int amount = 0;
        public JumpMax(int amount)
        {
            this.amount = amount;
        }
        public void Apply()
        {
            GameManager.Save.currentSlotData.GrowthSaveData.Player.playerStat.jumpMax += amount;
            GameManager.instance.Player.playerStat.JumpMax += amount;
        }
    }

    public class StartSoul : IConvenience
    {
        private int amount;

        public StartSoul(int amount)
        {
            this.amount = amount;
        }
        public void Apply()
        {
            GameManager.instance.Soul += amount;
        }
    }
    
    public class PotionCount : IConvenience
    {
        private int amount;

        public PotionCount(int amount)
        {
            this.amount = amount;
        }
        public void Apply()
        {
            GameManager.instance.Player.IncreaseMaxPotion(amount);
            GameManager.Save.currentSlotData.GrowthSaveData.Player.playerStat.potionCount += amount;
            GameManager.instance.Player.increasePotionCapacity(amount);
        }
    }

    public class ShopDiscount : IConvenience
    {
        private readonly float amount;
        public ShopDiscount(float amount)
        {
            this.amount = amount;
        }
        public void Apply()
        {
            FormulaConfig.shopDiscount += amount / 100f;
        }
    }
    public class UnLock : IConvenience
    {
        private int idx;
        public UnLock(int index)
        {
            idx = index;
        }
        public void Apply()
        {
            DataAccess.LobbyData.UnLock(idx);
        }
    }

    public class EnhanceWellOfWarmth : IConvenience
    {
        private int amount;

        public EnhanceWellOfWarmth(int amount)
        {
            this.amount = amount;
        }
        public void Apply()
        {
            GameManager.Save.currentSlotData.GrowthSaveData.ObjectData.wellOfWarmthCount += amount;
        }
    }
}
