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
| IMeetingRepository  | Contract for Meeting related logic |
| IMeetingRoomRepository  | Contract for Meeting Room related logic |
| ITokenService  | Contract for Token related logic  |