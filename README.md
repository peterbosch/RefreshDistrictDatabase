# RefreshDistrictDB

## Description
This project assumes that the user is able to extract a CSV file of membership information from Clubrunner.

The CSV file is then used to create and populate (or if it already exists, destroy, then create and populate) a table called 'membership' in a MySql database.

## Installation and Setup
- Clone the repo.
- Provide your connection string, and the location of the CSV file in App.config

## Obligatory
Downloading a CSV from ClubRunner is a feature that, if you have access to it, is a privilege. Analyzing that data in a database is no different from analyzing it in Excel. Respect and protect it.


Make sure you follow [Rotary's Personal Data Use Policy](https://my.rotary.org/en/personal-data-use-policy) when using this tool.