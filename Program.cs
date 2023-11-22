//Author: Alex Farhood
//File Name: Program.cs
//Project Name: PASS1
//Creation Date: February 9, 2023
//Modified Date: February 26, 2023
//Description: Wordle immitation game, with a working stats file

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PASS1
{
    class MainClass
    {
        //Read Wordle Dictionary and Write userstats
        static StreamReader DictionaryAnswers;
        static StreamReader DictionaryExtras;
        static StreamReader ReadStatsFile;
        static StreamWriter WriteStatsFile;

        //Store word letter and guess limits
        const int WORD_LIMIT = 5;
        const int GUESS_LIMIT = 6;

        //Store user stats array
        static float[] stats = new float[10];

        //Store user guess counter
        static int guessCount = -1;

        //Store wordle dictionaries
        static List<string> wordleAnswersList = new List<string>();
        static List<string> wordleExtrasList = new List<string>();

        //Store the letter colour for each guess
        static string[,] letterColour = new string[GUESS_LIMIT, WORD_LIMIT];

        //Store randomized word
        static string randomWord = "";

        //Store flags - if the guess is correct - if a game has been played
        static bool isGuessCorrect;
        static bool hasGameBeenPlayed = false;

        public static void Main(string[] args)
        {

            //Call the subprogram that reads both wordle dictionaries and call the menu
            ReadWordleAnswers("WordleAnswers.txt");
            ReadWordleExtras("WordleExtras.txt");
            Menu();
        }

        //Pre:
        //Post:
        //Desc: User Interface Menu
        private static void Menu()
        {
            //Store menu message and the users choice
            string menuMsg = "Welcome to Wordle!\n\nChoose One of the Following Options\n1. Rules\n2. Play\n3. Exit";
            string menuChoice;

            //Display menu Message and read answer
            Console.WriteLine(menuMsg);
            menuChoice = Console.ReadLine();
            Console.Clear();

            //Choose appropriate subprogram based on the answer
            switch (menuChoice)
            {
                case "1":
                    DisplayRules();
                    break;
                case "2":
                    WordleLogic();
                    break;
                case "3":
                    break;
            }
        }

        //Pre:
        //Post:
        //Desc: Logic behing the wordle game
        private static void WordleLogic()
        {
            //Store and generate a random number
            Random rng = new Random();

            //Store bounds of list
            int answersListMax = 2315;
            int answersListMin = 0;

            //Store the random correct word by finding the corresponding value in the list at the corresponding randomly generated element
            randomWord = wordleAnswersList.ElementAtOrDefault(rng.Next(answersListMin, answersListMax - 1));

            //Store 2D Array for the worlde grid
            char[,] wordleGrid = new char[GUESS_LIMIT, WORD_LIMIT];

            //Store the users guess and texts instructing the user to enter a word
            string userGuess = "";
            string enterWordMsg = "Enter a 5 letter word: ";
            string notWordMsg = "You have not entered a valid 5 letter word, please try again:";

            //Store flags checking if the guess is a valid word and if it is the users first guess
            bool isValidWordEntered = true;
            bool isItFirstGuess = true;

            //Determine if the game has begun
            if (hasGameBeenPlayed)
            {
                //Call subprogram to reset values, store that the game has is being played
                resetValues(letterColour);
                hasGameBeenPlayed = false;
            }

            //Stay in loop as long as the user has not reached the guess limit or guessed the correct word
            while (guessCount < GUESS_LIMIT && !isGuessCorrect)
            {
                //Determine if it is the first guess
                if (isItFirstGuess)
                {
                    //Call subprogram that displays the wordle grid
                    DrawWordleDisplay(wordleGrid);

                    //Store guess count as 0 and first guess to false
                    guessCount = 0;
                    isItFirstGuess = false;
                }

                //Check if a valid word has been entered
                if (isValidWordEntered)
                {
                    //Display message telling user to enter a word
                    Console.WriteLine(enterWordMsg);
                }
                else
                {
                    //Display message telling user they have not entered a valid word, reset flag checking for a valid word
                    Console.WriteLine(notWordMsg);
                    isValidWordEntered = true;
                }

                //Read user input and store it as the user's guess
                userGuess = Console.ReadLine().ToLower();

                //Determine whether the user's guess is contained within either of the dictionaries
                if (wordleAnswersList.Contains(userGuess) || wordleExtrasList.Contains(userGuess))
                {
                    //Loop through all instances of the index
                    for (int j = 0; j < WORD_LIMIT; j++)
                    {
                        //Store each letter of the users guess within the wordle grid array
                        wordleGrid[guessCount, j] = userGuess[j];
                    }

                    //Call subprograms determining the colours of the letters and if the guess it correct
                    DetermineLetterColour(wordleGrid);
                    CorrectGuessCheck(userGuess);
                }
                else
                {
                    //Store that the entered word is invalid, dont count invalid word as a guess
                    isValidWordEntered = false;
                    guessCount--;
                }

                //Call subprogram displaying the updated wordle grid with the most recent word
                DrawWordleDisplay(wordleGrid);

                //Determine if the user has guessed the correct word
                if (!isGuessCorrect)
                {
                    //Calculate guess counter one higher each round
                    guessCount++;
                }
            }

            //Call subprogram that displays the end of game screen
            DisplayEndOfGameScreen();
        }

        //Pre: none
        //Post: none
        //Desc: Checks every instance of the wordle grid array for each guess and determines the colour of which the letter will be displayed
        public static void DetermineLetterColour(char[,] wordleGrid)
        {
            //Store each letter in random word in array
            char[] letterAppearMoreThanOnce = randomWord.ToCharArray();

            //As long as the value of j is less than the amount of letters in a 5 letter word stay in the loop. j represents columns of the correct word
            for (int j = 0; j < WORD_LIMIT; j++)
            {
                //Store the default of each letter colour to gray
                letterColour[guessCount, j] = "gray";

                //As long as the value of k is within the amount of letters in a 5 letter word stay in loop. k represents the columns of the inputted guess. i.e. guess "ABCDE", if k = 2 C is returned
                for (int k = 0; k < WORD_LIMIT; k++)
                {
                    //Check if the letter of guessed word at column j is equal to the correct letter of column k
                    if (wordleGrid[guessCount, j].Equals(letterAppearMoreThanOnce[k]))
                    {
                        //Store it as ' ' so that the letter won't be used again and will not return yellow if called twice i.e. correct word: magic, Guess: aging, the first g should be yellow, second g is gray
                        letterAppearMoreThanOnce[k]= ' ';

                        //Check if the column of the guess is not equal to the column of the answer
                        if (j != k)
                        {
                            //Store the colour of the guessed letter at instance j to yellow
                            letterColour[guessCount, j] = "yellow";
                        }

                        //Check if the column of the guess is equal to the column of the answer
                        if (j == k)
                        {
                            //Store the colour of the guessed letter at instance j to green, then break the loop, moving on to the next letter of the correct word, therefore green cant be overwritten.
                            letterColour[guessCount, j] = "green";
                            break;
                        }
                    }
                }
            }
        }

        //Pre:
        //Post:
        //Desc: Determines whether the guessed word is correct
        public static void CorrectGuessCheck(string userGuess)
        {
            //Checking if the user guesses the correct word
            if (userGuess == randomWord)
            {
                //Store that the user guessed the right word
                isGuessCorrect = true;
            }
        }

        //Pre:
        //Post:
        //Desc: Read the file containing the wordle answers dictionary and store it in a list
        private static void ReadWordleAnswers(string dictionary)
        {
            //Try to read the dictionary and store it in a list, catch any exceptions
            try
            {
                //Read the answers file
                DictionaryAnswers = File.OpenText(dictionary);

                //As long as the file has not reach the end stay in loop
                while (!DictionaryAnswers.EndOfStream)
                {
                    //Store each line from the file and add it to the answers list
                    wordleAnswersList.Add(DictionaryAnswers.ReadLine());
                }

                //Close answers file
                DictionaryAnswers.Close();
            }
            catch (FormatException fe)
            {
                //Display format exception message
                Console.WriteLine(fe.Message);
            }
            catch (FileNotFoundException fnf)
            {
                //Display file not found exception message
                Console.WriteLine(fnf.Message);
            }
            catch (Exception e)
            {
                //Display general exception message
                Console.WriteLine(e.Message);
            }
        }

        //Pre:
        //Post:
        //Desc: Read the file containing the wordle extras dictionary and store it in a list
        private static void ReadWordleExtras(string dictionary)
        {
            //Try to read the dictionary and store it in a list, catch any exceptions
            try
            {
                //Read the extras file
                DictionaryExtras = File.OpenText(dictionary);

                //As long as the file has not reach the end stay in loop
                while (!DictionaryExtras.EndOfStream)
                {
                    //Store each line from the file and add it to the extras list
                    wordleExtrasList.Add(DictionaryExtras.ReadLine());
                }

                //Close extras file
                DictionaryExtras.Close();
            }
            catch (FormatException fe)
            {
                //Display format exception message
                Console.WriteLine(fe.Message);
            }
            catch (FileNotFoundException fnf)
            {
                //Display file not found exception message
                Console.WriteLine(fnf.Message);
            }
            catch (Exception e)
            {
                //Display general exception message
                Console.WriteLine(e.Message);
            }
        }

        //Pre:
        //Post:
        //Desc:
        private static void WriteUserStats(bool isResetStats)
        {
            //Try to read the stats file and store the updated stats in the same rewritten file, catch any exceptions
            try
            {
                //Reading the stats file to have the most recent and accurate stats
                ReadUserStats("WordleUserStats.txt");

                //Store the stats to their respective value in the stats array
                float gamesPlayed = stats[0];
                float gamesWon = stats[1];
                float currentWinStreak = stats[2];
                float maxWinStreak = stats[3];
                float winsOnGuess1 = stats[4];
                float winsOnGuess2 = stats[5];
                float winsOnGuess3 = stats[6];
                float winsOnGuess4 = stats[7];
                float winsOnGuess5 = stats[8];
                float winsOnGuess6 = stats[9];

                //Check if the user guessed the correct word
                if (isGuessCorrect)
                {
                    //Calculate the updated winstreak, games played and games won
                    currentWinStreak++;
                    gamesPlayed++;
                    gamesWon++;

                    //Update appropriate win count for each distributed guess depending on when the appropriate guess count
                    switch (guessCount)
                    {
                        case 0:
                            //Calculate update win count for guess 1
                            winsOnGuess1++;
                            break;
                        case 1:
                            //Calculate update win count for guess 2
                            winsOnGuess2++;
                            break;
                        case 2:
                            //Calculate update win count for guess 3
                            winsOnGuess3++;
                            break;
                        case 3:
                            //Calculate update win count for guess 4
                            winsOnGuess4++;
                            break;
                        case 4:
                            //Calculate update win count for guess 5
                            winsOnGuess5++;
                            break;
                        case 5:
                            //Calculate update win count for guess 6
                            winsOnGuess6++;
                            break;
                    }

                    //Check if the max win streak is less than the current win streak
                    if (maxWinStreak < currentWinStreak)
                    {
                        //Calculate updated max winstreak
                        maxWinStreak++;
                    }
                }
                else
                {
                    //Calculate updated games played and current winstreak
                    gamesPlayed++;
                    currentWinStreak = 0;
                }

                //Check if the reset stats flag is true
                if (isResetStats)
                {
                    //Calculate the reset user stats back to their original values of 0
                    gamesPlayed = 0;
                    gamesWon = 0;
                    currentWinStreak = 0;
                    maxWinStreak = 0;
                    winsOnGuess1 = 0;
                    winsOnGuess2 = 0;
                    winsOnGuess3 = 0;
                    winsOnGuess4 = 0;
                    winsOnGuess5 = 0;
                    winsOnGuess6 = 0;
                }

                //Store user stats in the stats array
                stats = new float[]
                { gamesPlayed, gamesWon, currentWinStreak, maxWinStreak, winsOnGuess1, winsOnGuess2, winsOnGuess3, winsOnGuess4, winsOnGuess5, winsOnGuess6};

                //Overwrite the stats file
                WriteStatsFile = File.CreateText("WordleUserStats.txt");

                //As long as i is within the bounds of the stats array stay in loop
                for (int i = 0; i < stats.Length; i++)
                {
                    //Write the stats into the file
                    WriteStatsFile.WriteLine(stats[i]);
                }
            }
            catch (IndexOutOfRangeException ore)
            {
                //Display index out of range exception message
                Console.WriteLine("Error: you wrote past array " + ore.Message);
            }
            catch (Exception e)
            {
                //Display general exception message
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                //Check if the file is not null
                if (WriteStatsFile != null)
                {
                    //Close the stats file
                    WriteStatsFile.Close();
                }
            }
        }

        //Pre:
        //Post:
        //Desc: Read the user stats file
        private static void ReadUserStats(string readStats)
        {
            //Try to read the stats file catch any exceptions
            try
            {
                //Store counter
                int count = 0;

                //Open the stats file
                ReadStatsFile = File.OpenText(readStats);

                //As long as the file has not reached the end stay in loop
                while (!ReadStatsFile.EndOfStream)
                {
                    //Store each line of the file in the stats array
                    stats[count] = Convert.ToInt32(ReadStatsFile.ReadLine());
                    count++;
                }

                //Close stats file
                ReadStatsFile.Close();
            }
            catch (FormatException fe)
            {
                //Display format exception message
                Console.WriteLine(fe.Message);
            }
            catch (FileNotFoundException fnf)
            {
                //Display file not found exception message
                Console.WriteLine(fnf.Message);
            }
            catch (Exception e)
            {
                //Display general exception message
                Console.WriteLine(e.Message);
            }
        }

        //Pre:
        //Post:
        //Desc: Display the users stats and potentially reset stats
        private static void DisplayUserStats()
        {
            //Store play again message and answer, wrong answer message, and stat names array
            string playAgainMsg = "\n\nDo you want to play again? (yes/ no)";
            string playAgainAnswer = "";
            string wrongAnswerMsg = "Invalid Answer...";
            string[] names = { "Games Played", "Games Won", "Current Win Streak", "Max Win Streak", "Winning Guess Distribution: ", "1. ", "2. ", "3. ", "4. ", "5. ", "6. " };

            //Store flag are the stats reset, reset stats message and answer
            bool isResetStats = false;
            string resetStatsMsg = "If you want to reset your stats, Enter \"yes\": ";
            string resetStatsAnswer = "";

        //Jump to this line to rewrite stats
        RewriteStats:

            //Call subprogram to rewrite user stats and clear the display
            WriteUserStats(isResetStats);
            Console.Clear();

            //Display stats title
            Console.WriteLine("Here are your stats:\n");

            //As long as k does not exceed the stats array at value 3 which represents max win streak
            for (int k = 0; k <= 3; k++)
            {
                //Display names and stats up until before the win percentage stat
                Console.WriteLine(names[k] + ": " + stats[k]);
            }

            //Check if games played does not equal 0 because when calculating win% cant devided by 0
            if (stats[0] != 0)
            {
                //Display win%, calculate games won devided by games played to get the result of win%
                Console.WriteLine("Win Percentage: " + Math.Round(stats[1] / stats[0], 2) * 100 + "%\n");
            }
            else
            {
                //Display win% as 0%
                Console.WriteLine("Win Percentage: 0%\n");
            }

            //Display Headline for winning guess distribution
            Console.WriteLine(names[4]);

            //As long as i is within the length of the array stay in loop
            for (int i = 5; i < names.Length; i++)
            {
                //Check if i is equal to guess count and the stats are not getting reset
                if (i - 5 == guessCount && !isResetStats)
                {
                    //Change foreground colour to darkgreen
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                }

                //Display stats names
                Console.Write(names[i]);

                //As long as a is less than the value of stats at i - 1 stay in array. Subtract 1 from i because names array is one value longer than stats
                for (int a = 0; a < stats[i - 1]; a++)
                {
                    //Display bar
                    Console.Write("█");
                }

                //Display stats and reset colour
                Console.WriteLine(stats[i - 1]);
                Console.ResetColor();
            }

            //Check if reset stats flag is false
            if (!isResetStats)
            {
                //Display askig user reset stats message, and read reset stats answer
                Console.Write(resetStatsMsg);
                resetStatsAnswer = Console.ReadLine();
            }

            //Store reset stats flag to false
            isResetStats = false;

            //Check if user inputted reset stats answer is yes
            if (resetStatsAnswer == "yes")
            {
                //Store reset stats answer, reset stats flag set to true.
                resetStatsAnswer = "";
                isResetStats = true;

                //Jump to line rewrite stats
                goto RewriteStats;
            }

            //While the play again answer is neither yes nor no keep asking them to if they want to play again
            while (playAgainAnswer != "yes" && playAgainAnswer != "no")
            {
                //Display play again message and read play again answer
                Console.WriteLine(playAgainMsg);
                playAgainAnswer = Console.ReadLine().ToLower();

                //Choose appropriate case depending on play again answer
                switch (playAgainAnswer)
                {
                    case "yes":
                        //Clear console and start a new game
                        Console.Clear();
                        WordleLogic();
                        break;
                    case "no":
                        //Clear console and go back to menu
                        Console.Clear();
                        Menu();
                        break;
                    default:
                        //Clear console, Display wrong answer message, stay in loop
                        Console.Clear();
                        Console.WriteLine(wrongAnswerMsg);
                        break;
                }
            }
        }

        //Pre:
        //Post:
        //Desc: Screen displaying the rules
        private static void DisplayRules()
        {
            //Store continue message and rules message
            string continueMsg = "Click Enter to Proceed.";
            string rulesMsg = "How to play Wordle:" +
                "\n\n1. To win you must guess a random 5 letter word in 6 or less guesses." +
                "\n\n2. A yellow letter means the letter in your guessed word contains a letter in the correct word but it is not in the right spot." +
                "\n\n3. A green letter means the letter in your guessed word is the right letter in the right spot." +
                "\n\n4. A red letter means the letter in your guessed word is not a letter in the correct word.";

            //Display the rules and promt the user to proceed, wait for input then clear console and go back to menu
            Console.WriteLine(rulesMsg + "\n\n" + continueMsg);
            Console.ReadLine();
            Console.Clear();
            Menu();
        }

        //Pre:
        //Post:
        //Desc: Draw the wordle grid display and set the colours for each letter
        public static void DrawWordleDisplay(char[,] wordleGrid)
        {
            //Clear console and display top of grid
            Console.Clear();
            Console.WriteLine("╔═══╦═══╦═══╦═══╦═══╗");

            //For each instance of i that is less than the guess count stay in loop
            for (int i = 0; i <= guessCount; i++)
            {
                //For each instance of j that is less than the 5 letter word limit stay in loop
                for (int j = 0; j < WORD_LIMIT; j++)
                {
                    //Check if letter colour at row i, column j is gray, green or yellow
                    if (letterColour[i, j] == "gray")
                    {
                        //Display the outline of the grid with the respective letter as grey
                        Console.Write("║ ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(wordleGrid[i, j] + " ");
                    }
                    else if (letterColour[i, j] == "green")
                    {
                        //Display the outline of the grid with the respective letter as green
                        Console.Write("║ ");
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write(wordleGrid[i, j] + " ");
                    }
                    else if (letterColour[i, j] == "yellow")
                    {
                        //Display the outline of the grid with the respective letter as yellow
                        Console.Write("║ ");
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write(wordleGrid[i, j] + " ");
                    }

                    //Reset colour after each letter
                    Console.ResetColor();
                }

                //Display the final outline of the grid for each row
                Console.WriteLine("║");

                //Check if the index is less than the last letter of the word
                if (i < GUESS_LIMIT - 1)
                {
                    //Display outline of grid inbetween rows
                    Console.WriteLine("╠═══╬═══╬═══╬═══╬═══╣");
                }
            }

            //As long as guess count is less than the guess limit stay in loop
            for (int k = guessCount; k < GUESS_LIMIT; k++)
            {
                //Check guess count's value relative to the guess limit
                if (k == (GUESS_LIMIT - 2))
                {
                    //Display the final row of the grid, break loop
                    Console.WriteLine("║ ░ ║ ░ ║ ░ ║ ░ ║ ░ ║");
                    Console.WriteLine("╚═══╩═══╩═══╩═══╩═══╝");
                    break;
                }
                else if (k == (GUESS_LIMIT - 1))
                {
                    //Display the bottom outline of the grid when the last guess has happened
                    Console.WriteLine("╚═══╩═══╩═══╩═══╩═══╝");
                }
                else
                {
                    //Display the empty grid
                    Console.WriteLine("║ ░ ║ ░ ║ ░ ║ ░ ║ ░ ║");
                    Console.WriteLine("╠═══╬═══╬═══╬═══╬═══╣");
                }
            }
        }

        //Pre:
        //Post:
        //Desc: Screen displayed at the end of each game
        private static void DisplayEndOfGameScreen()
        {
            //Store the messages for the end screen, if the user wins, if the user loses and to proceed
            string endScreenMsg = "\nThank you for playing Wordle.";
            string winMsg = "\n\nCongratulations you won in " + (guessCount + 1) + " guesses!";
            string loseMsg = "\n\nOh no... Better luck next time.\nThe correct word was: " + randomWord;
            string continueMsg = "Click Enter to Proceed.";

            //Check if the user guessed the correct word
            if (isGuessCorrect)
            {
                //Display win message
                Console.WriteLine(winMsg);
            }
            else
            {
                //Display lose message
                Console.WriteLine(loseMsg);
            }

            //Display the end screen message with a prompt to continue, wait for input
            Console.WriteLine(endScreenMsg + "\n" + continueMsg);
            Console.ReadLine();

            //Store has a game been played flag as true
            hasGameBeenPlayed = true;

            //Call subprogram display user stats.
            DisplayUserStats();
        }

        //Pre:
        //Post:
        //Desc: Reset values that may carry over from game to game after each game is played
        private static void resetValues(string[,] letterColour)
        {
            //Clearing letter colour array
            Array.Clear(letterColour, 0, letterColour.Length);

            //Store guess count and is guess correct flag
            guessCount = -1;
            isGuessCorrect = false;
        }
    }
}