# Stratum-bot

This trading bot used to be a private product between 2017 and 2022. I have no idea if it works now, but there is still demand for this software. I’ve decided to make it open-source two years ago and... Here we are. I’ve removed all the licensing parts today. There are some bugs, a lot of refactoring required, and some messy code. Honestly, it's quite sad how much time I’ve invested in these lines of code and classes that have been rewritten multiple times.

It was originally designed to work with Binance (both spot and futures).

Currently, three strategies are implemented: scalping, long, and short.

You can extend the functionality by adding new strategies and exchanges using the appropriate interfaces.

Features:
- DCA (Dollar-Cost Averaging)
- Filters and indicators

To run the bot, you will need a "Logs" folder in the build directory and a settings file (grab it from the release).

## Known Issues:
- The backend API used to store filter configurations no longer works. You’ll need to move the configurations to local storage.

## Community 

https://t.me/bablobtn