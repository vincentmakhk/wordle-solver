// See https://aka.ms/new-console-template for more information
using System;
using System.Text.RegularExpressions;
using ConsoleApp3;

Console.WriteLine("Hello, World!");




var judge = new Judge();
var words = new Words();
//var answer = words.RandomizeAnswer();
var answer = "plaza";
var firstGuess = "crypt";

var analysis = new Analysis(judge, words);
//analysis.Play(firstGuess, answer);


// Find best first word
//analysis.FindBestFirstWord();

// Identity words cannot be first
//analysis.FindNotFirstWords();

// Max guess for a first word
var result = analysis.WorstGames("olate");

return 0;
