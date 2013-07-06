Feature: Election
	In order to make choices about how I allocate money to my benefits
	As a participant
	I want to view and manage my elections

Scenario: Create an election
	//Given I have no current elections
	When I issue a create election command
	Then I should receive an election created event

Scenario: Terminate election
	Given I have an existing election:
		| AdmCode | CoCode | EmpNum    | PlanCode | PlanYear | BenefitCode | ElectionAmount | ElectionId |
		| MHMRASP | MHM    | 123456789 | 22       | 2012     | MEDFSA      | 1200           | 123        |
	When I send a terminate election command
	Then the election terminated event should be raised
