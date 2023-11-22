DROP DATABASE IF EXISTS T02_BBDD;
CREATE DATABASE T02_BBDD;
USE T02_BBDD;

CREATE TABLE jugadores (
	id INT not NULL, 
	PRIMARY KEY(id),
	nombre VARCHAR(25),
	contrasenya VARCHAR(20),
	mail VARCHAR(100)
)ENGINE = InnoDB;

CREATE TABLE partidas (
	id INT not NULL,
	PRIMARY KEY(id),
	fecha VARCHAR(10),
	ganador VARCHAR(25) DEFAULT '-',
	duracion INT not NULL
)ENGINE = InnoDB;

CREATE TABLE registro (
	idJ INT,
	FOREIGN KEY(idJ) REFERENCES jugadores(id),
	idP INT,
	FOREIGN KEY(idP) REFERENCES partidas(id),
	puntos INT not NULL
)ENGINE = InnoDB;

INSERT INTO jugadores VALUES (1, 'Bruno', 'bruno123','bruno.colitti@estudiantat.upc.edu');
INSERT INTO jugadores VALUES (2, 'Alba', 'albagonzalez2','alba.gonzalez@estudiantat.upc.edu');
INSERT INTO jugadores VALUES (3, 'Antonia', 'mimara','agallardo@upc.edu');
INSERT INTO jugadores VALUES (4, 'Miguel', 'valerogarcia','miguel.valero@upc.edu');

INSERT INTO partidas VALUES (1, '05/11/2023', 'Bruno',100);
INSERT INTO partidas VALUES (2, '05/11/2023', 'Alba', 500);
INSERT INTO partidas VALUES (3, '05/11/2023', 'Antonia',750);
INSERT INTO partidas VALUES (4, '05/11/2023', 'Miguel',240);

INSERT INTO registro VALUES (1, 1, 10);
INSERT INTO registro VALUES (2, 1, 8);
INSERT INTO registro VALUES (2, 2, 7);
INSERT INTO registro VALUES (3, 2, 5);
INSERT INTO registro VALUES (4, 3, 11);
INSERT INTO registro VALUES (2, 3, 9);
INSERT INTO registro VALUES (4, 4, 8);
INSERT INTO registro VALUES (1, 4, 7);
INSERT INTO registro VALUES (3, 5, 6);
INSERT INTO registro VALUES (2, 5, 5);
