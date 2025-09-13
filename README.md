# wordle-solver

NYT Wordle is a mastermind (does anyone still knows?) like game to guess a 5-letter word in 6 guesses.

And there are two modes, easy and hard mode. In hard mode, your next guess has to follow the previous results.

Given the assumption that there are only around 2000 answers, and around 14000 legitimate words

On top of implementing an algorithm to attempt solving the puzzle, would like to have an analysis

## Built With

C#

## Problem Statement

Define f(x), where x is a word, as whether using x as a first word can guarantee solving in 6 guesses.

It would be interesting to know:
1. If x exists, i.e. there exists a strategy that guarantees arriving at the answer in 6 guesses
2. If x is unable to know, what are the best words for first guess?
3. An analysis on the words, what words should be avoided, i.e. words that cannot be x

## Results

For 1, I haven't an answer
For 2, according to algorithm I've written, there is one single word that stands out
For 3, there are at least 6500 words that cannot be x. Based on patterns like _OVER, _IGHT which has 7 or more answers

## Authors

Vincent Mak

## License

This project is licensed under the GNU General Public License v3.0 License - see the LICENSE.txt file for details

## Acknowledgments

Thanks to the game inventor.
Thanks to the one who has introduced me to the game.
