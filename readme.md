# Buzzfeed Quizzes -

This is an Academy PGH project when we were learning SQL. My group initially wrote the code to be able to create a quiz and save it to a SQL database. After the project ended, I worked with a teammate to expand our code to also allow a user to take quiz from a list of options.

## NOTES
### MAKING QUIZ
    [x] Create menu
    [x] Create quiz name
    [x] Add one question at a time linked to specific quiz
    [x] Add answers to questions, ideally after you enter each question
    [x] Have user provide values associated with each answer

### TAKING QUIZ
    [x] Get user name, save into User Table
    [x] Display menu of quizzes
    [x] Display one question at a time, with all possible answers
    [x] Save user answer choice to UserAnswers
    [x] Figure out results

### UPDATES TO WORK ON
    [] Rewrite with classes to make it more readable
    [] Polish up display formatting and spacing between sections
    [] What to do if two quizzes have the same name? How can you get the right ID?
    [] Create a variable or column in quiz table to set how many answer options per question
        - this would remove how it is currently hardcoded to 2 answers per question
    [] Do we need to store number of questions created per quiz? Currently can make as many as user wants
    [] Add explanation text for user making a quiz outlining how results math works
    [] Add way for previous user to re-login with their ID or enter new username
    [] Add section for user to view all past quiz results
    [] Can the value list be created at the same time as the answer list to reduce number of steps?
    [] Create new row in answer table that provides user choice (like 1, 2, 3, 4 or a,b,c,d) instead of how currently the user has to enter the unique answer ID.
    [] Add in something to keep user inputs from causing errors (injection issues?)
    [] Update/delete functionality somewhere - delete user or edit quizzes?
