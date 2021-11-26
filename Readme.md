# Meeting room reservation

### Video demo: <https://youtu.be/qHNu00-GXPk>

### Description:

#### Functional

With the meeting room reservation application it is possible to add physical meeting rooms and reserve them. This will be possible with a simple to use interface. 
It is also possible to show the state of a meeting room using a table, tv or other display device. 
You can select a meeting room and click 'Display' to see the current state of the meeting room (Available, almost reserved and reserved). 
The upcoming meetings of the current day will also be displayed.

#### Technical 

##### Usages

This MVC design pattern application is written in the laguage c# (C Sharp), using .NET 5 for runtime and Entity Framework framework for database communication. 
Sqlite is used for data storage, although other Sql could also be used if the product will go to production.
For client side logic raw javascript is used and Bootstrap is used for UI related components

To be sure users are not able to access data directly from the database the repository patters is used. 
This means that database related actions are done in a place a user cannot access. 
This is done so the controllers won't have to much logic implemented and safety for data manipulation.

##### Files
###### Interfaces
Interfaces are used to define contracts for the classes that will implement the logic. Implementation details will be explained when the respective classes are up. 

| Interface name  | Usage |
| ------------- | ------------- |
| IAccountRepository  | Contract for account related logic  |
| IMeetingRepository  | Contract for Meeting related logic |
| IMeetingRoomRepository  | Contract for Meeting Room related logic |
| ITokenService  | Contract for Token related logic  |

###### Repositories
**AccountRepostiroy**

| Method | Public/Private | Usage |
| ------------- | ------------- | ------------- |
| GetCompany  | Public | Get company registered with current token from memory cache |
| Login  | Public | Check if the user and password exist in the database and are correct, if so add company to memoy cache |
| Logout  | Public | Log user out (Remove from memory cache) |
| Register | Public | Create a new company record in the database |
| CompanyExist | Private | Check if given company name already exists in the database |

**MeetingRepository**

| Method | Public/Private | Usage |
| ------------- | ------------- | ------------- |
| GetCompanyList  | Public | Get list of meetings belonging to your company |
| GetMeetingRoomList  | Public | Get list of meetings belonging to the selected meeting room |
| GetSingle  | Public | Get selected meeting |
| Create | Public | Create a new meeting record in the database |
| Delete | Pubic | Delete selected meeting from database |
| Update | Pubic | Update selected meeting |
| IsAllowedMeetingRoom | Private | Check if the user is allowed to used the given meeting room |
| IsOverLapping | Private | Check if a meeting overlaps with an existing meeting (One for create and one for update) |
| InCorrectDates | Private | Check if meeting enddate is greater then the startdate |
| AddAttendees | Private | Create an Attendee object with the list of given attendees |
| AddRemoveOrUpdateAttendees | Private | Check if attendees need to be added or removed when updating a meeting |

**MeetingRoomRepository**

| Method | Public/Private | Usage |
| ------------- | ------------- | ------------- |
| GetList  | Public | Get list of meeting rooms belonging to your company |
| GetSingle  | Public | Get selected meeting room |
| Create | Public | Create a new meeting room record in the database |
| Update | Pubic | Update selected meeting room |
| Delete | Pubic | Delete selected meeting room from database |
| MeetingRoomExist | Private | Check if meeting room already exist |

###### Services
**TokenService**

| Method | Public/Private | Usage |
| ------------- | ------------- | ------------- |
| BuildToken  | Public | Build a simple valid JWT token |
| ValidateToken  | Public | Validate if the given JWT token is valid |

###### Exceptions

Some custom exceptions have been made for better exception handling. This way it will be easier to show logical errors to the user.

**AccountExceptions** will be thrown when a user executes invalid account actions.
**InvalidMeetingRoomOperationException** will be thrown when a user tries to execute actions with a meeting room they are not allowed to use.
**MeetingRoomException** will be thrown when a user executes wrong meeting or meeting room actions.

###### Controllers
Controllers are the endpoints that are used to GET or POST data from and to the application. 
Repositories are injected in them to communicate with the database. 
Before sending information to the view for the user to see, entities are mapped to ViewModels

**AccountController**

| Method | Http Method | Public/Private  | Usage |
| ------------- | ------------- | ------------- | ------------- |
| Login | GET | Public | Get the login view |
| Login | POST | Public | Log a user in and redirect to the meeting room index |
| Register | GET | Public | Get the register view |
| Register | POST | Public | Register a new company and redirect to the login view |
| Logout | GET | Public | Log a user out and redirect to the login page |
| GetToken | N/A | Private | Get JWT from session |

**DisplayController**

| Method | Http Method | Public/Private  | Usage |
| ------------- | ------------- | ------------- | ------------- |
| Index | GET | Public | Get view to select a meeting room to display |
| Meetings | GET | Public | Get meetings for selected meeting room and return view where we show the upcoming meetings |
| GetMeetings | GET | Public | Endpoint to get meetings for selected meeting room. This endpoint is used for an AJAX request |
| MapMeetingsToViewModel | N/A | Private | Map Meeting entity to a ViewModel |
| GetToken | N/A | Private | Get JWT from session |

**HomeController**

| Method | Http Method | Public/Private  | Usage |
| ------------- | ------------- | ------------- | ------------- |
| Index | GET | Public | Get landing page view |
| Error | GET | Public | Get genere error view |

**MeetingController**

| Method | Http Method | Public/Private  | Usage |
| ------------- | ------------- | ------------- | ------------- |
| Index | GET | Public | Get meetings and show view with list of meetings |
| Details | GET | Public | Get meeting and show details view |
| Create | GET | Public | Get view to create a new meeting |
| Create | POST | Public | Create a new meeting and redirect to meeting details |
| Update | GET | Public | Get meeting and show view to update meeting |
| Update | POST | Public | Post updated values for meeting and redirect to details |
| Delete | GET | Public | Get meeting and show view to delete meeting |
| DeleteConfirmed | POST | Public | Delete selected meeting and redirect to index |
| GetToken | N/A | Private | Get JWT from session |
| MapMeetingViewModel  | N/A | Private | Map meeting to viewmodel |
| GetMeetingRooms  | N/A | Private | Get and map meeting rooms to viewmodels |

**MeetingRoomController**

| Method | Http Method | Public/Private  | Usage |
| ------------- | ------------- | ------------- | ------------- |
| Index | GET | Public | Get meeting rooms and show view with list of meeting rooms |
| Create | GET | Public | Get view to create a new meeting room |
| Create | POST | Public | Create a new meeting room and redirect to meeting room index |
| Update | GET | Public | Get meeting room and show view to update meeting room |
| Update | POST | Public | Post updated values for meeting room and redirect to index |
| Delete | GET | Public | Get meeting room and show view to delete meeting room |
| DeleteConfirmed | POST | Public | Delete selected meeting room and redirect to index |
| GetToken | N/A | Private | Get JWT from session |
| MapToViewModel  | N/A | Private | Map meeting room to viewmodel |


