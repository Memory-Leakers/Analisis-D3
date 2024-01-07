USE robertri;

CREATE TABLE Sessions
(
    Id int NOT NULL AUTO_INCREMENT,
    start_datetime DATETIME,
    end_datetime DATETIME,
    
    PRIMARY KEY(Id)
);

CREATE TABLE Events
(
    Id int NOT NULL AUTO_INCREMENT,
    Type varchar(255),
    Level int,
    Position_X FLOAT,
    Position_Y FLOAT,
    Position_Z FLOAT,
    Session_id int NOT NULL,
    date DATETIME,
    step int,
    
    PRIMARY KEY(Id),
    FOREIGN KEY(Session_id) REFERENCES Sessions(Id)
);