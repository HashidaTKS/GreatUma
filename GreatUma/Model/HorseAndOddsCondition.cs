﻿using GreatUma.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreatUma.Model
{
        public class HorseAndOddsCondition
        {
            [DisplayName("発送時刻")]
            public DateTime StartTime { get; set; }
            [DisplayName("競馬場")]
            public string Region { get; set; }
            [DisplayName("タイトル")]
            public string Title { get; set; }
            [DisplayName("コース")]
            public string Course { get; set; }
            [DisplayName("馬番")]
            public string HorseNum { get; set; }
            [DisplayName("騎手")]
            public string Jocky { get; set; }
            [DisplayName("0時オッズ")]
            public string MidnightOdds { get; set; }
            [DisplayName("現在オッズ")]
            public string CurrentOdds { get; set; }
            [DisplayName("3分前購入条件")]
            public double PurchaseCondition { get; set; }
            [Browsable(false)]
            public RaceData RaceData { get; set; }
        }
}
