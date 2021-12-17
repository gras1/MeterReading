# MeterReading

Two parts to this technical task:-

1. create a small application that will create and populate a database with account details. The database will require three tables:
   - Accounts - Id is a primary key but not auto incrementing, seeded with the contents of the Test_Accounts.csv file
   - MeterReadings - Id is a primary key and auto incrementing, AccountId as a foreign key, AuditId as a foreign key
   - Audit - Id is a primary key, name of csv file uploaded, date time stamp when this was done, number of successful records imported and number of records that didn't import successfully with the reason

2. an API that exposes a POST endpoint "/meter-reading-uploads" that processes a CSV of meter readings. It should follow RESTful standards and report on the number of successful readings and failed readings. Store the import to an audit table. Importing the data must follow the following validation: no duplicate data / wrong data format / invalid account id

Steps in VSCode ...
1. in a Terminal window from the repo root, type:
dotnet new console -f net6.0 -n Ensek.Net.MeterReading.DbSetup

2. in the Terminal window cd in to the Ensek.Net.MeterReading.DbSetup folder, then type:
dotnet add package Microsoft.Data.Sqlite

3. in the Terminal window from the repo root, type:
dotnet new classlib -f net6.0 -n Ensek.Net.MeterReading.Data

4. in the Terminal window cd in to the Ensek.Net.MeterReading.Data folder, then type:
dotnet add package Microsoft.Data.Sqlite
dotnet add package Microsoft.Extensions.Options
dotnet add package Ardalis.GuardClauses

5. in the Terminal window from the repo root, type:
dotnet new xunit -f net6.0 -n Ensek.Net.MeterReading.Data.Tests.Unit

6. in the Terminal window cd in to the Ensek.Net.MeterReading.Data.Tests.Unit folder, then type:
dotnet add package Microsoft.Data.Sqlite
dotnet add package Microsoft.Extensions.Options
dotnet add package FluentAssertions
dotnet add package FakeItEasy

7. in the Terminal window from the repo root, type:
dotnet new classlib -f net6.0 -n Ensek.Net.MeterReading.Dtos

8. in the Terminal window from the repo root, type:
dotnet new webapi -f net6.0 -n Ensek.Net.MeterReading.Api

9. in the Terminal window from the repo root, type:
dotnet new xunit -f net6.0 -n Ensek.Net.MeterReading.Api.Tests.Unit

10. in the Terminal window cd in to the Ensek.Net.MeterReading.Api.Tests.Unit folder, then type:
dotnet add package FluentAssertions

11. in the Terminal window from the repo root, type:
dotnet new xunit -f net6.0 -n Ensek.Net.MeterReading.Api.Tests.Integration

12. in the Terminal window cd in to the Ensek.Net.MeterReading.Api.Tests.Integration folder, then type:
dotnet add package FluentAssertions
dotnet add package Xunit.Gherkin.Quick

13. create a solution file, in the Terminal window from the repo root, type:
dotnet new sln --name Ensek.Net.MeterReading.sln

14. add project references to the solution, in the Terminal window from the repo root, type:
dotnet sln add ./Ensek.Net.MeterReading.DbSetup/Ensek.Net.MeterReading.DbSetup.csproj
dotnet sln add ./Ensek.Net.MeterReading.Data/Ensek.Net.MeterReading.Data.csproj
dotnet sln add ./Ensek.Net.MeterReading.Data.Tests.Unit/Ensek.Net.MeterReading.Data.Tests.Unit.csproj
dotnet sln add ./Ensek.Net.MeterReading.Dtos/Ensek.Net.MeterReading.Dtos.csproj
dotnet sln add ./Ensek.Net.MeterReading.Api/Ensek.Net.MeterReading.Api.csproj
dotnet sln add ./Ensek.Net.MeterReading.Api.Tests.Unit/Ensek.Net.MeterReading.Api.Tests.Unit.csproj
dotnet sln add ./Ensek.Net.MeterReading.Api.Tests.Integration/Ensek.Net.MeterReading.Api.Tests.Integration.csproj

15. add the inter project references, in the Terminal window from the repo root, type:
dotnet add ./Ensek.Net.MeterReading.Data/Ensek.Net.MeterReading.Data.csproj reference ./Ensek.Net.MeterReading.Dtos/Ensek.Net.MeterReading.Dtos.csproj
dotnet add ./Ensek.Net.MeterReading.Data.Tests.Unit/Ensek.Net.MeterReading.Data.Tests.Unit.csproj reference ./Ensek.Net.MeterReading.Data/Ensek.Net.MeterReading.Data.csproj
dotnet add ./Ensek.Net.MeterReading.Data.Tests.Unit/Ensek.Net.MeterReading.Data.Tests.Unit.csproj reference ./Ensek.Net.MeterReading.Dtos/Ensek.Net.MeterReading.Dtos.csproj
dotnet add ./Ensek.Net.MeterReading.Api/Ensek.Net.MeterReading.Api.csproj reference ./Ensek.Net.MeterReading.Dtos/Ensek.Net.MeterReading.Dtos.csproj
dotnet add ./Ensek.Net.MeterReading.Api.Tests.Unit/Ensek.Net.MeterReading.Api.Tests.Unit.csproj reference ./Ensek.Net.MeterReading.Api/Ensek.Net.MeterReading.Api.csproj
dotnet add ./Ensek.Net.MeterReading.Api.Tests.Unit/Ensek.Net.MeterReading.Api.Tests.Unit.csproj reference ./Ensek.Net.MeterReading.Dtos/Ensek.Net.MeterReading.Dtos.csproj
dotnet add ./Ensek.Net.MeterReading.Api.Tests.Integration/Ensek.Net.MeterReading.Api.Tests.Integration.csproj reference ./Ensek.Net.MeterReading.Api/Ensek.Net.MeterReading.Api.csproj

16. Added a DatabaseOptions class that has a property for a ConnectionString

17. Added a skeleton BaseRepository class, then added BaseRepositoryTests class that tests if a null IOptions is passed in to the constructor of BaseRepository that an ArgumentNullException is thrown, then I added the implementation. I then added a test that checks that if a ConnectionString is added to the fake/mock IOptions constructor parameter that it can be retrieved from the BaseRepository

18. Created an IAuditRepository interface that defines two methods, one called CreateNewAuditRecord and the other called UpdateAuditRecord.

19. Added a new class called AuditRepository that implements the interface and an AuditRepositoryTests class. Leave the default implementation of the methods in AuditRepository with ThrowNotImplementedException so that I could then start adding unit tests.

NB: I realised after the commit 1am 15/12/2021 that I would have to be creative unit testing the AuditRepository, in the end I decided to unit test it with an in-memory defined SQLite database, the only drawback of this is I would need to re-create the database table structure in each unit test and leave the SqliteConnection open when calling AuditRepository

20. Created a Guard clause extension method to check that if there were failed records that didn't import correctly that there is some description of what didn't work

21. Created a custom exception to handle when an audit record doesn't exist when you try and update it

22. Created a custom exception when a meter reading record could not be created