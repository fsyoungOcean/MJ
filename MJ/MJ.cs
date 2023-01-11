using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Mahjong
{
    /// <summary>
    /// 玩法
    /// 十三張
    /// 十六張
    /// </summary>
    public enum MahjongGameType
    {
        e13 = 14,
        e16 = 17
    }
    public enum MJ
    {
        //萬 Charactor
        C1 = 1, C2 = 2, C3 = 3, C4 = 4, C5 = 5, C6 = 6, C7 = 7, C8 = 8, C9 = 9,
        //筒 Dot
        D1 = 11, D2 = 12, D3 = 13, D4 = 14, D5 = 15, D6 = 16, D7 = 17, D8 = 18, D9 = 19,
        //條 Bamboo
        B1 = 21, B2 = 22, B3 = 23, B4 = 24, B5 = 25, B6 = 26, B7 = 27, B8 = 28, B9 = 29,

        East = 31, South = 33, West = 35, North = 37,
        Red = 41, Green = 43, White = 45,
        F1 = 51, F2 = 52, F3 = 53, F4 = 54, F5 = 55, F6 = 56, F7 = 57, F8 = 58
    }

    public static class Globals
    {
        public const int THREE_CARD = 1;
        public const int STRAIGHT_CARD = 2;
        public const int PAIR_CARD = 3;
    }

    public class MJTileGourp
    {
        /// <summary>
        /// 牌搭
        /// </summary>
        /// <typeparam name="int">牌編號</typeparam>
        /// <typeparam name="MJTile">麻將牌</typeparam>
        /// <returns></returns>
        private Dictionary<int, MJTile> Map = new Dictionary<int, MJTile>();
        public MJTileGourp(List<int> tiles)
        {
            ///先整理牌
            tiles.Sort();

            foreach (var index in tiles)
            {
                if (Map.ContainsKey(index))
                {
                    Map[index].Add();
                }
                else
                {
                    Map.Add(index, new MJTile(index));
                }
            }
        }

        public List<MJTile> ToList()
        {
            return Map.Values.ToList();
        }
        /// <summary>
        /// 計算目前手邊有幾張
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                return Map.Values.Sum(item => item.Count);
            }
        }
    }

    /// <summary>
    /// 麻將牌資訊
    /// </summary>
    public class MJTile : ICloneable
    {
        /// <summary>
        /// 最多幾張牌
        /// 除了花牌一張外，其他都四張
        /// </summary>
        private readonly int Max;
        /// <summary>
        /// 麻將編號
        /// </summary>
        /// <value></value>
        public int Tile { get; private set; }
        /// <summary>
        /// 目前有幾張牌
        /// </summary>
        /// <value></value>
        public int Count { get; private set; }

        public MJTile(int tile)
        {
            Max = (tile >= (int)MJ.F1) ? 1 : 4;
            Tile = tile;
            Count = 1;
        }

        /// <summary>
        /// 計算牌搭
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Add(int count = 1)
        {
            if (Count + count > Max)
                throw new Exception($"{Tile} {Count} + 1 exceed the limit {Max}");
            Count += count;
            return Count;
        }

        /// <summary>
        /// 丟牌或門前牌
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public int Remove(int count = 1)
        {
            if (Count - count < 0)
                throw new Exception($"{Tile} {Count} - 1 out of range 0");
            Count -= count;
            return Count;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// 目前是否已達牌組上限
        /// </summary>
        /// <value></value>
        public bool IsMax
        {
            get
            {
                return Count == Max;
            }
        }
    }

    /// <summary>
    /// 表示門前牌的組合
    /// </summary>
    public class MJSetType
    {
        public List<int> Tiles { set; get; } = new List<int>();
        /// <summary>
        /// 1 : 刻
        /// 2 : 順
        /// </summary>
        public int Type { set; get; } = 0;
    }

    public class ListenState : IEquatable<ListenState>, IComparable<ListenState>
    {
        /// <summary>
        /// 判斷此手牌已經獲勝
        /// </summary>
        public bool Win { set; get; } = false;
        /// <summary>
        /// 需要移除的牌
        /// </summary>
        public int Remove { set; get; } = 0;
        /// <summary>
        /// 可以聽幾張牌
        /// </summary>
        public List<int> Listen { set; get; } = new List<int>();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"{Win}-");
            sb.Append($"{Remove}-");
            Listen.Sort();
            sb.Append($"{string.Join(",", Listen)}");
            return sb.ToString();
        }
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is ListenState))
                return false;
            else
                return base.Equals((ListenState)obj);
        }

        public int CompareTo(ListenState? other)
        {
            if (other == null)
                return 1;
            else
                return this.ToString().CompareTo(other.ToString());
        }

        public bool Equals(ListenState? other)
        {
            if (other == null) return false;
            return (this.ToString().Equals(other.ToString()));
        }
    }

    public class TableInfo
    {
        //莊家是誰
        public int Dealer;
        //玩家目前手牌數量
        public int TileNum;
        public int LastCardOwner;

        /// <summary>
        /// 玩家前一次動作
        /// 0 丟牌 1 吃 2 碰3 明槓
        /// 4 暗槓 5 加槓 6 補花
        /// </summary>
        public int PreAction;

        /// <summary>
        /// 目前剩餘牌數量
        /// </summary>
        public int RemainTileCount;
        /// <summary>
        /// 風圈
        /// </summary>
        public int Wind;
        /// <summary>
        /// 胡牌玩家風位
        /// </summary>
        public int DoorWind;
        /// <summary>
        /// 贏牌玩家
        /// </summary>
        public int WinPlayer;

        //int cont_dealer;
        /// <summary>
        /// 底
        /// </summary>
        public int Base_value;
        /// <summary>
        /// 台
        /// </summary>
        public int Tai_value;
    }


    /// <summary>
    /// 門前牌資訊
    /// </summary>
    public class TileType
    {
        /// <summary>
        /// 是不是暗槓
        /// </summary>
        public bool ConcealedKong{ set; get; } = false;
        /// <summary>
        /// 牌組
        /// </summary>
        public List<int> Tiles { set; get; } = new List<int>();
        /// <summary>
        /// 0 : 花
        /// 1 : 刻
        /// 2 : 順
        /// 3 : 對
        /// </summary>
        public int Type { set; get; } = -1;
    };

    public class TilesTaiRequest
    {
        /// <summary>
        /// 門前牌
        /// </summary>
        public List<TileType> Grounded { set; get; } = new List<TileType>();
        /// <summary>
        /// 手牌
        /// </summary>
        public List<TileType> Hand { set; get; } = new List<TileType>();
        /// <summary>
        /// 最後一張是甚麼牌
        /// </summary>
        public int Last;
        /// <summary>
        /// 胡牌時，手牌的數量
        /// </summary>
        public int HandCount;
    }

    public interface IMahjongAction
    {
        /// <summary>
        /// 判斷目前手牌是否移除某張牌就可以聽牌
        /// </summary>
        /// <param name="grounded">門前牌</param>
        /// <param name="hand">目前手牌</param>
        /// <returns></returns>
        public List<ListenState> CheckListen(List<int> grounded, List<int> hand);

        /// <summary>
        /// 判斷目前牌組是否聽牌
        /// </summary>
        /// <param name="lists">目前牌組</param>
        /// <returns>聽牌牌組</returns>
        public List<int> ShowListenTile(List<MJTile> lists, List<int>? checklistenList = null);

        /// <summary>
        /// 判斷目前牌組是否胡牌
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public bool IsWin(List<MJTile> lists);

        /// <summary>
        /// 找到眼牌
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        public List<int> FindPairs(List<MJTile> lists);
    }

    public class MahjongAction : IMahjongAction
    {
        private const int Pair = 2;
        private const int Set = 3;

        private MahjongGameType mahjongGameType;

        public MahjongAction(MahjongGameType type)
        {
            mahjongGameType = type;
        }

        /// <summary>
        /// 根據目前的牌組，判斷是否已經胡牌
        /// 或是丟哪張牌，可以聽哪些牌
        /// </summary>
        /// <param name="grounded">門前牌</param>
        /// <param name="hand">手牌</param>
        /// <returns>聽牌狀況</returns>
        public List<ListenState> CheckListen(List<int> grounded, List<int> hand)
        {
            var groundMap = (new MJTileGourp(grounded)).ToList();
            var handMap = (new MJTileGourp(hand)).ToList();


            bool win = IsWin(handMap);
            if (win)
            {
                return new List<ListenState> { new ListenState { Win = true } };
            }
            else
            {
                var canListenlist = Enum.GetValues(typeof(MJ))
                    .Cast<int>().ToList().FindAll(item => item < (int)MJ.F1);

                //移除已經四張的牌組
                var fullTile = groundMap.FindAll(g => g.IsMax);
                if (fullTile.Count > 0)
                    canListenlist.RemoveAll(item => fullTile.FindIndex(index => index.Tile == item) != -1);

                var listenStatelist = new List<ListenState>();
                foreach (var item in handMap)
                {
                    var list = handMap.Clone().ToList();
                    var index = list.FindIndex(index => index.Tile == item.Tile);

                    //判斷將哪張牌，可以聽哪些牌
                    var tileIndex = list[index].Tile;
                    list[index].Remove();

                    var listenlist = this.ShowListenTile(list, canListenlist);
                    if (listenlist.Count > 0)
                    {
                        listenStatelist.Add(new ListenState { Win = false, Remove = tileIndex, Listen = listenlist });
                    }
                }

                return listenStatelist;
            }

        }

        /// <summary>
        /// 檢查聽了哪幾張牌
        /// </summary>
        /// <param name="lists">原本的牌組</param>
        /// <param name="checklistenList">要檢查的牌，若為null，就檢查全部</param>
        /// <returns>聽的牌</returns>
        public List<int> ShowListenTile(List<MJTile> lists, List<int>? checklistenList = null)
        {
            var mjlist = new List<int>();

            if (checklistenList == null)
            {
                //將全部的牌都塞入，一一作檢查
                mjlist = Enum.GetValues(typeof(MJ))
                .Cast<int>().ToList().FindAll(item => item < (int)MJ.F1);
            }
            else
            {
                mjlist = checklistenList;
            }

            List<int> listenList = new List<int>();

            foreach (var tile in mjlist)
            {
                var check = lists.Clone().ToList();
                var index = check.FindIndex(item => item.Tile == tile);
                //已經槓的牌，不會拿來胡牌
                if (index != -1 && check[index].IsMax)
                    continue;

                if (index == -1)
                    check.Add(new MJTile(tile));
                else
                    check[index].Add();

                check.Sort((a, b) => a.Tile - b.Tile);
                if (IsWin(check))
                    listenList.Add(tile);
            }

            return listenList;
        }

        public bool IsWin(List<MJTile> lists)
        {
            return CheckNormal(lists);
        }

        /// <summary>
        /// 判斷一般牌型
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        private bool CheckNormal(List<MJTile> lists)
        {
            //一般牌型判斷
            try
            {
                var pairList = FindPairs(lists);
                if (pairList.Count == 0)
                    return false;

                for (int i = 0; i < pairList.Count; i++)
                {
                    var tileList = lists.Clone().ToList();
                    //找尋眼牌位置
                    var index = tileList.FindIndex(item => item.Tile == pairList[i] && item.Count >= Pair);
                    //找不到眼牌 有問題
                    if (index == -1)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (var item in tileList)
                        {
                            sb.Append($"{item.Tile} {item.Count}/ ");
                        }
                        throw new Exception($"Find pair {pairList[i]} in {sb} Not Found");
                    }
                    //移除眼牌兩張
                    tileList[index].Remove(Pair);
                    tileList.RemoveAll(item => item.Count == 0);
                    var win = CheckWin(tileList);
                    if (win)
                        return true;
                }

            }
            catch
            {
                throw;
            }

            return false;
        }


        /// <summary>
        /// 判斷眼牌
        /// </summary>
        /// <param name="lists">目前牌組</param>
        /// <returns>可以當眼牌的牌</returns>
        public List<int> FindPairs(List<MJTile> lists)
        {
            //字牌
            var honorLists = lists.FindAll(item => item.Tile >= (int)MJ.East);
            //萬筒條
            var suitLists = lists.FindAll(item => item.Tile < (int)MJ.East);
            bool hasHonorPair = false;
            List<int> pairList = new List<int>();
            //判斷字牌當眼時的狀況
            var pairCount = honorLists.Count(item => item.Count == Pair);
            if (pairCount == 0 || pairCount == 1)
            {
                hasHonorPair = (pairCount == 1);
                if (hasHonorPair)
                {
                    var honorPair = honorLists.Find(item => item.Count == Pair);
                    if (honorPair != null)
                    {
                        pairList.Add(honorPair.Tile);
                        honorLists.RemoveAll(item => item.Count == Pair);
                    }

                }

                var honorSet = honorLists.TrueForAll(item => item.Count % Set == 0);
                //剩下的字牌都沒有成刻
                if (honorSet == false)
                    return new List<int>();

                //分成萬筒條
                var suitGroup = new List<List<MJTile>>
                {
                    suitLists.FindAll(item => item.Tile >= (int)MJ.C1 && item.Tile <= (int)MJ.C9),
                    suitLists.FindAll(item => item.Tile >= (int)MJ.D1 && item.Tile <= (int)MJ.D9),
                    suitLists.FindAll(item => item.Tile >= (int)MJ.B1 && item.Tile <= (int)MJ.B9)
                };

                //字牌眼成立後 萬筒條個別數量應該為3的倍數
                if (hasHonorPair && suitGroup.TrueForAll(suit => suit.Sum(item => item.Count) % Set == 0) == false)
                    return new List<int>();

                //將牌組分成 147 258 369 3組
                //根據此方法找眼
                //一副牌，依一四七、二五八、三六九分成三堆，每堆的張數除以三的餘數必有一個與另兩個不同，則眼睛就在不同的那堆裡。
                List<List<int>> groupCheck = new List<List<int>>();
                foreach (var group in suitGroup)
                {
                    var g147 = group.FindAll(item => (item.Tile % 10) % Set == 1).Sum(c => c.Count) % 3;
                    var g258 = group.FindAll(item => (item.Tile % 10) % Set == 2).Sum(c => c.Count) % 3;
                    var g369 = group.FindAll(item => (item.Tile % 10) % Set == 0).Sum(c => c.Count) % 3;
                    var list = new List<int> { g147, g258, g369 };
                    groupCheck.Add(list);
                }

                if (hasHonorPair)
                {
                    //找到眼牌 剩餘牌組都是3個倍數
                    if (CheckAllSame(groupCheck))
                        return pairList;
                }
                else
                {
                    //沒有眼, 如果胡牌 一定會有一組 (147%3,258%3,369%3) 的餘數不一樣
                    if (CheckAllSame(groupCheck) == false)
                    {
                        //眼在餘數不一樣的那一組上
                        var index = groupCheck.FindIndex(item => (item[0] == item[1] && item[1] != item[2]) ||
                                                                 (item[0] != item[1] && item[1] == item[2]) ||
                                                                 (item[0] != item[1] && item[0] == item[2])
                                                                 );
                        //表示找到眼牌
                        // index = 0 萬
                        // index = 1 筒
                        // index = 2 條
                        if (index != -1)
                        {
                            var suit = groupCheck[index];
                            groupCheck.RemoveAt(index);
                            //找出眼牌後, 剩餘牌型皆成對或順
                            //(147%3,258%3,369%3) 的餘數都一樣
                            if (CheckAllSame(groupCheck))
                            {
                                int groupType = 0;
                                if (suit[0] == suit[1])
                                    groupType = 3; //369
                                else if (suit[1] == suit[2])
                                    groupType = 1; //147
                                else if (suit[0] == suit[2])
                                    groupType = 2; //258

                                for (int shift = 0; shift < Set; shift++)
                                {
                                    var tile = index * 10 + groupType + shift * Set;
                                    if (suitGroup[index].FindIndex(item => item.Tile == tile && item.Count >= Pair) != -1)
                                        pairList.Add(tile);
                                }

                                return pairList;
                            }
                        }
                    }
                }
            }

            return new List<int>();
        }

        /// <summary>
        /// 判斷是否胡牌
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        private bool CheckWin(List<MJTile> lists)
        {
            //一副牌 P,若把一個對子(俗稱眼睛) 拿掉後,假設此時數字最小的牌是 x,若 x 的張數是 3 張以上,則拿掉 3 張 x(一刻)後,剩下牌為 Q
            //否則拿掉 x, x+1, x + 2(一順)之後,剩下的牌為Q(若無法拿，則 P 沒胡) 則  P胡 若且唯若 Q胡,
            while (lists.Count > 0)
            {
                lists.Sort((a, b) => a.Tile - b.Tile);
                if (lists[0].Count == Set)
                {
                    lists[0].Remove(Set);
                }
                else if (lists.Count >= Set && (lists[2].Tile - lists[0].Tile) == 2 && (lists[0].Tile + lists[1].Tile + lists[2].Tile) == lists[1].Tile * Set)
                {
                    lists[0].Remove();
                    lists[1].Remove();
                    lists[2].Remove();
                }
                else
                {
                    return false;
                }

                lists.RemoveAll(item => item.Count == 0);
            }
            return true;
        }


        /// <summary>
        /// 判斷每一個單一數列內的數值都一樣
        /// </summary>
        /// <param name="groupCheck"></param>
        /// <returns></returns>
        private bool CheckAllSame(List<List<int>> groupCheck)
        {
            return groupCheck.TrueForAll(g => g.All(item => item == g.First()));
        }

    }

}
