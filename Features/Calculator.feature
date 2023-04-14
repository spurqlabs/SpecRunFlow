Feature: Calculator
	Simple calculator for adding two numbers

	@smoke
Scenario: Addition of two numbers
	Given User is on Calculator page
	When User enters 5
	And User press '+'
	And User enters 4
	And User press '='
	Then User verifies result is 9

	@smoke
	Scenario: Subtraction of two numbers
	Given User is on Calculator page
	When User enters 5
	And User press '-'
	And User enters 4
	And User press '='
	Then User verifies result is 1

	@smoke
	Scenario: Multiplication of two numbers
	Given User is on Calculator page
	When User enters 6
	And User press '*'
	And User enters 2
	And User press '='
	Then User verifies result is 12

	@smoke
	Scenario: Division of two numbers
	Given User is on Calculator page
	When User enters 8
	And User press '/'
	And User enters 4
	And User press '='
	Then User verifies result is 2