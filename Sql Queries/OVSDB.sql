use OVSDB;
Alter table Candidate alter column DateOfBirth Date;
Alter table Election alter column EndDate Date;
Alter table Users alter column CanVote varchar(10);
ALTER TABLE Users
ADD  Gender VARCHAR(50);

select * from users;
select * from ElectionResult;
CREATE TABLE Users(
    Userid int IDENTITY(1,1) PRIMARY KEY,
    LastName varchar(255) NOT NULL,
    FirstName varchar(255),  
    DateOfBirth date,
	email varchar(255) NOT NULL,
    password varchar(255),
	address varchar(255),
	PhoneNo varchar(255),
	CanVote int

  
);

CREATE TABLE Election(
    Electionid int IDENTITY(1,1) PRIMARY KEY,
    ElectionName varchar(255),
    EndDate varchar(100)
	);


CREATE TABLE Candidate(
    Candidateid int IDENTITY(1,1) PRIMARY KEY,
    LastName varchar(255) NOT NULL,
    FirstName varchar(255),
    DateOfBirth varchar(100),
	TypeElection int,
	Image varchar(500),
	address varchar(255),
	PhoneNo varchar(255)
	FOREIGN KEY (TypeElection) REFERENCES Election(Electionid)

  
);
drop table ElectionVotes;
CREATE TABLE ElectionVotes(
	 ElectionVoteId int IDENTITY(1,1) PRIMARY KEY,
	 UserId int,
	 CandidateId int,
	 ElectionId int
	 FOREIGN KEY (ElectionId) REFERENCES Election(Electionid),
	 FOREIGN KEY (CandidateId) REFERENCES Candidate(Candidateid),
	 FOREIGN KEY (UserId) REFERENCES Users(Userid)
	 );

CREATE TABLE ElectionResult(
	 ElectionResultId int IDENTITY(1,1) PRIMARY KEY,
	 ElectionId int,
	 CandidateId int,
	 votes int
	 );
	 Select * from Users;
	  Select * from Election;
	 Update Users set password='admin' where Userid=6
	 Update Users set CanVote='true' 
	 Update Users set DateOfBirth='0001-01-01' Where userid=16;
	  Select * from Candidate Where Candidateid=17;
	  Select * from ElectionVotes;
	  select * from ElectionResult;
	  

	  CREATE TABLE Gen(
    GenderValue varchar(10) PRIMARY KEY,
    GenderName varchar(255)
   
	);
	select * from election
	insert into Gen(GenderValue,GenderName) values('F','Female');
	insert into Gen(GenderValue,GenderName) values('M','Male');
	insert into Gen(GenderValue,GenderName) values('O','Other');
