using System;
using System.Collections.Generic;
using System.Text;

namespace GreatUma.Utils
{
    public enum OddsType
    {
        Theoretical = 0,
        Actual = 1
    }

    public enum RegionType
    {
        Central = 0,
        Regional = 1,
    }

    public enum TicketType
    {
        //単勝
        Win = 0,
        //馬単
        Exacta = 1,
        //馬連
        Quinella = 2,
        //三連単
        Trifecta = 3,
        //三連複
        Trio = 4,
        //ワイド
        Wide = 5,
        //複勝
        Place = 6
    }
}
