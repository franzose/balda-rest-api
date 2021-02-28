Feature: Sign in
  In order to play the game
  As a registered user
  I should be able to sign in

  Scenario: Successful sign in
    Given there are some registered users
      | username | password             |
      | user1    | kRk9mopFE@pzR]4WtujY |
      | user2    | W2W3FBnfXkw*&wjwqbaw |
    When I send POST request to "/signin" with
    """
    {"username": "user1", "password": "kRk9mopFE@pzR]4WtujY"}
    """
    Then I should see 200 status code
    And I should be authenticated

  Scenario: Invalid credentials
    Given there are some registered users
      | username | password             |
      | user1    | kRk9mopFE@pzR]4WtujY |
      | user2    | W2W3FBnfXkw*&wjwqbaw |
    When I send POST request to "/signin" with
    """
    {"username": "non_existent_user", "password": "EQ#DuCUvg2XE7oMzc^AU"}
    """
    Then I should see 401 status code
    And I should not be authenticated
    And I should see the "No user was found for the given credentials" message

  Scenario Outline: Missing credential
    Given there are some registered users
      | username | password             |
      | user1    | kRk9mopFE@pzR]4WtujY |
      | user2    | W2W3FBnfXkw*&wjwqbaw |
    When I send POST request to "/signin" with
      """
      {"username": "<username>", "password": "<password>"}
      """
    Then I should see 401 status code
    And I should not be authenticated
    Examples:
      | username | password             |
      |          | kRk9mopFE@pzR]4WtujY |
      | user1    |                      |

  Scenario: Successful sign out
    Given there are some registered users
      | username | password             |
      | user1    | kRk9mopFE@pzR]4WtujY |
      | user2    | W2W3FBnfXkw*&wjwqbaw |
    And I send POST request to "/signin" with
      """
      {"username": "user1", "password": "kRk9mopFE@pzR]4WtujY"}
      """
    Then I should be authenticated
    When I send POST request to "/signout"
    Then I should not be authenticated