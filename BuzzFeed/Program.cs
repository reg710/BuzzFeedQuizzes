using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace buzzfeed02
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            //Creates the connection to shared database
            SqlConnection myConnection = new SqlConnection(@"");
            myConnection.Open();

            //Allow user to select creating/taking a quiz
            string choice = "";
            while (choice != "exit")
            {
                Console.WriteLine("Would you like to CREATE a quiz or TAKE a quiz?");
                choice = Console.ReadLine().ToLower();

                if (choice == "create")
                {
                    Console.WriteLine("What will you name your quiz?");
                    string quizName = Console.ReadLine();

                    //Inserts what the user input as a name into the Quizzes table into Name column
                    SqlCommand command = new SqlCommand($@"
                    INSERT INTO Quizzes (Name)
                    VALUES ('{quizName}')"
                    , myConnection);

                    //Scott's recommendation here was to set variable which you could use to check how many rows were added (I believe?)
                    int a = command.ExecuteNonQuery();

                    //This command selects the row in the Quizzes table with the name equal to what the user just entered as a quiz name
                    SqlCommand mycommand = new SqlCommand($@"
                    SELECT *
                    FROM Quizzes
                    WHERE Name = '{quizName}'"
                    , myConnection);
                    SqlDataReader reader = mycommand.ExecuteReader();

                    //Creating a variable to store the Quiz ID from the new quiz name
                    int currentQuiz = 0;

                    //Looping through the reader, and converting output to an int and saving in variable
                    //Scott suggested a loop here in case there were multiple quizzes with identical names. If that happened, only the last one in the loop would be saved to the variable. This could possible cause problems, to be worried about/fixed later
                    while (reader.Read())
                    {
                        currentQuiz = Convert.ToInt32(reader["Id"]);
                    }
                    reader.Close();

                    //Create number variable to display question number being entered
                    int questionNumber = 1;

                    // Asking for a Question
                    bool looping = true;
                    while (looping)
                    {
                        Console.WriteLine($"Enter Question Number {questionNumber}:");
                        string quizQuestion = Console.ReadLine();

                        //Inserts user questions into the Questions Table
                        //We are able to link these questions with the correct Quiz ID by using the currentQuiz variable
                        SqlCommand questioncommand = new SqlCommand($@"
                        INSERT INTO Questions (Text, QuizId)
                        VALUES ('{quizQuestion}', '{currentQuiz}')"
                        , myConnection);
                        int b = questioncommand.ExecuteNonQuery();

                        SqlCommand newcommand = new SqlCommand($@"
                        SELECT * 
                        FROM Questions
                        WHERE Text = '{quizQuestion}'"
                        , myConnection);
                        SqlDataReader newreader = newcommand.ExecuteReader();

                        //Creating a variable to store the Question ID from the newly created question
                        int currentQuestion = 0;

                        while (newreader.Read())
                        {
                            currentQuestion = Convert.ToInt32(newreader["Id"]);
                        }
                        newreader.Close();

                        // Asking for answers and values
                        // Currently set to only accept two answer options per question
                        // This matches the take a quiz loop so if updating, update both
                        // One way to add answer selections would be to insert variable i into a new column in the answers Table
                        for (int i = 1; i < 3; i++)
                        {
                            Console.WriteLine($"Enter Answer Number {i}:");
                            string userAnswer = Console.ReadLine();
                            Console.WriteLine($"Enter Answer Value:");
                            string userAnswerValue = Console.ReadLine();

                            SqlCommand answercommand = new SqlCommand($@"
                            INSERT INTO Answers (Text, Value, QuestionId)
                            VALUES ('{userAnswer}', '{userAnswerValue}', '{currentQuestion}')"
                            , myConnection);
                            int c = answercommand.ExecuteNonQuery();
                        }
                        //increases question count
                        questionNumber++;

                        //Allow user to continue adding questions or break loop
                        Console.WriteLine("Do you want to add another question? (yes/no)");
                        string answer = Console.ReadLine().ToLower();
                        if (answer == "no")
                        {
                            looping = false;
                        }
                    }

                    // Adding results
                    // May want to add explainer for how results math works for users
                    bool resultLoop = true;

                    while (resultLoop)
                    {
                        Console.WriteLine("Name the Title of your outcome:");
                        string outcome = Console.ReadLine();

                        Console.WriteLine("Describe your outcome:");
                        string userDescription = Console.ReadLine();

                        Console.WriteLine("At what value does this outcome apply?");
                        int userStartAt = Convert.ToInt32(Console.ReadLine());


                        SqlCommand resultcommand = new SqlCommand($@"
                        INSERT INTO Results (Title, Description, QuizId, StartAt)
                        VALUES ('{outcome}', '{userDescription}', '{currentQuiz}', '{userStartAt}')"
                             , myConnection);
                        int d = resultcommand.ExecuteNonQuery();

                        Console.WriteLine("Do you want to add another outcome? (yes/no)");
                        string choice2 = Console.ReadLine();
                        if (choice2 == "no")
                        {
                            resultLoop = false;
                        }
                    }
                }

                // Taking a quiz
                else if (choice == "take")
                {
                    // creating lists variable for storing user answers and values
                    var answerList = new List<int>();
                    var answerValues = new List<int>();

                    //Create user
                    //Currently always making a new user, add something to allow same user to take more than one quiz
                    Console.WriteLine("Please enter a username:");
                    string userName = Console.ReadLine();

                    SqlCommand userCommand = new SqlCommand($@"
                    INSERT INTO Users (Name)
                    VALUES ('{userName}')"
                    , myConnection);
                    int e = userCommand.ExecuteNonQuery();

                    //This command selects the row in the Users table with the name equal to what the user just entered as a name
                    SqlCommand userIdCommand = new SqlCommand($@"
                    SELECT *
                    FROM Users
                    WHERE Name = '{userName}'"
                    , myConnection);
                    SqlDataReader userIdReader = userIdCommand.ExecuteReader();

                    //Creating a variable to store the UserID from the new quiz name
                    int currentUser = 0;

                    //Looping through the reader, and converting output to an int and saving in variable
                    // Still need to work out what to do when users have the same name (do we prevent duplicate names or require them to enter own unique user ID)
                    while (userIdReader.Read())
                    {
                        currentUser = Convert.ToInt32(userIdReader["Id"]);
                    }
                    userIdReader.Close();
                    Console.WriteLine();


                    //Quiz Menu
                    Console.WriteLine("Please select a quiz!");
                    SqlCommand menuCommand = new SqlCommand($@"
                    SELECT *
                    FROM Quizzes
                    ", myConnection);

                    SqlDataReader menuReader = menuCommand.ExecuteReader();

                    while (menuReader.Read())
                    {
                        Console.WriteLine($"{menuReader["Id"]}: {menuReader["Name"]}");
                    }
                    menuReader.Close();

                    int userQuizChoice = Convert.ToInt32(Console.ReadLine());

                    SqlCommand questionsCommand = new SqlCommand($@"
                    SELECT Questions.QuizId AS QuizId, Questions.Text AS QText, Questions.ID AS QuestionsID, Answers.Id AS AnswersID, Answers.Text AS AText, Answers.QuestionId AS JoiningID
                    FROM Questions
                    JOIN Answers ON Questions.Id = Answers.QuestionId
					WHERE QuizId = {userQuizChoice}"
                    , myConnection);

                    SqlDataReader questionReader = questionsCommand.ExecuteReader();

                    string previousQuestion = "";
                    //int answerNumber = 1;
                    int answerCounter = 0;

                    while (questionReader.Read())
                    {
                        // if this is the first line for a quesion in the selected table
                        // Print out the question, with answer on it's own line
                        if (questionReader["QText"].ToString() != previousQuestion)
                        {
                            // When question is different than question is when you want to print it
                            Console.WriteLine($"{questionReader["QText"]}");
                            previousQuestion = questionReader["QText"].ToString();
                            //answerNumber = 1;
                        }
                        // If not, printing out the answer options
                        // Currently, user will have to pick the answerID in order for us to insert it into UserAnswer table
                        // Will update later so they could choose (something like abcd) instead

                        Console.WriteLine($"   {questionReader["AnswersID"]}: {questionReader["AText"]}");

                        //answerNumber++;
                        answerCounter++;

                        // Taking user answer.
                        // Currently have to have a set number of answer options that is consistent across all questions in the quiz. Could update to a variable but for now is hard coded to 2 to match the make a quiz setting
                        if (answerCounter == 2)
                        {
                            Console.WriteLine("Select your answer number:");

                            // Can't save user response as a variable because it will overwrite for each loop it goes through. Use a list instead!                        
                            answerList.Add(Convert.ToInt32(Console.ReadLine()));
                            answerCounter = 0;                                  
                        }
                    }
                    questionReader.Close();

                    // Going through each answer in the list and saving it to the UserAnswers table
                    // along with current userID
                    foreach (var answer in answerList)
                    {
                        SqlCommand userAnswerCommand = new SqlCommand($@"
                                INSERT INTO UserAnswers (AnswerId, UserId)
                                VALUES ('{answer}', '{currentUser}')"
                                , myConnection);
                        int f = userAnswerCommand.ExecuteNonQuery();
                    }

                    // Going through each answer in the list,
                    // Finding the associated value and saving that to a value list
                    foreach (var answer in answerList)
                    {
                        SqlCommand valueCommand = new SqlCommand($@"
                        SELECT *
                        FROM Answers
                        WHERE Id = '{answer}'", myConnection);

                        SqlDataReader valueReader = valueCommand.ExecuteReader();
                        while (valueReader.Read())
                        {
                            answerValues.Add(Convert.ToInt32(valueReader["Value"]));
                        }
                        valueReader.Close();
                    }

                    int sumValues = answerValues.Sum();

                    //Display results
                    SqlCommand command = new SqlCommand($@"
                    SELECT *
                    FROM Results
                    WHERE StartAt <= {sumValues} AND QuizId = {userQuizChoice}
                    ORDER BY StartAt DESC", myConnection);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Read();
                    Console.WriteLine();
                    Console.WriteLine("Your Results: ");
                    Console.WriteLine(reader["Title"]);
                    Console.WriteLine(reader["Description"]);
                    reader.Close();
                }

                else
                {
                    Console.WriteLine("Buh");
                }
            }
            Console.WriteLine();
            myConnection.Close();
        }
    }
}
