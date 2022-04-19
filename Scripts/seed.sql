CREATE TABLE Accounts
(
    id varchar (36) DEFAULT (uuid()) PRIMARY KEY,
    username  VARCHAR (50)  NOT NULL,
    email  VARCHAR (100)  NOT NULL,
    password  VARCHAR (100)  NOT NULL
);

Insert into Accounts(username,email,password) values( 'Name 1','email1@mail.com', '123123123');
Insert into Accounts(username,email,password) values( 'Name 2','email2@mail.com', '123123123');
Insert into Accounts(username,email,password) values( 'Name 3','email3@mail.com', '123123123');
Insert into Accounts(username,email,password) values( 'Name 4','email4@mail.com', '123123123');