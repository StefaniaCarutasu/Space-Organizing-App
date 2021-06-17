# Space-Organizing-App

## User Stories
1) As a student, I want a way to split chores between me and my other roommates.
2) As a student, I want to be able to keep up with the common expenses.
3) As a mother, I want to be able to assign chores around the house for my family.
4) As a tenant, I want to easily keep the apartment organized and clean.
5) As a tenant, I want to waste less food.
6) As an office worker, I want to avoid conflicts over the care taking of the common space.
7) As a tenant, I want to know what I have to do without spending extra time talking with my roommate.
8) As a user, I want to be able to manage the common spaces I live in
9) As a student, I want to simultaneously keep track of my dorm room and my home’s chores
10) As a user I want to send in-app notifications to enter certain groups
11) As a user I want to be able to customize the app’s interface and my profile
12) As a user I want to find other groups I’m supposed to join by their names
  
  
## Backlog
We used Trello to keep track of our tasks: https://trello.com/b/ji4XLFmP/organize-living-space-app

![image](https://user-images.githubusercontent.com/62221313/122079659-4b193180-ce06-11eb-96be-97e237f02bfa.png)
  

## Conceptual Diagram
![Space-Organizing-Diagram (3)](https://user-images.githubusercontent.com/62221313/122174899-d4bf1280-ce8b-11eb-8551-f068d0930f9a.png)

  

## App description:
One of the biggest problems that comes with sharing a space with somebody else is its management. It can prove to be quite difficult to share the tasks around the house and keep track of them, and sometimes people even forget about them. So what can you do? I'm glad you asked. Here is our solution: a space organizing app whose purpose is to make the life of those who share a space together easier.  
  
How, you may ask? By doing the tedious part of keeping track of tasks and expenses so you don't have to.  
  
Our app is designed in such a way that it is easy to create a group for your common space and then assign tasks and add expenses for it. Just a few clicks and your roomates can know they have to do the dishes or clean the floors. And in case you've got tired of keeping track of how much money you spend and how much you have to get back from your colleagues, we've got you coverd as well. All you have to do is add the expense to your group's board and we'll do all the math for you.  
  
The platform is very intuitive and user-freindly. Easy to navigate and add or remove elements, view your own tasks and edit your group's page.  
  
No more sticky notes left all over the place. We are here to do what most people hate: organize. And we do it well.  
  
  
## Features:
   - Creating a user profile (including eiting)
   - Creating a new group for your shared space (including editing and deleting)
   - Searching a group by its name or description
   - Requesting to enter a group (needs accept from the admin of the group)
   - Sending invites to users to join a group
   - Adding a new task to the group dashboard (including editing and deleting)
   - Assigning tasks to different members of the groups
   - Viewing all the tasks grouped by priority
   - Viewing all the tasks assigned to the current user
   - Adding a new expense to the group (inlcuding editing and deleting)
   - Calculating the total expense's price and how much each member has to pay
   - Resetting the expenses for the group
   - User-friendly calendar in the groups' dashboards
  
  
## Demo
https://drive.google.com/file/d/1M5Y3rfhEyJfHV9hzLAY74iLatHpsJLlZ/view?usp=sharing
  
  
## Design Patterns
   - MVC arhitecture
  
  
## Code standards
   - Bootstrap
   - Jquery
   - Camel Case naming 
   - Controller standard naming
  
  
## Build tool
   - Visual Studio build tool (MSBuild)
  
  
## Bug reporting:
   1) Getting the index for a group
      - Error: There is already an open DataReader associated with this Command which must be closed first
      - Solution: Transforming data from DB into a list using the ToList method, thus closing the open DB connection
   2) Adding the 404 page
      - Error: Couldn't change design for 404 page
      - Solution: The error was 500
   3) Can't edit assigned user of task
      - Error: Instead of editing the existing task, another task is added to the database
      - Solution: Deleting the old task and leving only the edited one in the database
   4) Can't edit task from within group page
      - Error: Invalid model state
      - Solution: Added the missing Priority field
   5) Can't login after register
      - Error: Register successful, but user couldn't stay logged in
      - Solution: name != username
   6) Can't add new task
      - Error: Invalid model state
      - Solution: Added the missing Priority field
  

## Automation Testing:
   - Unit testing using Moq to simulate the context of assigning roles for users that are accessing the app
   - Authorization to access the app's functionalities has been tested 
   - Ran 20 successful tests

![image](https://user-images.githubusercontent.com/62206596/122086113-13ad8380-ce0c-11eb-8490-371bcc9906ba.png)
  
  
## Refactoring
Before:
![WhatsApp Image 2021-06-16 at 09 48 51](https://user-images.githubusercontent.com/62221313/122171319-3e3d2200-ce88-11eb-9b8e-ab6341010bef.jpeg)

After:
![WhatsApp Image 2021-06-16 at 09 48 51 (1)](https://user-images.githubusercontent.com/62221313/122171322-3ed5b880-ce88-11eb-92bb-63d2469f5882.jpeg)
  
  
## Our motto:
![apes](https://user-images.githubusercontent.com/62221313/122080609-1194f600-ce07-11eb-93ee-b604fd7aaa5a.jpg)
