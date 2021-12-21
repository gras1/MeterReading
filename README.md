# MeterReading

Two parts to this technical task:-

1. create a small application that will create and populate a database with account details. The database will require three tables:
   - Accounts - Id is a primary key but not auto incrementing, seeded with the contents of the Test_Accounts.csv file
   - MeterReadings - Id is a primary key and auto incrementing, AccountId as a foreign key, AuditId as a foreign key
   - Audit - Id is a primary key, name of csv file uploaded, date time stamp when this was done, number of successful records imported and number of records that didn't import successfully with the reason

2. an API that exposes a POST endpoint "/meter-reading-uploads" that processes a CSV of meter readings. It should follow RESTful standards and report on the number of successful readings and failed readings. Store the import to an audit table. Importing the data must follow the following validation: no duplicate data / wrong data format / invalid account id

After cloning the repo, to get started:
1. ensure you have the .NET 6 SDK installed on your computer

2. clone the repo: https://github.com/gras1/meterreading

3. in a Terminal window cd in to the MeterReading root folder and type 'dotnet build' - this will install the nuget packages and compile the projects

4. in a Terminal window cd in to the Ensek.Net.MeterReading.DbSetup project folder and type 'dotnet run' - this will create the Sqlite database in the root of that folder called MeterReadings.db and populate it with the contents of the Test_Accounts.csv file

5. edit the appsettings.json file in the Ensek.Net.MeterReading.Api project folder and change the folder location path to the MeterReadings.db in the Ensek.Net.MeterReading.Data to suit your setup and operating system

6. edit the testsettings.json file in the Ensek.Net.MeterReading.Api.Tests.Integration project folder and change the folder location path to MeterReadings.db in the Ensek.Net.MeterReading.Data to suit your setup and operating system

7. in a Terminal window cd in to the Ensek.Net.MeterReading.Api project folder and type 'dotnet run', then in a browser go to http://localhost:5139/index.html. This presents the swagger window. Click the 'Try it out' button, select the Meter_Reading.csv file, untick the 'Send empty value' option and click the 'Execute' button. Once it has successfully run, take a look at the response body and it should provide a full breakdown of import with full details of the records that didn't validate correctly or failed to import in to the database.

*NB:* ensure the meter reading csv file uploaded is saved as either UTF7 or UTF8 encoding
*NB:* make sure that DBSetup project is run before running the integration test
