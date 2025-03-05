USE TareasDB1

CREATE TABLE Usuarios (
id INT IDENTITY(1,1) PRIMARY KEY,
nombreUsuario VARCHAR(50) UNIQUE NOT NULL,
contrasena VARCHAR(MAX) NOT NULL,
correo VARCHAR(100) UNIQUE NULL,
fechaCreacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE Categorias (
id INT IDENTITY(1,1) PRIMARY KEY,
nombre VARCHAR(100) UNIQUE NOT NULL,
descripcion TEXT NULL,
fechaCreacion DATETIME DEFAULT GETDATE()
);

CREATE TABLE Estados (
id INT IDENTITY(1,1) PRIMARY KEY,
nombreEstado VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE Tareas (
id INT IDENTITY(1,1) PRIMARY KEY,
titulo VARCHAR(255) NOT NULL,
descripcion TEXT NOT NULL,
categoriaId INT NOT NULL,
usuarioId INT NOT NULL,
estadoId INT NOT NULL,
fechaCreacion DATETIME DEFAULT GETDATE(),
fechaVencimiento DATE NULL,
FOREIGN KEY (categoriaId) REFERENCES Categorias(id) ON DELETE CASCADE,
FOREIGN KEY (usuarioId) REFERENCES Usuarios(id) ON DELETE CASCADE,
FOREIGN KEY (estadoId) REFERENCES Estados(id) ON DELETE CASCADE
);

INSERT INTO Usuarios (nombreUsuario, contrasena)VALUES ('admin', 'admin123');
INSERT INTO Estados (nombreEstado) Values ('Pendiente'), ('En Progreso'), ('Completa');
INSERT INTO Categorias(nombre) VALUES ('lógica'),('Comprensión Lectora'),('Física');
INSERT INTO Categorias(descripcion,fechaCreacion) VALUES ('Pensamiento Lógico'),('02-05-25')


SELECT * FROM Categorias;

SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Usuarios';

USE master;
GO
CREATE LOGIN admin WITH PASSWORD = 'admin123';
GO

CREATE USER admin FOR LOGIN admin;
SELECT name FROM sys.sql_logins WHERE name = 'admin';
GO

SELECT name FROM sys.database_principals WHERE name = 'admin';
GO

ALTER ROLE db_owner ADD MEMBER admin;
GO

USE master;
GO
SELECT name FROM sys.sql_logins WHERE name = 'admin';
GO

SELECT name FROM sys.database_principals WHERE name = 'admin';
GO

ALTER ROLE db_owner ADD MEMBER admin;
GO

SELECT 
    USER_NAME(grantee_principal_id) AS Usuario,
    permission_name AS Permiso,
    state_desc AS Estado
FROM sys.database_permissions
WHERE grantee_principal_id = USER_ID('admin');
GO

ALTER ROLE db_datareader ADD MEMBER admin; -- Permite leer datos
ALTER ROLE db_datawriter ADD MEMBER admin; -- Permite escribir datos
GO

GRANT SELECT, INSERT, UPDATE, DELETE TO admin;
GO

ALTER ROLE db_owner ADD MEMBER admin;
GO

SELECT 
    USER_NAME(grantee_principal_id) AS Usuario,
    permission_name AS Permiso,
    state_desc AS Estado
FROM sys.database_permissions
WHERE grantee_principal_id = USER_ID('admin');
GO

sp_help 'Tareas';
GO