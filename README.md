# PartnerQuest
PartnerQuest is a web application project designed to bring people together, create meaningful connections and help individuals find their ideal partner. Your journey to find meaningful connection begins here.

Technologies that have been used to build the web application are ***Angular 15***, ***.NET 7.0***, ***Bootstrap 5***, ***SignalR***, ***SQLite Database*** and ***Entity Framework Core***. *Microsoft Identity* has also been introduced in this project. Authentication and Authorization are handled by using ***JWT***.

## Functionalities Implemented in **PartnerQuest**
- User Registration system using **Reactive forms** of Angular. Persistent Login using **JSON Web Token (JWT)** Authorization.
- Quick Profile Card to show user informations.
- **Active Status Tracker** to show whether a user is active or not at any point of time. It also tracks the **Last Active** property of a user.
- Editing Own Profile info. Leaving the edit profile page with unsaved changes is also prevented using **Route Guards**.
- Photo Gallery and Photo Upload and Deleting functionality with the facility of setting a photo as the user's Profile Photo.
- Adding other users in one's **Favourite List**. Sending Toastr notification to the added user in such case.
- **Real-time Messaging** functionality between users using **SignalR**. Keeping track of messages being **Read** or staying **Unread**.
- **Toastr** notification on the arrival of new message from any user.
- **Sorting and Filtering** data depending on various parameters. **Pagination** for vast amount of data.
- **Role Management** is also implemented for the **admin** user.


## Project's Working Demo
https://github.com/AkashAhmed41/PartnerQuest/assets/86203010/f31e4104-e582-4f90-955f-de1d5d58eca8

## Installation & Starting Instructions
As a developer if you want to create a copy of the full working web application in your local machine, follow the instructions below:
### Prerequisites
To run the project locally, you need the following things installed in your machine:
- [Git](https://git-scm.com/downloads)
- [Node.js](https://nodejs.org/en/download)
- [.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) (Navigate to the link. Expand section 7.0.5, then find and install SDK 7.0.203)
- [Visual Studio Code](https://code.visualstudio.com/download)

Make sure that you've installed Node.js and ***npm*** properly. Then open your *Command Prompt* and run the following command there. This will install the required *[Angular CLI](https://www.npmjs.com/package/@angular/cli/v/15.0.3)* globally in your local machine.
```
npm install -g @angular/cli@15.0.3
```
### Installation & Running PartnerQuest
- Clone the repository: Go to the directory where you want to clone the project and run the following command:
  ```
  git clone https://github.com/AkashAhmed41/PartnerQuest.git
  ```
- Open the whole project folder using *Visual Studio Code*.
- Open a new terminal in **vscode** and go to the ```BackendWebApi``` directory using the command:``` cd BackendWebApi ```
- Run the following commands one after another in the terminal:
  ```
  dotnet restore
  dotnet run
  ```
  These commands will restore all the dependency packages and run the ***API's***.
- Open another new terminal in **vscode** and go to the ```FrontendPart``` directory using the command:``` cd FrontendPart ```
- Then run the following commands one after another in that terminal:
  ```
  npm i
  ng serve --open
  ```
  These commands will install all the packages that are needed to run the web application and open ***PartnerQuest*** on your default browser.

You then just need to register as a new user providing some informations to explore the application.

## Important Note
There is a ***Photo Upload*** functionality in the project which requires a [Cloudinary](https://cloudinary.com) account and couple lines of code to work with that. If you want to explore the functionality of ***Uploading Photos***, you can create a free account at [Cloudinary](https://cloudinary.com) just by using your *Gmail*.

Once you have your *Cloudinary Account*, go to the ```BackendWebApi``` folder and create a new file named ```appsettings.json```. Then open that file and provide the following lines of code:
```
{
    "AllowedHosts": "*",
    "CloudinarySettings": { 
        "CloudName": "your_CloudName",
        "ApiKey": "your_ApiKey",
        "ApiSecret": "your_ApiSecret"
    }
}
```
You will get your ```CloudName```, ```ApiKey```, ```ApiSecret``` from your *Cloudinary Dashboard*. Just use those and you are good to go.
## Developer Tip
You can test the functionalities like:
- Live Messaging via ***SignalRHub***
- ***Notification*** on Arrival of New Message
- Messages being ***Read*** or staying ***Unread***
- ***Active State Tracking*** of currently logged in users
- Adding an user in your ***Favourite List***
- Updation of Photos on other user's Uploading a new Photo etc.
by logging in as two different user from two different browsers!!

