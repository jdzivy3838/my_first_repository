Feature: Browse App
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

@mytag @SIT @STAGING
Scenario Outline: Open Google and find FPT page
	Given I navigate to page '<uri>'
	And   I search company '<company>'
	And   I open company '<company>'
	When Item is clicked in menu '<menu_option>' 
	Then the opened page contains text '<message>'
	Examples: 
	| uri_name	| company      | menu_option | message    |
	| google	| FPT Slovakia | Kontakt     | My sme FPT |
