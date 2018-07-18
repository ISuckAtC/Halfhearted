# Halfhearted
So this little thing is a console application that (for now) lets you create a character and create a class.

Your character has an inventory of 10 slots and 1 equip slot (the weapon slot), and some stats, health, skill, and experience.

The application works as a menu system where you'll have to navigate through menus through typing on the command line.

If you type in an invalid command the application will simply prompt you to type in a new line, it wont crash (hopefully).

-- [MENU MAP] --

This is a compressed and basic menu map, commands are explained in detail further below. [INT] means you have to input an integer, and dev commands will have a (!) behind them.

- Main

  - Create
  - [INT]
    - Inventory
      - [INT]
        - Add (!)
        - Info
        - Equip
      - Weapon
        - Info
        - Unequip
    - getStats
    - getName (!)
    - getClass (!)
    - modCash (!)
    - listLevelXp (!)
      - total
      - diff
  - DEVMODE
    - Password


Overall commands

 - exit (this command is only usable on the Start menu and will exit the application
 - back (this command is usable in most areas of the application and will take you to the previous menu, if you want to exit just use this command until you reach the Start menu)

[Start]

Here you will get a prompt to either create or select a character. In this current build the first character slot will always be taken by a Test character with the character class "Warrior". Each time you create a character it will be assigned an ID which goes from 0-99, you can thus make up to 100 characters (in this build). Creating a character will let you pick a name and character class and will then save that character in memory. 

Commands
 - Create (Will start creating a new character)
 - ID (Type in the ID of the character you'd like to access, this is a number between 0 and 99, this will take you to [Character Menu])
 
 
 [Character Menu]
 
 Here you can interact with your character through different commands.
 
 Commands
 
  - Inventory (Opens the inventory, taking you to the inventory select)
  - getName (Returns the name of your character)
  - getClass (Returns the character class of your character)
  - getStats (Lists the stats of your character, including name and class)
  - modCash (Add or remove gold from the character, single integer input, so use minus to remove)
  - listLevelXp (Dev command which lets you see how much XP is needed for different levels)
  
  
 [Inventory Select]
 
 Here you can either select an inventory slot or an equip slot (currently the only equip slot is the "Weapon" slot.
 
 Commands
 
  - ID (Type in the ID of the inventory slot you wish to select, this will take you to the inventory slot you chose. (in this build a character has 10 inventory slots, with IDs from 0 to 9))
  
  - Weapon (This will take you to the weapon slot)
  
 [Inventory Slot]
 
 Here you can add an item to a slot, equip the item to an equip slot (the only equip slot is the weapon slot currently) and display info about an item in that specific slot.
 
 Commands
 
  - Add (Adds an item to the selected inventory slot (Item IDs range from integers 0 to 99, item ID 0 is an empty slot, and the only items added in this build have the IDs [1] to [4].
  
  - Info (Displays info about the item in the selected inventory slot)
  
  - Equip (Equips the item in the selected inventory slot to your Weapon slot)
  
 [Weapon Slot]
 
 Here you can display info about the item currently equipped and unequip the equipped item
 
 Commands
 
  - Info (Displays info about the currently equipped item)
  
  - Unequip (Unequips the currently equipped item, and puts it in your first empty inventory slot)
