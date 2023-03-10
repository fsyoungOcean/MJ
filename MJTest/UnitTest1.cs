using Mahjong;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace MJTest;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    #region CheckTileList
    class CheckTileListEnumerable : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var test in CheckTileList)
            {
                yield return new TestCaseData(SetTile(test.Value.Item1), SetTile(test.Value.Item2), test.Value.Item3)
                    .SetName($"CheckTileList {test.Key}");
            }
        }
    }

    [TestCaseSource(typeof(CheckTileListEnumerable))]
    public void CheckAllTile(List<int> grounded, List<int> hand, List<ListenState> expectResult)
    {
        var mj = new MahjongAction(MahjongGameType.e16);

        var result = mj.CheckListen(grounded, hand);

        expectResult.Sort();
        result.Sort();
        Assert.AreEqual(expectResult.Count, result.Count);
        for (int i = 0; i < result.Count; i++)
        {
            Assert.AreEqual(expectResult[i], result[i]);
        }


    }
    #endregion

    #region ShowListenTile
    class CheckListenList : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var test in TileListen)
            {
                yield return new TestCaseData(SetTile(test.Value.Item1))
                    .Returns(SetName<int>(test.Value.Item2))
                    .SetName($"CheckListen {test.Key} {SetName<MJ>(test.Value.Item1)}");
            }
        }
    }


    [TestCaseSource(typeof(CheckListenList))]
    public string CheckListen(List<int> lists)
    {
        var mj = new MahjongAction(MahjongGameType.e16);
        var map = new MJTileGourp(lists);

        var result = mj.ShowListenTile(map.ToList());
        return string.Join(",", result);
    }
    #endregion

    #region CheckWin
    class CheckWinList : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var test in TileWinGroup)
            {
                yield return new TestCaseData(SetTile(test.Value.Item1))
                    .Returns(test.Value.Item2)
                    .SetName($"CheckWin {test.Key} {SetName<MJ>(test.Value.Item1)}");
            }
        }
    }

    [TestCaseSource(typeof(CheckWinList))]
    public bool CheckIsWin(List<int> lists)
    {
        var mj = new MahjongAction(MahjongGameType.e16);
        var map = new MJTileGourp(lists);

        return mj.IsWin(map.ToList());
    }
    #endregion

    #region CheckPair

    class CheckPairList : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            foreach (var test in TileWinGroup)
            {
                yield return new TestCaseData(SetTile(test.Value.Item1))
                    .Returns(SetName<int>(test.Value.Item3))
                    .SetName($"CheckPair {test.Key} {SetName<MJ>(test.Value.Item1)}");
            }
        }
    }

    [TestCaseSource(typeof(CheckPairList))]
    public string CheckPairs(List<int> lists)
    {
        var mj = new MahjongAction(MahjongGameType.e16);

        var map = new MJTileGourp(lists);

        var result = mj.FindPairs(map.ToList());

        return string.Join(",", result);
    }
    #endregion

    static List<int> SetTile(params MJ[] list)
    {
        return new List<int>(list.Cast<int>());
    }

    static string SetName<T>(params MJ[] list)
    {
        return string.Join(",", new List<T>(list.Cast<T>()));
    }

    /// <summary>
    /// ???????????????????????????
    /// ??????????????????????????????
    /// int, (MJ[], MJ[], List<ListenState> => ?????????(?????????????????????????????????)
    /// </summary>
    /// <param name="CheckTileList"></param>
    /// <returns></returns>
    static Dictionary<int, (MJ[], MJ[], List<ListenState>)> CheckTileList = new Dictionary<int, (MJ[], MJ[], List<ListenState>)>
    {
        {
            0, (new MJ[]{ },
                new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9, MJ.B2, MJ.B3, MJ.B4, MJ.B1 },
                new List<ListenState>{ new ListenState(){ Win = true} })
        },
        {
            1, (new MJ[]{ },
                new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9, MJ.B2, MJ.B3, MJ.B4, MJ.Red },
                new List<ListenState>
                {
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.Red,
                        Listen = (new List<MJ>{ MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 }).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B2,
                        Listen = (new List<MJ>{ MJ.Red }).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B5,
                        Listen = (new List<MJ>{ MJ.Red }).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B8,
                        Listen = (new List<MJ>{ MJ.Red }).Cast<int>().ToList()
                    }
                })
        },
        {
            2, (new MJ[]{ MJ.C1, MJ.C2, MJ.C3, MJ.C2, MJ.C3, MJ.C4 },
                new MJ[]{ MJ.B1, MJ.B2, MJ.B2, MJ.B3, MJ.B3, MJ.B4, MJ.B4, MJ.B5, MJ.B5, MJ.B6, MJ.B6},
                new List<ListenState>
                {
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B1,
                        Listen = (new List<MJ>{MJ.B2, MJ.B3, MJ.B5, MJ.B6 }).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B2,
                        Listen = (new List<MJ>{MJ.B3, MJ.B6 }).Cast<int>().ToList()
                    },

                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B3,
                        Listen = (new List<MJ>{MJ.B2 }).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B4,
                        Listen = (new List<MJ>{MJ.B5, MJ.B6 }).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B5,
                        Listen = (new List<MJ>{MJ.B3, MJ.B6 }).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.B6,
                        Listen = (new List<MJ>{MJ.B2,MJ.B5}).Cast<int>().ToList()
                    }
                })
        },
        {
            3, (new MJ[]{ },
                new MJ[]{MJ.B1, MJ.B1, MJ.Red, MJ.Green, MJ.C3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9, MJ.B2, MJ.B3, MJ.B4, MJ.B1 },
                new List<ListenState>())
        },
        {
            4, (new MJ[]{ },
                new MJ[]{MJ.D3, MJ.D4, MJ.D4, MJ.D4, MJ.D4, MJ.D5, MJ.D6, MJ.D7, MJ.D8, MJ.D9, MJ.B9, MJ.B9, MJ.B9, MJ.B8, MJ.B8, MJ.B8, MJ.C9 },
                new List<ListenState> {
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.C9,
                        Listen = (new List<MJ>{ MJ.D2,MJ.D3,MJ.D5,MJ.D6,MJ.D9}).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.D3,
                        Listen = (new List<MJ>{ MJ.C9}).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.D6,
                        Listen = (new List<MJ>{ MJ.C9}).Cast<int>().ToList()
                    },
                    new ListenState()
                    {
                        Win = false,
                        Remove = (int)MJ.D9,
                        Listen = (new List<MJ>{ MJ.C9}).Cast<int>().ToList()
                    }
                })
        },
    };

    /// <summary>
    /// ??????????????????
    /// int, (MJ[], MJ[]) => index (???????????????)
    /// </summary>
    static Dictionary<int, (MJ[], MJ[])> TileListen = new Dictionary<int, (MJ[], MJ[])>
    {
        //???1~9
        //????????????
        {
            0, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9, MJ.B2, MJ.B3, MJ.B4 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            1, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9, MJ.B3, MJ.B4, MJ.B5 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            2, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9, MJ.B4, MJ.B5, MJ.B6 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            3, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9, MJ.B5, MJ.B6, MJ.B7 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            4, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9, MJ.B6, MJ.B7, MJ.B8 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        //????????????
        {
            5, (new MJ[]{MJ.B1, MJ.B1, MJ.B2, MJ.B2, MJ.B2, MJ.B3, MJ.B3, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            6, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B7, MJ.B7, MJ.B8, MJ.B8, MJ.B8, MJ.B9, MJ.B9 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        //?????????
        {
            7, (new MJ[]{MJ.B2, MJ.B2, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B7, MJ.B7, MJ.B8, MJ.B8, MJ.B8, MJ.B9, MJ.B9, MJ.B9 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            8, (new MJ[]{MJ.B2, MJ.B2, MJ.B2, MJ.B3, MJ.B3, MJ.B3, MJ.B4, MJ.B4, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9, MJ.B9, MJ.B9 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            9, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B6, MJ.B6, MJ.B7, MJ.B7, MJ.B7, MJ.B8, MJ.B8, MJ.B8 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            10, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B2, MJ.B2, MJ.B2, MJ.B3, MJ.B3, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B8, MJ.B8 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        //?????????
        {
            11, (new MJ[]{MJ.B1, MJ.B1, MJ.B1, MJ.B3, MJ.B3, MJ.B3, MJ.B4, MJ.B4, MJ.B5, MJ.B5, MJ.B6, MJ.B6, MJ.B7, MJ.B7, MJ.B8, MJ.B8 },
                new MJ[]{MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            12, (new MJ[]{MJ.B2, MJ.B2, MJ.B2, MJ.B3, MJ.B3, MJ.B3, MJ.B4, MJ.B4, MJ.B5, MJ.B5, MJ.B6, MJ.B6, MJ.B7, MJ.B7, MJ.B8, MJ.B8 },
                new MJ[]{MJ.B1, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8, MJ.B9 })
        },
        {
            13, (new MJ[]{MJ.B2, MJ.B2, MJ.B3, MJ.B3, MJ.B4, MJ.B4, MJ.B5, MJ.B5, MJ.B6, MJ.B6, MJ.B7, MJ.B7, MJ.B7, MJ.B8, MJ.B8, MJ.B8 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B9 })
        },
        {
            14, (new MJ[]{MJ.B2, MJ.B2, MJ.B3, MJ.B3, MJ.B4, MJ.B4, MJ.B5, MJ.B5, MJ.B6, MJ.B6, MJ.B7, MJ.B7, MJ.B7, MJ.B9, MJ.B9, MJ.B9 },
                new MJ[]{MJ.B1, MJ.B2, MJ.B3, MJ.B4, MJ.B5, MJ.B6, MJ.B7, MJ.B8 })
        }
    };

    /// <summary>
    /// ????????????
    /// int, (MJ[], bool, MJ[]) => index, (????????????????????????????????????)
    /// </summary>
    static Dictionary<int, (MJ[], bool, MJ[])> TileWinGroup = new Dictionary<int, (MJ[], bool, MJ[])>
    {
        { 0,(new MJ[] { MJ.C1, MJ.C1, MJ.C1, MJ.C2, MJ.C2, MJ.C2, MJ.C4, MJ.C4, MJ.C4, MJ.C5, MJ.C5, MJ.C5, MJ.C7, MJ.C8, MJ.C9, MJ.East, MJ.East },
                true,
                new MJ[] { MJ.East })
        },
        { 1,(new MJ[]{ MJ.C1, MJ.C1, MJ.C1, MJ.C2, MJ.C2, MJ.C2, MJ.C4, MJ.C4, MJ.C4, MJ.C5, MJ.C5, MJ.C5, MJ.Red, MJ.Red, MJ.Red, MJ.East, MJ.East},
            true,
            new MJ[]{ MJ.East })
        },
        { 2,(new MJ[]{ MJ.C1, MJ.C1, MJ.C1, MJ.C2, MJ.C2, MJ.C2, MJ.C4, MJ.C4, MJ.C4, MJ.C5, MJ.C5, MJ.C5, MJ.Red, MJ.Red, MJ.Red, MJ.B5, MJ.B5 },
            true,
            new MJ[]{ MJ.B5 })
        },
        { 3,(new MJ[]{ MJ.C1, MJ.C2, MJ.C3, MJ.C2, MJ.C3, MJ.C4, MJ.C4, MJ.C4, MJ.C4, MJ.C5, MJ.C5, MJ.C5, MJ.Red, MJ.Red, MJ.Red, MJ.C1, MJ.C1 },
            true,
            new MJ[]{ MJ.C1,MJ.C4})
        },
        { 4,(new MJ[]{ MJ.C1, MJ.C1, MJ.C1, MJ.C2, MJ.C2, MJ.C2, MJ.C4, MJ.C4, MJ.C4, MJ.C5, MJ.C5, MJ.C5, MJ.Red, MJ.Red, MJ.Green, MJ.East, MJ.East},
            false,
            new MJ[]{})
        },
        { 5,(new MJ[]{ MJ.East, MJ.East},
            true,
            new MJ[]{ MJ.East })
        }
    };
}