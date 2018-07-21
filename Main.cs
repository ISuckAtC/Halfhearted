using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CSharp_Shell
{
    public class ExtraFunctions
    {
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0) { return min; }
            if (value.CompareTo(max) > 0) { return max; }
            return value;
        }
    }
    public class Item
    {
        public string itemName;

        public int itemStack;

        public int itemDamage;
        public int itemHealing;

        public bool isEquipable;
        public bool isConsumable;

        public Item(string name, int damage = 0, bool equipable = false, bool consumable = false, int healing = 0, int stack = 0)
        {
            itemName = name;
            itemDamage = damage;
            itemHealing = healing;
            isEquipable = equipable;
            isConsumable = consumable;
            itemStack = stack;
        }
        public void infoDump()
        {
            Console.WriteLine(itemName);
            if (itemDamage != 0)
            {
                Console.WriteLine("Damage: {0}", itemDamage);
            }
            if (itemHealing != 0)
            {
                Console.WriteLine("Healing: {0}", itemHealing);
            }
            if (itemStack > 0)
            {
                Console.WriteLine("Uses: {0}", itemStack);
            }

        }
    }

    public class PlayerProfile
    {
        public string playerName;
        public string playerClass;

        public int health;
        private int skill;
        private int xp;
        private int level;
        private int damage;
        private int maxLevel;
        public int maxInv;
        public int maxHealth;

        public bool isAlive;

        private int startXp;
        private double XpX;

        private int[] levelXp;

        private int[] inventory;

        public int weaponSlot;

        private int cash;

        public PlayerProfile(string pName, string pClass)
        {
            playerName = pName;
            playerClass = pClass;
            xp = 0;
            level = 1;
            startXp = 100;
            XpX = 1.5;
            maxLevel = 20;
            maxInv = 10;
            isAlive = true;

            levelXp = new int[maxLevel];
            inventory = new int[maxInv];

            levelXp[0] = startXp;


            switch (playerClass)
            {
                case "Knight":
                    maxHealth = 200;
                    skill = 8;
                    break;

                case "Warrior":
                    maxHealth = 150;
                    skill = 10;
                    break;

                case "Thief":
                    maxHealth = 100;
                    skill = 13;
                    break;

                case "Kieran":
                    maxHealth = 30;
                    skill = -5;
                    break;

                default:
                    maxHealth = 140;
                    skill = 10;
                    break;
            }

            for (int x = level; x < maxLevel; x++)
            {
                levelXp[x] = Convert.ToInt32(levelXp[x - 1] * XpX);
            }

            health = maxHealth;

            UpdateStats();
        }
        private void UpdateStats()
        {
            if (isAlive == false)
            {
                Console.WriteLine("Oh dear, you are dead!");
            }
            damage = skill / 2;

            if (weaponSlot != 0)
            {
                damage += Program.items[weaponSlot].itemDamage;
            }
        }
        public void TakeDamage(int amount)
        {
            health -= amount;
            Console.WriteLine("{0} took {1} damage!", playerName, amount);
            if (health < 1)
            {
                isAlive = false;
            }
            UpdateStats();
        }
        public void Consume(Item consumable)
        {
            if (consumable.itemHealing > 0)
            {
                if (maxHealth - health > 0)
                {
                    int trueHeal = ExtraFunctions.Clamp(consumable.itemHealing, 0, maxHealth - health);
                    health += trueHeal;
                    Console.WriteLine("{0} was healed for {1}", playerName, trueHeal);
                    Console.WriteLine("{0} now has {1} health", playerName, health);
                    consumable.itemStack--;
                    if (consumable.itemStack == 0)
                    {
                        SetItem(Program.invID, 0);
                    }
                } else { Console.WriteLine("{0} already has full health!",playerName); }
            }
        }
        public void ListLevelXp()
        {
            string read = Console.ReadLine();
            bool total;
            int tempXp;

            switch (read)
            {
                case "total":
                    total = true;
                    break;

                case "diff":
                    total = false;
                    break;

                default:
                    total = true;
                    break;
            }
            int tempLevel = 1;
            for (int x = 1; x < maxLevel; x++)
            {
                if (total == true)
                {
                    tempXp = levelXp[x];
                }
                else tempXp = levelXp[x] - levelXp[x - 1];
                Console.WriteLine("[Level]: {0} || [XP to Level {1}]: {2}", tempLevel, tempLevel + 1, tempXp);
                tempLevel++;
            }
        }
        public void Stats()
        {
            Console.WriteLine("[Name]: {0}", playerName);
            Console.WriteLine("[Class]: {0}", playerClass);
            Console.WriteLine("[Level]: {0}", level);
            Console.WriteLine("[Health]: {0}", health);
            Console.WriteLine("[Skill]: {0}", skill);
            Console.WriteLine("[Experience]: {0}", xp);
            Console.WriteLine("[Money]: {0} gold", cash);
        }
        public void ModCash(int amount)
        {
            cash += amount;
            string joker = "added";
            string jokker = "to";
            if (amount < 0)
            {
                joker = "removed";
                jokker = "from";
                amount = -amount;
            }
            Console.WriteLine("You {0} {1} gold {2} {3}", joker, amount, jokker, playerName);
        }
        public void SetItem(int invID, int itemID)
        {
            inventory[invID] = itemID;
        }
        public void SetWeapon(int invID, int itemID)
        {
            weaponSlot = itemID;
        }
        public int ItemInfo(int invID)
        {
            return inventory[invID];
        }
        public int FindEmpty()
        {
            int temp = -1;
            foreach (int a in inventory)
            {
                if (a == 0)
                {
                    temp = a;
                    break;
                }
            }
            return temp;
        }
    }

    public class Program
    {

        static public PlayerProfile[] profiles = new PlayerProfile[99];
        static public Item[] items = new Item[99];
        static int playerCount = 0;
        static int profileID;
        static public int invID;
        static string read;
        static int tempID;
        public static bool dev = true;
        static public PlayerProfile activePlayer;

        private const string I = "Info";


        public static void CreatePlayer()
        {
            Console.WriteLine("What is your player name?");
            string pName = Console.ReadLine();
            Console.WriteLine("What is your player class?");
            string pClass = Console.ReadLine();
            profiles[playerCount] = new PlayerProfile(pName, pClass);
            playerCount++;
        }

        public static void PopulateItems()
        {
            items[1] = new Item(name: "Iron Sword", damage: 10, equipable: true);
            items[2] = new Item(name: "Lesser Health Potion", healing: 4, consumable: true);
            items[3] = new Item(name: "Dark Totem");
            items[4] = new Item(name: "Flesh Blade", damage: 3, healing: 2, equipable: true, consumable: true);
            items[5] = new Item(name: "Saradomin Brew", healing: 16, consumable: true, stack: 4);
        }

        public static void Main()
        {
            profiles[playerCount] = new PlayerProfile("Test", "Warrior");
            playerCount++;
            PopulateItems();

            Console.WriteLine("Create or select profile");
            for (; ; )
            {
                read = Console.ReadLine();
                switch (read)
                {
                    case "DEVMODE":
                        string devCheck = Console.ReadLine();
                        if (devCheck == "No Nut Kieran 170394")
                        {
                            dev = true;
                            Console.WriteLine("DEVMODE ACTIVATED");
                        }
                        break;

                    case "Create":
                        CreatePlayer();
                        break;

                    default:
                        break;
                }
                if (int.TryParse(read, out profileID))
                {
                    activePlayer = profiles[profileID];
                    if (activePlayer.isAlive == false)
                    {
                        Console.WriteLine("This character is dead");
                        continue;
                    }
                    Console.WriteLine("Selected profile: [{0}] (ID: {1})", activePlayer.playerName, profileID);
                    for (; ; )
                    {
                        read = Console.ReadLine();
                        switch (read)
                        {
                            case "TakeDamage":
                                for (; ; )
                                {
                                    Console.WriteLine("How much damage do you wish to take?");
                                    read = Console.ReadLine();
                                    int amount;
                                    if (int.TryParse(read, out amount))
                                    {
                                        if (amount >= 0)
                                        {
                                            activePlayer.TakeDamage(amount);
                                            break;
                                        } else { Console.WriteLine("You can't take negative damage!"); }
                                    } else { Console.WriteLine("That is not a valid integer!"); }
                                }
                                break;

                            case "ListLevelXp":
                                activePlayer.ListLevelXp();
                                break;

                            case "Name":
                                string getName = activePlayer.playerName;
                                Console.WriteLine(getName);
                                break;

                            case "Class":
                                string getClass = activePlayer.playerClass;
                                Console.WriteLine(getClass);
                                break;

                            case I:
                                activePlayer.Stats();
                                break;

                            case "ModCash":
                                if (dev == true)
                                {
                                    Console.WriteLine("How much money do you want to add/remove?");
                                    int amount;
                                    if (int.TryParse(Console.ReadLine(), out amount))
                                    {
                                        activePlayer.ModCash(amount);
                                        break;
                                    }
                                    else { break; }
                                }
                                break;

                            case "Inventory":
                                string name = activePlayer.playerName;
                                Console.WriteLine("[Inventory of {0}]", name);
                                Console.WriteLine("Select an inventory or equipment slot");
                                for (; ; )
                                {
                                    read = Console.ReadLine();
                                    if (int.TryParse(read, out invID))
                                    {
                                        Console.WriteLine("Selected inventory slot [{0}]", invID);
                                        for (; ; )
                                        {
                                            if (invID > (activePlayer.maxInv - 1) || invID < 0)
                                            {
                                                Console.WriteLine("That is not a valid inventory slot");
                                                break;
                                            } else {tempID = activePlayer.ItemInfo(invID);}
                                            read = Console.ReadLine();
                                            switch (read)
                                            {
                                                case "Consume":
                                                    if (tempID != 0)
                                                    {
                                                        if (items[tempID].isConsumable)
                                                        {
                                                            activePlayer.Consume(items[tempID]);
                                                        }
                                                        else
                                                        { Console.WriteLine("This is not a consumable item"); }
                                                    }
                                                    else { Console.WriteLine("There is no item to consume"); }
                                                    break;


                                                case "Add":
                                                    for (; ; )
                                                    {
                                                        Console.WriteLine("Enter item ID");
                                                        read = Console.ReadLine();
                                                        if (int.TryParse(read, out tempID))
                                                        {
                                                            activePlayer.SetItem(invID, tempID);
                                                            break;
                                                        }
                                                    }
                                                    break;

                                                case I:
                                                    if (tempID != 0)
                                                    {
                                                        items[tempID].infoDump();
                                                    }
                                                    else { Console.WriteLine("There is no item to view"); }
                                                    break;

                                                case "Equip":
                                                    if (tempID != 0)
                                                    {
                                                        if (items[tempID].isEquipable == true)
                                                        {
                                                            activePlayer.weaponSlot = tempID;
                                                            activePlayer.SetItem(invID, 0);
                                                        }
                                                        else { Console.WriteLine("You can't equip that item!"); }
                                                    }
                                                    else { Console.WriteLine("There is no item to equip"); }
                                                    break;

                                                default: break;
                                            }
                                            if (read == "back")
                                            {
                                                read = "lul";
                                                Console.WriteLine("[Inventory of {0}]", name);
                                                Console.WriteLine("Select an inventory or equipment slot");
                                                break;
                                            }
                                        }

                                    }
                                    switch (read)
                                    {
                                        case "Weapon":
                                            Console.WriteLine("Weapon Slot");
                                            for (; ; )
                                            {
                                                tempID = activePlayer.weaponSlot;
                                                read = Console.ReadLine();
                                                switch (read)
                                                {
                                                    case I:
                                                        if (tempID != 0)
                                                        {
                                                            items[tempID].infoDump();
                                                        }
                                                        else { Console.WriteLine("There is nothing equipped in this slot"); }
                                                        break;

                                                    case "Unequip":
                                                        if (tempID != 0)
                                                        {
                                                            if (activePlayer.FindEmpty() == -1)
                                                            {
                                                                Console.WriteLine("No empty inventory slots available");
                                                                break;
                                                            }
                                                            activePlayer.SetItem(activePlayer.FindEmpty(), tempID);
                                                            activePlayer.weaponSlot = 0;
                                                        }
                                                        else { Console.WriteLine("There is nothing equipped in this slot"); }
                                                        break;

                                                    default: break;
                                                }
                                                if (read == "back")
                                                {
                                                    read = "lul";
                                                    Console.WriteLine("[Inventory of {0}]", name);
                                                    Console.WriteLine("Select an inventory or equipment slot");
                                                    break;
                                                }
                                            }
                                            break;

                                        default: break;
                                    }
                                    if (read == "back")
                                    {
                                        read = "lul";
                                        Console.WriteLine("Selected profile: [{0}] (ID: {1})", activePlayer.playerName, profileID);
                                        break;
                                    }
                                }
                                break;

                            default:
                                break;
                        }
                        if (read == "back")
                        {
                            read = "lul";
                            Console.WriteLine("Create or select profile");
                            break;
                        }
                        if (activePlayer.isAlive == false)
                        {
                            Console.WriteLine("Create or select profile");
                            break;
                        }
                    }
                }
                if (read == "exit")
                {
                    break;
                }
            }
        }
    }
}
