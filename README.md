# HackerNewsAssessment

## How to run projects

You can follow these instructions or execute them through Visual Studio or Visual Studio Code.

### Dotnet API

	1. Go to the HackerNewsAPI\HackerNewsAPI folder in a console
	2. Run 'dotnet run'

### Angular frontend

	1. Go to the HackerNewsWeb folder in a console
	2. Run 'npm install'
	3. Run 'npm start'
	4. You can go now to http://localhost:4200/ and see the running page (make sure the API is also running)
	
Note that the Angular CLI requires a minimum Node.js version of v20.19 or v22.12.

## How to run tests

You can follow these instructions or test them through Visual Studio or Visual Studio Code.

### Dotnet API

	1. Go to the HackerNewsAPI\HackerNewsAPI.Tests folder in a console
	2. Run 'dotnet test'

### Angular frontend

	1. Go to the HackerNewsWeb folder in a console
	2. Run 'npm test'

This will start the Karma test runner and execute all unit tests using Jasmine.
Note that the Angular CLI requires a minimum Node.js version of v20.19 or v22.12.

## Initial Requirements

Using the Hacker News API (https://github.com/HackerNews/API), create a solution that allows users to view the newest stories from the feed.

Your solution must be developed using an Angular front-end app that calls a C# .NET Core back-end restful API.

At a minimum, the front-end UI should consist of:

	- A list of the newest stories
	- Each list item should include the title and a link to the news article. Watch out for stories that do not have hyperlinks!
	- A search feature that allows users to find stories
	- A paging mechanism. While we love reading, 200 stories on the page is a bit much.
	- Automated tests for your code
 

At a minimum, the back-end API should consist of:

	- Usage of dependency injection, it's built-in so why not?
	- Caching of the newest stories
	- Automated tests for your code
 
## Assumptions

	- We still want to show the stories without hyperlinks.
	- We are only caching to avoid going to the API for every action (sorting, paging, searching) for getting details again, but we'll always go to get at least ids so we make sure we get the newest data.
	- We are displaying the news in a grid
	- Stories in the newstories endpoint are already sorted, newest first.
	- Stories without url and without title won't be shown.
	- Pagination, sorting and search happen in the backend.
	- There's only one search input and that searchs for the fields Title and URL. 
	- Pagination controls in the UI display a list of possible values (10, 20, 50) and the ability to move between pages.
	- If after searching there's no data that matches the criteria the grid will be hidden and a message will be displayed instead.
	- The user can sort by Title or URL.

## Hours logged

6hs- 7/1
6hs- 7/2
6hs- 7/3
6hs- 7/4
6hs- 7/7
1hs- 7/8
1hs- 7/9