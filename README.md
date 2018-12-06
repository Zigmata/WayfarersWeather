# WayfarersWeather
Weather Bot for /r/WayfarersPub

## What is this thing?
This is a bot designed to use pre-determined weather patterns and generates weather based on the current real-world season. Results are posted to a meta thread in http://www.reddit.com/r/wayfarerspub.

## How does it work?
Right now, the bot is hard-coded with logic to determine the current season, and phase of day (day or night) based off of Coordinated Universal Time. The program runs as a windows service, and reads a JSON-formatted text file full of descriptive weather patterns, sorted by season and phase of day. The bot simply figures out what time it is, and then selects a random weather from the appropriate array.

## Upcoming Features
Eventually, WeatherBot will be updated with a GUI configuration tool, and currently hard-coded properties like weather probability will be configurable from the GUI, allowing for more customization. Long-term plans are to implement the Google Docs API to allow the bot to read new weather submissions from a spreadsheet-linked form, and update itself daily.

## Can I Contribute?
Right now, collaboration is limited to approved members of the WayfarersPub. But feel free to clone the repo and make your own modifications! You will need to know a bit about the Reddit Application process, and JSON formatting, to utilize the code as-is for your own bot.
