\connect octopus_db

CREATE TABLE Users
(
    id serial PRIMARY KEY,
    Username  VARCHAR (50)  NOT NULL,
    description  VARCHAR (100)  NOT NULL
);

INSERT INTO test (Username) VALUES
('Name_1');