using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace TerrariaAssets
{
    public class ItemData
    {
        public string _pageName { get; set; }
        public int itemid { get; set; }
        public string name { get; set; }
        public string internalname { get; set; }
        public string image { get; set; }
        public string imagefile { get; set; }
        public string imageplaced { get; set; }
        public string imageequipped { get; set; }
        public bool? autoswing { get; set; }
        public int stack { get; set; }
        public bool? consumable { get; set; }
        public bool? hardmode { get; set; }
        public List<string> type { get; set; }
        public List<string> listcat { get; set; }
        public List<string> tag { get; set; }
        public int? damage { get; set; }
        public string damagetype { get; set; }
        public int? defense { get; set; }
        public decimal? velocity { get; set; }
        public decimal? knockback { get; set; }
        public int? research { get; set; }
        public string rare { get; set; }
        public string buy { get; set; }
        public int? sell { get; set; }
        public string axe { get; set; }
        public string pick { get; set; }
        public string hammer { get; set; }
        public int? fishing { get; set; }
        public int? bait { get; set; }
        public int? bonus { get;  set; }
        public int? toolspeed { get; set; }
        public int? usetime { get; set; }
        public bool? unobtainable { get; set; }
        public int? critical { get; set; }
        public string tooltip { get; set; }
        public bool? placeable { get; set; }
        public short? placedwidth { get; set; }
        public short? placedheight { get; set; }
        public short? mana { get; set; }
        public short? hheal { get; set; }
        public short? mheal { get; set; }
        public string bodyslot { get; set; }
        public List<string> buffs { get; set; }
        public List<string> debuffs { get; set; }
        public bool? desktop { get; set; }
        public bool? oldgen { get; set; }
        public bool? japanese { get; set; }
        public string Exclusive_pageName { get; set; }
        public Sprite sprite { get; set; }
    }
}
