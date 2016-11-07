﻿namespace PokerstarsAutoNotes.Enums
{
    ///<summary>
    ///</summary>
    public enum DbFieldEnum
    {
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
        compiledplayerresults_id = 1,
        totalhands = 2,
        totalamountwon = 3,
        totalrake = 4,
        totalbbswon = 5,
        vpiphands = 6,
        pfrhands = 7,
        couldcoldcall = 8,
        didcoldcall = 9,
        couldthreebet = 10,
        didthreebet = 11,
        couldsqueeze = 12,
        didsqueeze = 13,
        facingtwopreflopraisers = 14,
        calledtwopreflopraisers = 15,
        raisedtwopreflopraisers = 16,
        smallblindstealattempted = 17,
        smallblindstealdefended = 18,
        smallblindstealreraised = 19,
        bigblindstealattempted = 20,
        bigblindstealdefended = 21,
        bigblindstealreraised = 22,
        sawnonsmallshowdown = 23,
        wonnonsmallshowdown = 24,
        sawlargeshowdown = 25,
        wonlargeshowdown = 26,
        sawnonsmallshowdownlimpedflop = 27,
        wonnonsmallshowdownlimpedflop = 28,
        sawlargeshowdownlimpedflop = 29,
        wonlargeshowdownlimpedflop = 30,
        wonhand = 31,
        wonhandwhensawflop = 32,
        wonhandwhensawturn = 33,
        wonhandwhensawriver = 34,
        facedthreebetpreflop = 35,
        foldedtothreebetpreflop = 36,
        calledthreebetpreflop = 37,
        raisedthreebetpreflop = 38,
        facedfourbetpreflop = 39,
        foldedtofourbetpreflop = 40,
        calledfourbetpreflop = 41,
        raisedfourbetpreflop = 42,
        bigbetpreflopsawshowdown = 43,
        bigbetflopsawshowdown = 44,
        bigbetturnsawshowdown = 45,
        bigbetriversawshowdown = 46,
        bigcallpreflopsawshowdown = 47,
        bigcallflopsawshowdown = 48,
        bigcallturnsawshowdown = 49,
        bigcallriversawshowdown = 50,
        bigbetpreflopwonshowdown = 51,
        bigbetflopwonshowdown = 52,
        bigbetturnwonshowdown = 53,
        bigbetriverwonshowdown = 54,
        bigcallpreflopwonshowdown = 55,
        bigcallflopwonshowdown = 56,
        bigcallturnwonshowdown = 57,
        bigcallriverwonshowdown = 58,
        turnfoldippassonflopcb = 59,
        turncallippassonflopcb = 60,
        turnraiseippassonflopcb = 61,
        riverfoldippassonturncb = 62,
        rivercallippassonturncb = 63,
        riverraiseippassonturncb = 64,
        sawflop = 65,
        sawshowdown = 66,
        wonshowdown = 67,
        totalbets = 68,
        totalcalls = 69,
        flopcontinuationbetpossible = 70,
        flopcontinuationbetmade = 71,
        turncontinuationbetpossible = 72,
        turncontinuationbetmade = 73,
        rivercontinuationbetpossible = 74,
        rivercontinuationbetmade = 75,
        facingflopcontinuationbet = 76,
        foldedtoflopcontinuationbet = 77,
        calledflopcontinuationbet = 78,
        raisedflopcontinuationbet = 79,
        facingturncontinuationbet = 80,
        foldedtoturncontinuationbet = 81,
        calledturncontinuationbet = 82,
        raisedturncontinuationbet = 83,
        facingrivercontinuationbet = 84,
        foldedtorivercontinuationbet = 85,
        calledrivercontinuationbet = 86,
        raisedrivercontinuationbet = 87,
        totalpostflopstreetsseen = 88,
        totalaggressivepostflopstreetsseen = 89,
        vs_ep_raise_ip_fold = 90,
        vs_ep_raise_ip_call = 91,
        vs_ep_raise_ip_raise = 92,
        vs_mp_raise_ip_fold = 93,
        vs_mp_raise_ip_call = 94,
        vs_mp_raise_ip_raise = 95,
        vs_co_raise_ip_fold = 96,
        vs_co_raise_ip_call = 97,
        vs_co_raise_ip_raise = 98,
        vs_sb_raise_ip_fold = 99,
        vs_sb_raise_ip_call = 100,
        vs_sb_raise_ip_raise = 101,
        vs_ep_raise_oop_fold = 102,
        vs_ep_raise_oop_call = 103,
        vs_ep_raise_oop_raise = 104,
        vs_mp_raise_oop_fold = 105,
        vs_mp_raise_oop_call = 106,
        vs_mp_raise_oop_raise = 107,
        vs_co_raise_oop_fold = 108,
        vs_co_raise_oop_call = 109,
        vs_co_raise_oop_raise = 110,
        vs_bt_raise_oop_fold = 111,
        vs_bt_raise_oop_call = 112,
        vs_bt_raise_oop_raise = 113,
        ep_vs_raise_ip_fold = 114,
        ep_vs_raise_ip_call = 115,
        ep_vs_raise_ip_raise = 116,
        ep_vs_raise_oop_fold = 117,
        ep_vs_raise_oop_call = 118,
        ep_vs_raise_oop_raise = 119,
        mp_vs_raise_ip_fold = 120,
        mp_vs_raise_ip_call = 121,
        mp_vs_raise_ip_raise = 122,
        mp_vs_raise_oop_fold = 123,
        mp_vs_raise_oop_call = 124,
        mp_vs_raise_oop_raise = 125,
        co_vs_raise_ip_fold = 126,
        co_vs_raise_ip_call = 127,
        co_vs_raise_ip_raise = 128,
        co_vs_raise_oop_fold = 129,
        co_vs_raise_oop_call = 130,
        co_vs_raise_oop_raise = 131,
        bt_vs_raise_oop_fold = 132,
        bt_vs_raise_oop_call = 133,
        bt_vs_raise_oop_raise = 134,
        sb_vs_raise_ip_fold = 135,
        sb_vs_raise_ip_call = 136,
        sb_vs_raise_ip_raise = 137,
        facingsqueezeascaller = 138,
        foldtosqueezeascaller = 139,
        facingsqueezeasfirstraiser = 140,
        foldtosqueezeasfirstraiser = 141,
        bbfacingsbcompletion = 142,
        bbraisesbcompletion = 143,
        facingflopcheckraise = 144,
        foldtoflopcheckraise = 145,
        facingturncheckraise = 146,
        foldtoturncheckraise = 147,
        facingrivercheckraise = 148,
        foldtorivercheckraise = 149,
        facingflopcbetip = 150,
        foldtoflopcbetip = 151,
        raiseflopcbetip = 152,
        callflopcbetip = 153,
        facingturncbetip = 154,
        foldtoturncbetip = 155,
        raiseturncbetip = 156,
        callturncbetip = 157,
        facingrivercbetip = 158,
        foldtorivercbetip = 159,
        raiserivercbetip = 160,
        callrivercbetip = 161,
        facingflopcbetoop = 162,
        foldtoflopcbetoop = 163,
        raiseflopcbetoop = 164,
        callflopcbetoop = 165,
        facingturncbetoop = 166,
        foldtoturncbetoop = 167,
        raiseturncbetoop = 168,
        callturncbetoop = 169,
        facingrivercbetoop = 170,
        foldtorivercbetoop = 171,
        raiserivercbetoop = 172,
        callrivercbetoop = 173,
        cbetflopip = 174,
        couldcbetflopip = 175,
        cbetturnip = 176,
        couldcbetturnip = 177,
        cbetriverip = 178,
        couldcbetriverip = 179,
        cbetflopoop = 180,
        couldcbetflopoop = 181,
        cbetturnoop = 182,
        couldcbetturnoop = 183,
        cbetriveroop = 184,
        couldcbetriveroop = 185,
        bbfacingsbsteal = 186,
        bbfoldtosbsteal = 187,
        bbraisesbsteal = 188,
        bbcallsbsteal = 189,
        bbfacingbtnsteal = 190,
        bbfoldtobtnsteal = 191,
        bbraisebtnsteal = 192,
        bbcallbtnsteal = 193,
        bbfacingcosteal = 194,
        bbfoldtocosteal = 195,
        bbraisecosteal = 196,
        bbcallcosteal = 197,
        facedfivebetpreflop = 198,
        foldedtofivebetpreflop = 199,
        calledfivebetpreflop = 200,
        raisedfivebetpreflop = 201
// ReSharper restore UnusedMember.Global
// ReSharper restore InconsistentNaming
    }
}