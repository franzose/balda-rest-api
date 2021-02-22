Feature: Registration
  In order to play Balda
  As a Visitor
  I should be able to register in the game

  Scenario: Successful registration
    When I register using the following credentials
      | username | password             |
      | foo      | kRk9mopFE@pzR]4WtujY |
    Then I should see 200 status code
    And I should be registered
    And I should see the "You have been registered successfully!" message

  Scenario Outline: Missing credential
    When I register using the following credentials
      | username   | password   |
      | <username> | <password> |
    Then I should see 401 status code
    And I should not be registered
    Examples:
      | username | password             |
      |          | kRk9mopFE@pzR]4WtujY |
      | foo      |                      |

  Scenario: Trying to register with an already taken username
    Given There are some registered users
      | username | password             |
      | foo      | kRk9mopFE@pzR]4WtujY |
      | bar      | W2W3FBnfXkw*&wjwqbaw |
    When I register using the following credentials
      | username | password             |
      | foo      | NxixLfR[DQQ9PBw)AxN6 |
    Then I should see 401 status code
    And the "foo" user should have "kRk9mopFE@pzR]4WtujY" password
    And I should see the "Username 'foo' is already taken." message