using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace HalfHearted
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

    public abstract class Entity
    {
        public int health;
        public int speed;
        public int damage;

        public string Name;

        public bool isAlive;

        public abstract void Update();

        public void TakeDamage(int amount)
        {
            health -= amount;
            Console.WriteLine("{0} took {1} damage!", Name, amount);
            if (health < 1)
            {
                isAlive = false;
            }
            Update();
        }
    }

    public class Monster : Entity
    {
        private int XP;

        public Monster(string mName, int mHealth, int mSpeed = 0, int mDamage = 0, int mXp = 0)
        {
            isAlive = true;
            Name = mName;
            health = mHealth;
            speed = mSpeed;
            damage = mDamage;
            XP = mXp;
            Update();
        }

        public override void Update()
        {
            if (isAlive == false)
            {
                Console.WriteLine("{0} has slain {1}!", Program.activePlayer.Name, Name);
                Program.activePlayer.GainXP(XP);
            }
        }

        public void Stats()
        {
            Console.WriteLine("[Name]: {0}", Name);
            Console.WriteLine("[Health]: {0}", health);
            Console.WriteLine("[Damage]: {0}", damage);
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

    public class PlayerProfile : Entity
    {
        public string Class;

        private int strength;
        private int xp;
        private int level;
        private int maxLevel;
        public int maxInv;
        public int maxHealth;

        private int startXp;
        private double XpX;

        private int[] levelXp;

        private int[] inventory;

        public int weaponSlot;

        private int cash;

        public PlayerProfile(string pName, string pClass)
        {
            Name = pName;
            Class = pClass;
            xp = 0;
            level = 1;
            startXp = 100;
            XpX = 1.5;
            maxLevel = 20;
            maxInv = 10;
            isAlive = true;

            levelXp = new int[maxLevel];
            inventory = new int[maxInv];

            levelXp[1] = startXp;


            switch (Class)
            {
                case "Knight":
                    maxHealth = 200;
                    strength = 8;
                    speed = 5;
                    break;

                case "Warrior":
                    maxHealth = 150;
                    strength = 10;
                    speed = 7;
                    break;

                case "Thief":
                    maxHealth = 100;
                    strength = 13;
                    speed = 11;
                    break;

                case "Kieran":
                    maxHealth = 30;
                    strength = -5;
                    speed = 0;
                    break;

                default:
                    maxHealth = 140;
                    strength = 10;
                    speed = 5;
                    break;
            }

            for (int x = level; x < maxLevel - 1; x++)
            {
                levelXp[x + 1] = Convert.ToInt32(levelXp[x] * XpX);
            }

            health = maxHealth;

            Update();
        }

        public override void Update()
        {
            if (isAlive == false)
            {
                Console.WriteLine("Oh dear, you are dead!");
            }
            damage = strength / 2;

            if (weaponSlot != 0)
            {
                damage += Program.items[weaponSlot].itemDamage;
            }

            if (xp >= levelXp[level] && level < maxLevel)
            {
                LevelUp();
            }
        }

        public bool Consume(Item consumable)
        {
            if (consumable.itemHealing > 0)
            {
                if (maxHealth - health > 0)
                {
                    int trueHeal = ExtraFunctions.Clamp(consumable.itemHealing, 0, maxHealth - health);
                    health += trueHeal;
                    Console.WriteLine("{0} was healed for {1}", Name, trueHeal);
                    Console.WriteLine("{0} now has {1} health", Name, health);
                    consumable.itemStack--;
                    if (consumable.itemStack == 0)
                    {
                        SetItem(Program.invID, 0);
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("{0} already has full health!", Name);
                    return false;
                }
            }
            return false;
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
            Console.WriteLine("[Name]: {0}", Name);
            Console.WriteLine("[Class]: {0}", Class);
            Console.WriteLine("[Level]: {0}", level);
            Console.WriteLine("[Health]: {0}", health);
            Console.WriteLine("[Strength]: {0}", strength);
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
            Console.WriteLine("You {0} {1} gold {2} {3}", joker, amount, jokker, Name);
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
        public void GainXP(int amount)
        {
            xp += amount;
            Console.WriteLine("{0} gained {1} experience", Name, amount);
        }
        private void LevelUp()
        {
            level++;
            Console.WriteLine("LEVEL UP");
            Console.WriteLine("Congratulations, you are now level {0}", level);
            Console.WriteLine();
            Console.WriteLine("Choose a stat to level up");
            Console.WriteLine("Health | Strength | Speed");
            for (; ; )
            {
                bool end = false;
                string statAnswer = Console.ReadLine();
                switch (statAnswer)
                {
                    case "Health":
                        maxHealth += 10;
                        Console.WriteLine("Your health has been increased by 10 [{0}]", health);
                        end = true;
                        break;

                    case "Strength":
                        strength++;
                        Console.WriteLine("Your strength has been increased by 1 [{0}]", strength);
                        end = true;
                        break;

                    case "Speed":
                        speed++;
                        Console.WriteLine("Your speed has been increased by 1 [{0}]", speed);
                        end = true;
                        break;

                    default:
                        break;
                }
                if (end) { break; }
            }
            health = maxHealth;
        }
    }

    public class Program
    {

        static public List<PlayerProfile> profiles = new List<PlayerProfile>();
        static public Item[] items = new Item[99];
        static int playerCount = 0;
        static public int invID;
        static string read;
        static int tempID;
        public static bool dev = true;
        static public PlayerProfile activePlayer;
        static public Random random = new Random();
        static public bool endTurn;

        private const string I = "Info";


        public static void CreatePlayer()
        {
            Console.WriteLine("What is your player name?");
            string pName = Console.ReadLine();
            Console.WriteLine("What is your player class?");
            string pClass = Console.ReadLine();
            if (profiles.Exists(x => x.Name == pName) == false)
            {
                profiles.Add(new PlayerProfile(pName, pClass));
                playerCount++;
            } else { Console.WriteLine("A character with that name already exists"); }
        }

        public static void Battle(Monster monster)
        {
            Console.WriteLine("You encounter a {0}!", monster.Name);
            Console.WriteLine("Battle Start!");
            for (; ; )
            {
                for (; ; )
                {
                    endTurn = false;
                    Console.WriteLine("What do you wish to do?");
                    read = Console.ReadLine();
                    switch (read)
                    {
                        case "Attack":
                            int realDamage;

                            realDamage = activePlayer.damage;

                            Console.WriteLine("{0} attacks {1}!", activePlayer.Name, monster.Name, realDamage);
                            monster.TakeDamage(realDamage);
                            if (activePlayer.speed > monster.speed * 2 && monster.isAlive)
                            {
                                Console.WriteLine("Surperior speed allows {0} to attack again!", activePlayer.Name, realDamage);
                                monster.TakeDamage(realDamage);
                            }
                            endTurn = true;
                            break;

                        case "Inventory":
                            Inventory(activePlayer, true);
                            break;

                        case I:
                            if (dev)
                            {
                                monster.Stats();
                                break;
                            } else { break; }

                        default:
                            Console.WriteLine("Invalid choice");
                            break;
                    }
                    if (endTurn) { break; }
                }
                if (monster.isAlive)
                {
                    int monsterDamage;

                    monsterDamage = monster.damage;
                    Console.WriteLine("{0} attacks {1}!", monster.Name, activePlayer.Name, monsterDamage);
                    activePlayer.TakeDamage(monsterDamage);
                    if (monster.speed > activePlayer.speed * 2 && monster.isAlive)
                    {
                        Console.WriteLine("Surperior speed allows {0} to attack again!", monster.Name, monsterDamage);
                        activePlayer.TakeDamage(monsterDamage);
                    }
                }
                if (activePlayer.isAlive) { activePlayer.Update(); }
                if (monster.isAlive == false || activePlayer.isAlive == false) { break; }
            }
        }

        public static void Inventory(PlayerProfile player, bool isBattle)
        {
            bool hasConsumed = false;
            string name = player.Name;
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
                        if (invID > (player.maxInv - 1) || invID < 0)
                        {
                            Console.WriteLine("That is not a valid inventory slot");
                            break;
                        }
                        else { tempID = player.ItemInfo(invID); }
                        read = Console.ReadLine();
                        switch (read)
                        {
                            case "Consume":
                                if (isBattle == false || hasConsumed == false)
                                {
                                    if (tempID != 0)
                                    {
                                        if (items[tempID].isConsumable)
                                        {
                                            hasConsumed = player.Consume(items[tempID]);
                                        }
                                        else
                                        { Console.WriteLine("This is not a consumable item"); }
                                    }
                                    else { Console.WriteLine("There is no item to consume"); }
                                } else { Console.WriteLine("You have already consumed an item this turn!"); }
                                break;


                            case "Add":
                                for (; ; )
                                {
                                    Console.WriteLine("Enter item ID");
                                    read = Console.ReadLine();
                                    if (int.TryParse(read, out tempID))
                                    {
                                        player.SetItem(invID, tempID);
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
                                        player.weaponSlot = tempID;
                                        player.SetItem(invID, 0);
                                        activePlayer.Update();
                                    }
                                    else { Console.WriteLine("You can't equip that item!"); }
                                }
                                else { Console.WriteLine("There is no item to equip"); }
                                break;

                            default: break;
                        }
                        if (hasConsumed) { break; }
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
                            tempID = player.weaponSlot;
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
                                        if (player.FindEmpty() == -1)
                                        {
                                            Console.WriteLine("No empty inventory slots available");
                                            break;
                                        }
                                        player.SetItem(player.FindEmpty(), tempID);
                                        player.weaponSlot = 0;
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
                if (hasConsumed)
                {
                    endTurn = true;
                    break;
                }
                if (read == "back")
                {
                    read = "lul";
                    if (isBattle == false)
                    {
                        Console.WriteLine("Selected profile: [{0}]", player.Name);
                    }
                    break;
                }
            }
        }

        public static void ProfileSelected(PlayerProfile player)
        {
            activePlayer = player;
            Console.WriteLine("Selected profile: [{0}]", activePlayer.Name);
            for (; ; )
            {
                read = Console.ReadLine();
                switch (read)
                {
                    case "Fight":
                        Monster monster;
                        Console.WriteLine("What grade monster do you wish to fight?");
                        for (; ; )
                        {
                            read = Console.ReadLine();
                            int grade;
                            if (int.TryParse(read, out grade))
                            {
                                monster = generateMonster(grade);
                                break;
                            }
                            else { Console.WriteLine("Thats not a valid integer!"); }
                        }

                        Battle(monster);
                        break;

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
                                }
                                else { Console.WriteLine("You can't take negative damage!"); }
                            }
                            else { Console.WriteLine("That is not a valid integer!"); }
                        }
                        break;

                    case "ListLevelXp":
                        activePlayer.ListLevelXp();
                        break;

                    case "Name":
                        string getName = activePlayer.Name;
                        Console.WriteLine(getName);
                        break;

                    case "Class":
                        string getClass = activePlayer.Class;
                        Console.WriteLine(getClass);
                        break;

                    case I:
                        activePlayer.Stats();
                        break;

                    case "ModCash":
                        if (dev)
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
                        Inventory(activePlayer, false);
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
                    profiles.Remove(profiles.Find(x => x.Name == activePlayer.Name));
                    break;
                }
            }
        }

        public static void PopulateItems()
        {
            items[1] = new Item(name: "Iron Sword", damage: 10, equipable: true);
            items[2] = new Item(name: "Lesser Health Potion", healing: 4, consumable: true);
            items[3] = new Item(name: "Dark Totem");
            items[4] = new Item(name: "Flesh Blade", damage: 3, healing: 2, equipable: true, consumable: true);
            items[5] = new Item(name: "Saradomin Brew", healing: 16, consumable: true, stack: 4);
            items[6] = new Item(name: "2B2T", damage: 100, equipable: true);
        }

        public static Monster generateMonster(int grade = 1, string name = "Plant")
        {
            Monster monster = new Monster(
                name, 
                random.Next(grade, grade * 3) * 10,
                random.Next(grade, grade * 2),
                random.Next(grade, grade * 3),
                random.Next(grade * 2, grade * 5) * 5);
            return monster;
        }

        public static void Main()
        {
            profiles.Add(new PlayerProfile("Test", "Warrior"));
            playerCount++;
            PopulateItems();

            Console.WriteLine("Select or create character");
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

                    case "Select":
                        Console.WriteLine("What character do you wish to select?");
                        string select = Console.ReadLine();
                        if(profiles.Exists(x => x.Name == select))
                        {
                            PlayerProfile tempPlayer = profiles.Find(x => x.Name == select);
                            ProfileSelected(tempPlayer);
                        } else { Console.WriteLine("That character does not exist"); }
                        break;

                    default:
                        break;
                }
                if (read == "exit")
                {
                    break;
                }
            }
        }
    }
}
