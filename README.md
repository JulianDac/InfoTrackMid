# InfoTrackMid

A company, InfoTrack, offers a settlement service where a property purchaser's conveyancer meets with representatives of the mortgage provider and the vendor's conveyancer at a pre-arranged time. Due to fixed capacity, InfoTrack can only accommodate a limited number of simultaneous settlements. This API allows users to book a settlement time and receive a response indicating whether the reservation was successful.

## Functionalities

- **Single-Day bookings**: All bookings are assumed to be for the same day. No Date is available.

- **Business Hours**: Operates from 9:00 AM to 5:00 PM. Latest booking must be 4:00 PM.

- **Booking Duration**: 1 hour.

- **Simultaneous Settlements**: 4 concurrent bookings allowed in a single hour.

- **Booking Request format**: The API accepts GET, POST, PUT, DELETE. The POST request, i.e. making a booking, follows the following format:

  ```json
  {
    "bookingTime": "09:30",
    "name": "John Smith"
  }
  ```

and the success response will return a booking ID in GUID
  ```json
  {
    "bookingId": "d90f8c55-90a5-4537-a99d-c68242a6012b"
  }
  ```

- **Out of Hours Request: A Bad Request status will be returned.
- **Invalid Data Requests: Requests with invalid data return a Bad Request status.
- **Fully Booked Times: Requests for times when all settlement slots are reserved return a Conflict status. For example, there can never be a 5th booking in the same hour slot.
- **Name Validation: The name property must be a non-empty string.
- **Time Validation: The bookingTime property must be in 24-hour format (00:00 - 23:59).
- **Storage: Bookings are stored in the database.

## Set up Database
Option 1: Local SQL Server instance
1. Download SQL Server and SSMS
2. Run the following script to create the InfoTrack Database and Bookings table.
```sql
CREATE DATABASE InfoTrack;
GO

USE InfoTrack;
GO

CREATE TABLE Bookings (
    BookingId UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    BookingTime TIME NOT NULL,
    Name NVARCHAR(MAX) NOT NULL
);
GO
```

3. Check appsettings.json to ensure it's pointing to the right database.

Option 2: Use docker-compose
docker-compose.yml is available. It contains instructions on how to build a container for the app, and another docker volume for the database.
1. Create the Docker Volume
  docker volume create mssql-data
2. Build and run the containers
   docker-compose up --build -d
4. It will create and run both the app and the storage. To inspect the data, use Azure Data Studio.

## Set up the App using Docker
There is a Dockerfile. It can be used together with either database methods above.
1. Build the Docker image
   docker build -t settlementservicewebapi .
2. Run the container
   docker run -d -p 8080:80 --name settlementservice settlementservicewebapi
3. Then navigate to localhost:8080/swagger to open up the Swagger page.
4. Alternatively, just pull down the code and compile and run in Visual Studio.

## Code includes
1. Unit tests:
   There's SettlementServiceWebAPI.Tests. Simply run the tests in Test Explorer.
   This test uses WebApplicationFactory to test a Web API, following https://timdeschryver.dev/blog/how-to-test-your-csharp-web-api#a-simple-test
3. Validators:
   Validating incoming data is done at BookingRequestValidator.cs
4. BookingService and IBookingService implement service layer pattern
5. Separating models for Booking, BookingRequest, and BookingResponse
6. JWT Authentication: this feature is incomplete for now
