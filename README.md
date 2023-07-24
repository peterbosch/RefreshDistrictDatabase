# RefreshDistrictDB

## Description
This project assumes that the user is able to extract a CSV file of membership information from Clubrunner.

The CSV file is then used to create and populate (or if it already exists, destroy, then create and populate) a table called 'membership' in a MySql database.

## Installation and Setup
- Clone the repo.
- Provide your connection string, and the location of the CSV file in App.config
- [Obtain and install and then)[https://dev.mysql.com/downloads/] launch MySql Community Edition.
- [Obtain and](https://visualstudio.microsoft.com/vs/community/) launch Visual Studio Community Edition and load the RefreshDistrictDb project.
- Edit the App.config file to provide your connection string and the location of the CSV file.
- Build the project.
- Run the project.
- If it runs properly, the console window will display the number of rows in the membership table.
- If it does not run properly, the console window will display an error message.

## Obligatory
Downloading a CSV from ClubRunner is a feature that, if you have access to it, is a privilege. Analyzing that data in a database is no different from analyzing it in Excel. Respect and protect it.


Make sure you follow [Rotary's Personal Data Use Policy](https://my.rotary.org/en/personal-data-use-policy) when using this tool.