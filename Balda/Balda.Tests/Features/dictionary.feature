Feature: Word dictionary
  In order to play the game
  As a player
  I should be able

  Scenario: Check if the dictionary contains a word
    Given the dictionary has the following words
      | word    |
      | белка   |
      | стрелка |
      | собака  |
    When I check the word "белка"
    Then the dictionary should contain it
    When I check the word "друг"
    Then the dictionary should not contain it

  Scenario Outline: Pick a random word from the dictionary
    Given the dictionary has the following words
      | word     |
      | волк     |
      | тело     |
      | дело     |
      | собака   |
      | работа   |
      | морока   |
    When I pick a <length> characters long random word
    Then it should be one of the words in the list "<words>"
    Examples:
      | length | words                  |
      | 4      | волк, тело, дело       |
      | 6      | собака, работа, морока |
      | 8      |                        |