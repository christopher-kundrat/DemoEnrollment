This will be a pretty standard API utilizing Swagger. 
Running the application will bring up the swagger page where you can see the API endpoints and test them.

To manage a demo database I'm utilizing SQLite. If you wish to start from scratch just delete the Data/demoEnrollment.db file.
Demo data is automatically being inserted from program.cs as part of the CreateDB function so that can be updatd as needed. 

Authentication would get handled in the controllers that have been setup but authentication is not really a thing with this demo.

Structural wise is pretty standard. Controller, Service Model, with the entities broken out into request entities and response entities, etc. 
API endpoint is setup in the controller which then get's passed over to the service.
There we check business logic and craft the response. Any data calls get sent over to a dataservice for processing.


In the dataservice EntityFramework could have been an option but straight SQL was used to showcase it. 