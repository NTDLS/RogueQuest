using Library.Engine.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Engine
{
    public class PlayerState
    {
        private EngineCoreBase _core;

        public PlayerState(EngineCoreBase core)
        {
            _core = core;
            Equipment = new List<Equip>();
        }

        public void SetCore(EngineCoreBase core)
        {
            _core = core;
        }

        public List<Equip> Equipment { get; set; }
        public Guid UID { get; set; }
        public string Name { get; set; }
        public int Avatar { get; set; }

        //These are the attributes that the player started with from character creation.
        public int StartingConstitution { get; set; }
        public int StartingDexterity { get; set; }
        public int StartingIntelligence { get; set; }
        public int StartingStrength { get; set; }

        //Augmented attributes are increased/decreased via enchanted items, potions, etc.
        private int _augmentedConstitution = 0;
        public int AugmentedConstitution
        {
            get
            {
                return _augmentedConstitution;
            }
            set
            {
                var delta = value - _augmentedConstitution;
                _augmentedConstitution = value;
                AvailableHitpoints += delta;
            }
        }

        private int _augmentedIntelligence = 0;
        public int AugmentedIntelligence
        {
            get
            {
                return _augmentedIntelligence;
            }
            set
            {
                var delta = value - _augmentedIntelligence;
                _augmentedIntelligence = value;
                AvailableMana += delta;
            }
        }

        public int AugmentedDexterity { get; set; }
        public int AugmentedStrength { get; set; }
        public int AugmentedAC { get; set; }

        //Starting + Augmented attributes.
        public int Constitution => StartingConstitution + AugmentedConstitution;
        public int Dexterity => StartingDexterity + AugmentedDexterity;
        public int Intelligence => StartingIntelligence + AugmentedIntelligence;
        public int Strength => StartingStrength + AugmentedStrength;
        public int Mana => ((6 * Level) + StartingIntelligence) + AugmentedIntelligence;
        public int Hitpoints => ((6 * Level) + StartingConstitution) + AugmentedConstitution;
        public int MaxWeight => ((Level * 20) + StartingStrength) + AugmentedStrength;
        public int Experience { get; set; }
        public int NextLevelExperience { get; set; }
        public int Level { get; set; }

        private int _availableMana;
        public int AvailableMana
        {
            get
            {
                return _availableMana;
            }
            set
            {
                _availableMana = value < 0 ? 0 : value;
            }
        }

        private int _availableHitpoints;
        public int AvailableHitpoints
        {
            get
            {
                return _availableHitpoints;
            }
            set
            {
                _availableHitpoints = value < 0 ? 0 : value;
            }
        }

        /// <summary>
        /// Gets the aggregate amount of money in gold equivalent.
        /// </summary>
        public int Money
        {
            get
            {
                //Get the purse that is equiped.
                var equipSlot = _core.State.Character.GetEquipSlot(EquipSlot.Purse);
                if (equipSlot != null && equipSlot.Tile != null)
                {
                    //Find all the money in the purse.
                    var money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();
                    int value = (int)(money.Sum(o => o.Tile.Meta.Quantity * o.Tile.Meta.Value) ?? 0.0);

                    return value;
                }
                return 0;
            }
        }

        public void AddMoney(TileIdentifier moneyToAdd)
        {
            var equipSlot = _core.State.Character.GetEquipSlot(EquipSlot.Purse);
            if (equipSlot != null && equipSlot.Tile != null)
            {
                //Find all the money in the purse.
                var money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();

                //If we dont have a the coin type, add a zero quantity one.
                if (money.Where(o => o.Tile.Meta.Name.Contains(moneyToAdd.Meta.Name)).FirstOrDefault() == null)
                {
                    var addedTile = _core.Materials.Where(o => o.Meta.SubType == ActorSubType.Money && o.Meta.Name == moneyToAdd.Meta.Name).First().Clone(true);
                    _core.State.Items.Add(new CustodyItem() { Tile = addedTile, ContainerId = equipSlot.Tile.Meta.UID });

                    money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();
                }

                var tileToModify = money.Where(o => o.Tile.Meta.Name == moneyToAdd.Meta.Name).First().Tile;

                tileToModify.Meta.Quantity = (tileToModify.Meta.Quantity ?? 0) + moneyToAdd.Meta.Quantity;
            }
        }

        public void AddMoney(int amountOfMoneyToAdd)
        {
            var equipSlot = _core.State.Character.GetEquipSlot(EquipSlot.Purse);
            if (equipSlot != null && equipSlot.Tile != null)
            {
                //Find all the money in the purse.
                var money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();

                //If we dont have a gold coin, add a zero quantity one.
                if (money.Where(o => o.Tile.Meta.Name.Contains("Gold")).FirstOrDefault() == null)
                {
                    var goldTile = _core.Materials.Where(o => o.Meta.SubType == ActorSubType.Money && o.Meta.Name.Contains("Gold")).First().Clone(true);
                    _core.State.Items.Add(new CustodyItem() { Tile = goldTile, ContainerId = equipSlot.Tile.Meta.UID });

                    money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();
                }

                var goldTiles = money.Where(o => o.Tile.Meta.Name.Contains("Gold")).First().Tile;

                goldTiles.Meta.Quantity = (goldTiles.Meta.Quantity ?? 0) + amountOfMoneyToAdd;
            }
        }

        public void DeductMoney(int amountOfGoldToDeduct)
        {
            if (amountOfGoldToDeduct > this.Money)
            {
                return; //We don't have enough.
            }

            var equipSlot = _core.State.Character.GetEquipSlot(EquipSlot.Purse);
            if (equipSlot != null && equipSlot.Tile != null)
            {
                //Find all the money in the purse.
                var money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();

                //If we dont have a gold coin, add a zero quantity one.
                if (money.Where(o => o.Tile.Meta.Name.Contains("Gold")).FirstOrDefault() == null)
                {
                    var goldTile = _core.Materials.Where(o => o.Meta.SubType == ActorSubType.Money && o.Meta.Name.Contains("Gold")).First().Clone(true);
                    _core.State.Items.Add(new CustodyItem() { Tile = goldTile, ContainerId = equipSlot.Tile.Meta.UID });

                    money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();
                }

                int goldWeHave = (int)(money.Where(o => o.Tile.Meta.Name.Contains("Gold")).Sum(o => o.Tile.Meta.Quantity) ?? 0.0);

                if (goldWeHave < amountOfGoldToDeduct)
                {
                    MakeChangeUntilGoldAvailable(amountOfGoldToDeduct);
                }

                var goldTiles = money.Where(o => o.Tile.Meta.Name.Contains("Gold")).First().Tile;

                goldTiles.Meta.Quantity = (goldTiles.Meta.Quantity ?? 0) - amountOfGoldToDeduct;
            }
        }

        public void MakeChangeUntilGoldAvailable(int amountOfGoldToMake)
        {
            if (amountOfGoldToMake > this.Money)
            {
                return; //We don't have enough.
            }

            var equipSlot = _core.State.Character.GetEquipSlot(EquipSlot.Purse);
            if (equipSlot != null && equipSlot.Tile != null)
            {
                //Find all the money in the purse.
                var money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();

                //If we dont have a gold coin, add a zero quantity one.
                if (money.Where(o => o.Tile.Meta.Name.Contains("Gold")).FirstOrDefault() == null)
                {
                    var goldTile = _core.Materials.Where(o => o.Meta.SubType == ActorSubType.Money && o.Meta.Name.Contains("Gold")).First().Clone(true);
                    _core.State.Items.Add(new CustodyItem() { Tile = goldTile, ContainerId = equipSlot.Tile.Meta.UID });

                    money = _core.State.Items.Where(o => o.ContainerId == equipSlot.Tile.Meta.UID).ToList();
                }

                int goldWeHave = 0;

                do
                {
                    var copper = money.Where(o => o.Tile.Meta.Name.Contains("Copper")).FirstOrDefault()?.Tile.Meta;
                    var silver = money.Where(o => o.Tile.Meta.Name.Contains("Silver")).FirstOrDefault()?.Tile.Meta;
                    var gold = money.Where(o => o.Tile.Meta.Name.Contains("Gold")).FirstOrDefault()?.Tile.Meta;
                    var platinum = money.Where(o => o.Tile.Meta.Name.Contains("Platinum")).FirstOrDefault()?.Tile.Meta;

                    if (copper != null && copper.Quantity * copper.Value > 1)
                    {
                        int amountToDeduct = (int)(1.0 / copper.Value);
                        copper.Quantity -= amountToDeduct;
                        gold.Quantity = (gold.Quantity ?? 0) + 1;
                    }
                    else if (silver != null && silver.Quantity * silver.Value > 1)
                    {
                        int amountToDeduct = (int)(1.0 / silver.Value);
                        silver.Quantity -= amountToDeduct;
                        gold.Quantity = (gold.Quantity ?? 0) + 1;
                    }
                    else if (platinum != null && platinum.Quantity * platinum.Value > 1)
                    {
                        int amountToDeduct = (int)(1.0 / platinum.Value);
                        platinum.Quantity -= amountToDeduct;
                        gold.Quantity = (gold.Quantity ?? 0) + 1;
                    }

                    goldWeHave = (int)(money.Where(o => o.Tile.Meta.Name.Contains("Gold")).Sum(o => o.Tile.Meta.Quantity) ?? 0.0);
                } while (goldWeHave < amountOfGoldToMake);
            }
        }

        public void InitializeState()
        {
            AvailableHitpoints = Hitpoints;
            AvailableMana = Mana;
            NextLevelExperience = 300;
        }

        public void LevelUp()
        {
            Level++;
            NextLevelExperience = (int)(((float)NextLevelExperience) * 1.5f);
            AvailableHitpoints = Hitpoints;
            AvailableMana = Mana;
        }

        public Equip FindEquipSlot(Guid ?itemUid)
        {
            if (itemUid == null)
            {
                return null;
            }

            return Equipment.Where(o => o.Tile != null && o.Tile.Meta != null && o.Tile.Meta.UID == (Guid)itemUid).FirstOrDefault();
        }

        public Equip GetEquipSlot(EquipSlot slot)
        {
            var equip = Equipment.Where(o => o.Slot == slot).FirstOrDefault();
            if (equip == null)
            {
                equip = new Equip() { Slot = slot };
                Equipment.Add(equip);
            }

            return equip;
        }
    }
}
