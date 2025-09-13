# wordle-solver

NYT Wordle is a mastermind (an old game) like game to guess a 5-letter word in 6 guesses.

And there are two modes, easy and hard mode. In hard mode, your next guess has to follow the previous results.

Given the assumption that there are only around 2300 answers, and around 14000 legitimate words

On top of implementing an algorithm to attempt solving the puzzle, would like to have an analysis on the words

## Built With

C#

## Problem Statement

Define f(x), where x is a word, as whether using x as a first word can guarantee solving in 6 guesses.

It would be interesting to know:
1. If x exists, i.e. there exists a strategy that guarantees arriving at the answer in 6 guesses
2. If x is unable to know, what are the best words for first guess?
3. An analysis on the words, what words should be avoided as a first guess

## Results

There are patterns like _OVER, _IGHT have 7 or more answers.
Patterns like STA__, _A_RE also have many solution.
x was thought to be unlikely to exist.

However, after a few iterations, found the solution of x, which solves (1)

For (2), algorithm is to minimize the number of words, the best word for first guess gives a single word that stands out, and it is different from x

For (3), given the patterns, there are around 6500 words that cannot be the first word.

## Authors

Vincent Mak

## License

MIT License

## Acknowledgements

Thanks to the game inventor Josh Wardle, NYT who acquired it and the one who has introduced me to the game.
