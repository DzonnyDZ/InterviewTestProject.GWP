# How to run
## In Visual Studio
1. Make sure startup project is `Galytix.Test.Api`
2. Make sure selected configuration is `http`
3. Run the project (F5)
4. Browser should open with API Swagger page where the API can be tested

## Troubleshooting

- Browser does not open
  - When running the application navigate to http://localhost:9091/swagger/index.html
- CSV cannot be loaded
  - Make sure file referenced from `Galytix.Test.Api`'s `appsettings.json` (`CsvRepository.FilePath`) exists

# Implementation notes

- The architecture is overkill for such a small solution, but it's ready for project growth

# Bonus points notes

- Implement the calls asynchronously
  - ✔ Done, although there's not much async because the data are stored in and loaded from CSV file.
- Follow SOLID principles when designing the Client API, especially the use of DI/IoC
  - ✔ Followed 
- Decouple the database concerns with the business logic using a repository pattern to access the data
  - ✔ Done 
- Create a unit or E2E test to run  and test the data extraction code using framework of your choice, e.g. NUnit
  - ✔ There are some unit tests using NUnit although the coverage is not full 
- Provide some automated documentation of the API (e.g. using tools like Swagger) or create unit test to run the code
  - ✔ Swagger (OpenAPI) provided via Swashbuckle and Swagger UI at http://localhost:9091/swagger/index.html  
- Add some basic error handling
  - ✔ Error handling added, try
    - Remove the CSV file ⇒ 500, but _nice_ JSON response 
    - Pass invalid country code (not 2 lowercase letters a-z) ⇒ 400
    - Pass no line of business ⇒ 400
    - Pass duplicate line of business ⇒ 400
- Cache the results of each request and return the cached result if available
  - ✔ Basic cashing added at controller level 