CREATE TABLE Alumnos(
	Nombre NVarchar(50) not null, 
	DNI NVarchar(8) not null UNIQUE,
)
ALTER TABLE Alumnos ADD Id int IDENTITY(1, 1) not null
ALTER TABLE Alumnos ADD PRIMARY KEY (Id)

CREATE TABLE Horarios(
	Dia NVarchar(10) not null, 
	Turno NVarchar(6) not null
)
ALTER TABLE Horarios ADD Id int IDENTITY(1, 1) not null
ALTER TABLE Horarios ADD PRIMARY KEY (Id)

CREATE TABLE MateriaHorarios(
	Id int IDENTITY(1, 1) not null, 
	MateriaId int not null, 
	HorarioId int not null
)
ALTER TABLE Horarios ADD PRIMARY KEY (Id)

CREATE TABLE MateriaAlumno(
	Id int IDENTITY(1, 1) not null PRIMARY KEY, 
	AlumnoId int not null, 
	MateriaHorarioId int not null, 
	Nota smallint not null,
	Estado bit not null
)

CREATE TABLE Correlativas(
	Id int IDENTITY(1, 1) not null PRIMARY KEY, 
	Materia_ppal int not null,
	Materia_correlativa int not null, 
)

CREATE TABLE Seguidores(
	Id int IDENTITY(1, 1) not null PRIMARY KEY, 
	Alumnoid int not null,
	Seguido int not null, 
)

SELECT * FROM Alumnos