Feature: APIExamples
    In order to work with API
    As a framework user
    I want to be able to view, create, update, and delete an entity

@2342
Scenario: Verify the ability to create a new ToDo Item through API
	When I send request to create a new ToDo item with the following information:
		| name  | isComplete |
		| Drink | false      |
	Then I get the response back with status code 'Created'

@2342
Scenario: Verify the ability to change a ToDO Item through API
	Given I have created a ToDo Item with following information:
		| name | isComplete |
		| Code | true       |
	When I send a request to update a ToDo item with the following information:
		| name | isComplete |
		| Eat  | false      |
	Then I see the response with status code '204'

@2342
Scenario: Verify the ability to view/get ToDo Item through API
	When I send request to view all the ToDo Item
	Then I see the response with status code '200'