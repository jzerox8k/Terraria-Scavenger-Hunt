using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace TerrariaAssets
{
    public class TerrariaItemData : IElementConfiguration
    {
        public string pageurl { get; set; }
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

        public TerrariaItemData() { }

        public TerrariaItemData(TerrariaItemData itemData)
        {
            pageurl = itemData.pageurl;
            _pageName = itemData._pageName;
            itemid = itemData.itemid;
            name = itemData.name;
            internalname = itemData.internalname;
            image = itemData.image;
            imagefile = itemData.imagefile;
            imageplaced = itemData.imageplaced;
            imageequipped = itemData.imageequipped;
            autoswing = itemData.autoswing;
            stack = itemData.stack;
            consumable = itemData.consumable;
            hardmode = itemData.hardmode;
            type = itemData.type;
            listcat = itemData.listcat;
            tag = itemData.tag;
            damage = itemData.damage;
            damagetype = itemData.damagetype;
            defense = itemData.defense;
            velocity = itemData.velocity;
            knockback = itemData.knockback;
            research = itemData.research;
            rare = itemData.rare;
            buy = itemData.buy;
            sell = itemData.sell;
            axe = itemData.axe;
            pick = itemData.pick;
            hammer = itemData.hammer;
            fishing = itemData.fishing;
            bait = itemData.bait;
            bonus = itemData.bonus;
            toolspeed = itemData.toolspeed;
            usetime = itemData.usetime;
            unobtainable = itemData.unobtainable;
            critical = itemData.critical;
            tooltip = itemData.tooltip;
            placeable = itemData.placeable;
            placedwidth = itemData.placedwidth;
            placedheight = itemData.placedheight;
            mana = itemData.mana;
            hheal = itemData.hheal;
            mheal = itemData.mheal;
            bodyslot = itemData.bodyslot;
            buffs = itemData.buffs;
            debuffs = itemData.debuffs;
            desktop = itemData.desktop;
            oldgen = itemData.oldgen;
            japanese = itemData.japanese;
            Exclusive_pageName = itemData.Exclusive_pageName;
            sprite = itemData.sprite;
        }
    }
}
