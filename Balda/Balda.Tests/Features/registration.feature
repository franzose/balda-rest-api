Feature: Registration
  In order to play Balda
  As a Visitor
  I should be able to register in the game

  Scenario: Successful registration
    When I send POST request to "/register" with
      """
      {"username": "foo", "password": "kRk9mopFE@pzR]4WtujY"}
      """
    Then I should see 200 status code
    And I should be registered as "foo" with the "kRk9mopFE@pzR]4WtujY" password
    And I should see the "You have been registered successfully!" message

  Scenario Outline: Missing credential
    When I send POST request to "/register" with
      """
      {"username": "<username>", "password": "<password>"}
      """
    Then I should see 401 status code
    And I should not be registered as "<username>"
    Examples:
      | username | password             |
      |          | kRk9mopFE@pzR]4WtujY |
      | foo      |                      |

  Scenario: Trying to register with an already taken username
    Given There are some registered users
      | username | password             |
      | foo      | kRk9mopFE@pzR]4WtujY |
      | bar      | W2W3FBnfXkw*&wjwqbaw |
    When I send POST request to "/register" with
      """
      {"username": "foo", "password": "NxixLfR[DQQ9PBw)AxN6"}
      """
    Then I should see 401 status code
    And the "foo" user should have "kRk9mopFE@pzR]4WtujY" password
    And I should see the "Username 'foo' is already taken." message