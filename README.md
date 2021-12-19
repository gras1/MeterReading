# MeterReading

Two parts to this technical task:-

1. create a small application that will create and populate a database with account details. The database will require three tables:
   - Accounts - Id is a primary key but not auto incrementing, seeded with the contents of the Test_Accounts.csv file
   - MeterReadings - Id is a primary key and auto incrementing, AccountId as a foreign key, AuditId as a foreign key
   - Audit - Id is a primary key, name of csv file uploaded, date time stamp when this was done, number of successful records imported and number of records that didn't import successfully with the reason

2. an API that exposes a POST endpoint "/meter-reading-uploads" that processes a CSV of meter readings. It should follow RESTful standards and report on the number of successful readings and failed readings. Store the import to an audit table. Importing the data must follow the following validation: no duplicate data / wrong data format / invalid account id

To get started:
