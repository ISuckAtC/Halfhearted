using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CSharp_Shell
{
    public class Item
    {
        public string itemName;
        
        public int itemDamage;
        public int itemHealing;
        
        public bool isEquipable;
        public bool isConsumable;
        
        public Item(string name, int damage = 0, bool equipable = false, bool consumable = false, int healing = 0)
        {
            itemName = name;
            itemDamage = damage;
            itemHealing = healing;
            isEquipable = equipable;
            isConsumable = consumable;
        }
        public void infoDump()
        {
            Console.WriteLine(itemName);
            if (itemDamage!=0)
            {
                Console.WriteLine("Damage: {0}",itemDamage);
            }
            if (itemHealing!=0)
            {
                Console.WriteLine("Healing: {0}",itemHealing);
            }
            
        }
    }
    
    public class PlayerProfile
    {
        private string playerName;
        private string playerClass;
        
        private int health;
        private int skill;
        private int xp;
        private int level;
        private int damage;
        private int maxLevel;
        private int maxInv;
        
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
            
            levelXp = new int[maxLevel];
            inventory = new int[maxInv];
            
            levelXp[0] = startXp;
            
            
            switch(playerClass)
            {
                case "Knight":
                health = 200;
                skill = 8;
                break;
                
                case "Warrior":
                health = 150;
                skill = 10;
                break;
                
                case "Thief":
                health = 100;
                skill = 13;
                break;
                
                case "Kieran":
                health = 30;
                skill = -5;
                break;
                
                default:
                health = 140;
                skill = 10;
                break;
            }
            
            for (int x = level; x < maxLevel; x++)
            {
                levelXp[x] = Convert.ToInt32(levelXp[x-1] * XpX);
            }
            
            updateStats();
        }
        private void updateStats()
        {
            damage = skill/2;
            
            if(weaponSlot!=0)
            {
                damage += Program.returnItem(weaponSlot).itemDamage;
            }
        }
        public void listLevelXp()
        {
            string read = Console.ReadLine();
            bool total;
            int tempXp;
            
            switch(read)
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
            for(int x = 1; x < maxLevel; x++)
            {
                if (total == true)
                {
                    tempXp = levelXp[x];
                } else tempXp = levelXp[x] - levelXp[x-1];
                Console.WriteLine("[Level]: {0} || [XP to Level {1}]: {2}",tempLevel,tempLevel + 1,tempXp);
                tempLevel++;
            }
        }
        public string getName()
        {
            if (Program.dev == true)
            {
                return playerName;
            } else return "";
        }
        public string getClass()
        {
            if (Program.dev == true)
            {
                return playerClass;
            } else return "";
        }
        public void getStats()
        {
            Console.WriteLine("[Name]: {0}",playerName);
            Console.WriteLine("[Class]: {0}",playerClass);
            Console.WriteLine("[Level]: {0}",level);
            Console.WriteLine("[Health]: {0}",health);
            Console.WriteLine("[Skill]: {0}",skill);
            Console.WriteLine("[Experience]: {0}",xp);
            Console.WriteLine("[Money]: {0} gold",cash);
        }
        public void modCash(int amount)
        {
            cash += amount;
            string joker = "added";
            string jokker = "to";
            if (amount < 0)
            {
                joker = "removed";
                jokker = "from";
                amount = - amount;
            }
            Console.WriteLine("You {0} {1} gold {2} {3}",joker,amount,jokker,playerName);
        }
        public void setItem(int invID, int itemID)
        {
            inventory[invID] = itemID;
        }
        public int itemInfo(int invID)
        {
            return inventory[invID];
        }
        public int findEmpty()
        {
            int temp = -1;
            foreach(int a in inventory)
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
        
        static PlayerProfile[] profiles = new PlayerProfile[99];
        static Item[] itemID = new Item[99];
        static int playerCount = 0;
        static int profileID;
        static int invID;
        static string read;
        static int tempID;
        public static bool dev = true;
        
        
        public static void createPlayer()
        {
            Console.WriteLine("What is your player name?");
            string pName = Console.ReadLine();
            Console.WriteLine("What is your player class?");
            string pClass = Console.ReadLine();
            profiles[playerCount] = new PlayerProfile(pName,pClass);
            playerCount++;
        }
        
        public static void populateItems()
        {
            itemID[1] = new Item(name: "Iron Sword", damage: 10, equipable: true);
            itemID[2] = new Item(name: "Lesser Health Potion", healing: 5, consumable: true);
            itemID[3] = new Item(name: "Dark Totem");
            itemID[4] = new Item(name: "Flesh Blade", damage: 3, healing: 2, equipable: true, consumable: true);
        }
        
        public static Item returnItem(int ID)
        {
            return itemID[ID];
        }
        
        public static void Main() 
        {
            profiles[playerCount] = new PlayerProfile("Test","Warrior");
            playerCount++;
            populateItems();
            
            for(;;)
            {
                Console.WriteLine("Create or select profile");
                read = Console.ReadLine();
                switch(read)
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
                    createPlayer();
                    break;
                    
                    default:
                    break;
                }
                if(int.TryParse(read,out profileID))
                {
                    Console.WriteLine("Selected profile ID: {0}",profileID);
                    for(;;)
                    {
                        read = Console.ReadLine();
                        switch(read)
                        {
                            case "listLevelXp":
                            profiles[profileID].listLevelXp();
                            break;
                            
                            case "getName":
                            string getName = profiles[profileID].getName();
                            Console.WriteLine(getName);
                            break;
                            
                            case "getClass":
                            string getClass = profiles[profileID].getClass();
                            Console.WriteLine(getClass);
                            break;
                            
                            case "getStats":
                            profiles[profileID].getStats();
                            break;
                            
                            case "modCash":
                            if (dev == true)
                            {
                                Console.WriteLine("How much money do you want to add/remove?");
                                int amount;
                                if(int.TryParse(Console.ReadLine(),out amount))
                                {
                                    profiles[profileID].modCash(amount);
                                    break;
                                } else {break;}
                            }
                            break;
                            
                            case "Inventory":
                            for(;;)
                            {
                                string name = profiles[profileID].getName();
                                Console.WriteLine("[Inventory of {0}]",name);
                                Console.WriteLine("Select an inventory or equipment slot");
                                read = Console.ReadLine();
                                if (int.TryParse(read, out invID))
                                {
                                    Console.WriteLine("Selected inventory slot [{0}]",invID);
                                    for(;;)
                                    {
                                        if (invID > 9 || invID < 0)
                                        {
                                            Console.WriteLine("That is not a valid inventory slot");
                                            break;
                                        }
                                        tempID = profiles[profileID].itemInfo(invID);
                                        read = Console.ReadLine();
                                        switch(read)
                                        {
                                            case "Add":
                                            for(;;)
                                            {
                                                Console.WriteLine("Enter item ID");
                                                read = Console.ReadLine();
                                                if (int.TryParse(read, out tempID))
                                                {
                                                    profiles[profileID].setItem(invID,tempID);
                                                    break;
                                                }
                                            }
                                            break;
                                        
                                            case "Info":
                                            if(tempID!=0)
                                            {
                                                itemID[tempID].infoDump();
                                            }
                                            break;
                                            
                                            case "Equip":
                                            if (itemID[tempID].isEquipable == true)
                                            {
                                                profiles[profileID].weaponSlot = tempID;
                                                profiles[profileID].setItem(invID,0);
                                            } else {Console.WriteLine("You can't equip that item!");}
                                            break;
                                        
                                            default:break;
                                        }
                                        if(read == "back")
                                        {
                                            read = "lul";
                                            break;
                                        }
                                    }
                                    
                                }
                                switch(read)
                                {
                                    case "Weapon":
                                    Console.WriteLine("Weapon Slot");
                                    for(;;)
                                    {
                                        tempID = profiles[profileID].weaponSlot;
                                        read = Console.ReadLine();
                                        switch(read)
                                        {
                                            case "Info":
                                            if(tempID!=0)
                                            {
                                                itemID[tempID].infoDump();
                                            } else {Console.WriteLine("There is nothing equipped in this slot");}
                                            break;
                                            
                                            case "Unequip":
                                            if(tempID!=0)
                                            {
                                                if (profiles[profileID].findEmpty() == -1)
                                                {
                                                    Console.WriteLine("No empty inventory slots available");
                                                    break;
                                                }
                                                profiles[profileID].setItem(profiles[profileID].findEmpty(),tempID);
                                                profiles[profileID].weaponSlot = 0;
                                            } else {Console.WriteLine("There is nothing equipped in this slot");}
                                            break;
                                            
                                            default:break;
                                        }
                                        if(read == "back")
                                        {
                                            read = "lul";
                                            break;
                                        }
                                    }
                                    break;
                                    
                                    default:break;
                                }
                                if(read == "back")
                                {
                                    read = "lul";
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
