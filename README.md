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

- **Out of Hours Request**: A Bad Request status will be returned.
- **Invalid Data Requests**: Requests with invalid data return a Bad Request status.
- **Fully Booked Times**: Requests for times when all settlement slots are reserved return a Conflict status. For example, there can never be a 5th booking in the same hour slot.
- **Name Validation**: The name property must be a non-empty string.
- **Time Validation**: The bookingTime property must be in 24-hour format (00:00 - 23:59).
- **Storage**: Bookings are stored in the database.

## Set up Database
There is no database in this version. Bookings are in-memory.

## Set up the App using Docker
There is a Dockerfile.
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

## Instructions on the endpoints to contact
1. Get Available Slots
Endpoint: GET /booking/available-slots
Description: Retrieves a list of available time slots for bookings.
Response:
200 OK: Returns a list of available time slots.
500 Internal Server Error: An error occurred while processing the request.
```bash
curl -X GET "https://localhost:5000/booking/available-slots"
```

2. Get Booking by Name
Endpoint: GET /booking/{name}
Description: Retrieves the booking details for a specific user by name.
Parameters:
name (string): The name of the user whose booking details are to be retrieved.
Response:
200 OK: Returns the booking details for the specified user.
404 Not Found: No booking found for the specified user.
500 Internal Server Error: An error occurred while processing the request.
 ```bash
curl -X GET "https://localhost:5000/booking/Alice"
```

3. Create Booking
Endpoint: POST /booking
Description: Creates a new booking.
Request Body:
BookingRequest (JSON):
name (string): The name of the user making the booking.
bookingTime (string): The desired booking time in the format "HH:mm".
Response:
200 OK: Returns the booking ID of the newly created booking.
400 Bad Request: Validation errors or invalid input.
409 Conflict: A booking already exists for the specified user or no available slots for the requested time.
500 Internal Server Error: An error occurred while processing the request.
 ```bash
curl -X POST "https://localhost:5000/booking" -H "Content-Type: application/json" -d '{"name": "Alice", "bookingTime": "09:00"}'
```

4. Amend Booking
Endpoint: PUT /booking/{name}
Description: Amends an existing booking for a specific user by name.
Parameters:
name (string): The name of the user whose booking is to be amended.
Request Body:
BookingRequest (JSON):
bookingTime (string): The new desired booking time in the format "HH:mm".
Response:
200 OK: Returns the booking ID of the amended booking.
400 Bad Request: Validation errors or invalid input.
404 Not Found: No booking found for the specified user.
409 Conflict: No available slots for the requested time.
500 Internal Server Error: An error occurred while processing the request
```bash
curl -X POST "https://localhost:5000" -H "Content-Type: application/json" -d '{"name": "Alice", "bookingTime": "09:00"}'
```

5. Delete Booking
Endpoint: DELETE /booking/{name}
Description: Deletes an existing booking for a specific user by name.
Parameters:
name (string): The name of the user whose booking is to be deleted.
Response:
204 No Content: The booking was successfully deleted.
404 Not Found: No booking found for the specified user.
500 Internal Server Error: An error occurred while processing the request.
```bash
curl -X DELETE "https://localhost:5000/booking/Alice"
```

## To test Make bookings and other functions In SWAGGER
1. Simply test out the POST/Booking by adding in a time and a person's name and Execute
![image](https://github.com/user-attachments/assets/66f8126f-63ed-411d-a6ab-b44df47d270e)

2. Get available time slots will return all the possible time slots that can be booked
![image](https://github.com/user-attachments/assets/23ecbb19-e25e-4e5e-a6b8-c362ca89acdc)

3. Other API calls such as PUT and DELETE will also work as intended


