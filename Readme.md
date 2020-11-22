# OpenMod Installer for RocketMod
This plugin allows you to install RocketMod and migrate to OpenMod with only one command.


# How to use
Download the latest OpenModInstaller.dll from the [Releases](https://github.com/openmod/OpenMod.Installer.RocketMod/releases/) tab and install it on your RocketMod server.  
After that type `/openmod install` in console.   
Some options are also available:
* --no-permission-link: Will not install permission link. Includes --no-migration.
* --no-migration: Do not migrate data, from RocketMod
* --no-extras: Do not install extra plugins like Cooldowns or Economy.
* --no-uconomy-link: Do not redirect Uconomy economy to OpenMod economy.

# Reverting back
To revert back, type `/openmod uninstall`.