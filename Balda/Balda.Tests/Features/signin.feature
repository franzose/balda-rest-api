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
    And I should be authenticated as the "user1" user